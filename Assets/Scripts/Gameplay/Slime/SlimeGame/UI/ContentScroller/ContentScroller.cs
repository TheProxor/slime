using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.UI
{
	[ExecuteAlways]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(RectTransform))]
	public class ContentScroller : LayoutGroup
	{
		private event Action OnPageChanged;

		private const int FORWARD_DIR = 1;
		private const int BACKWARD_DIR = -1;

		[SerializeField]
		private float spacing = 0;

		[SerializeField]
		private bool isPageElementsCountLimitEnabled;

		[SerializeField, ShowIf(nameof(isPageElementsCountLimitEnabled))]
		private int pageElementsCountLimitMin;

		[SerializeField, ShowIf(nameof(isPageElementsCountLimitEnabled))]
		private int pageElementsCountLimitMax;

		[SerializeField, ShowIf(nameof(isPageElementsCountLimitEnabled))]
		private float contentSizeLimit;

		[SerializeField]
		private float aspectRatio = 1;

		[SerializeField]
		private int pageIndex = 0;

		[SerializeField]
		private int pageCount = 0;

		[SerializeField]
		private int pageSize = 0;

		[SerializeField]
		private bool animateScroll = false;

		[SerializeField]
		private float scrollDuration = 1;

		[SerializeField]
		private AnimationCurve scrollCurve = null;

		[SerializeField]
		private float scrollChildDelay = 0;

		[SerializeField]
		private Button previousPageButton;

		[SerializeField]
		private Button nextPageButton;

		[SerializeField]
		private bool isAnimating;

		private bool isDirty;
		private float cellSizeX;
		private float spacingX;
		private float freeSpaceX;

		public bool IsCanGoToPreviousPage { get; private set; }

		public bool IsCanGoToNextPage { get; private set; }

		public int PageSize
		{
			get => pageSize;

			set
			{
				pageSize = value;

				if (pageSize < 1)
				{
					return;
				}

				int childCount = transform.childCount;
				pageCount = childCount / pageSize + (childCount % pageSize > 0 ? 1 : 0);
				pageIndex = Mathf.Max(0, Mathf.Min(PageIndex, pageCount - 1));
			}
		}

		public bool IsAnimating => isAnimating;
		public int PageIndex => pageIndex;

		public override void CalculateLayoutInputVertical() {}

		public override void SetLayoutHorizontal()
		{
			UpdateXArrangeData();

			if (IsAnimating)
			{
				return;
			}

			ReArrangeChildXAxis(rectChildren);
		}

		public override void SetLayoutVertical()
		{
			float cellSizeY = GetCellSizeY();

			foreach (RectTransform rect in rectChildren)
			{
				SetChildAlongAxis(rect, 1, padding.top, cellSizeY);
			}
		}

		public void GoToPreviousPage()
		{
			if (!IsCanGoToPreviousPage)
			{
				return;
			}

			SwitchPage(BACKWARD_DIR);
			UpdateIsCanGoToPreviousPage();
		}

		public void GoToNextPage()
		{
			if (!IsCanGoToNextPage)
			{
				return;
			}

			SwitchPage(FORWARD_DIR);
			UpdateIsCanGoToNextPage();
		}

		private void UpdateXArrangeData()
		{
			UpdateCellSizeX();
			UpdateSpacing();
			UpdateFreeSpace();
			UpdatePageSize();
		}

		private void UpdateCellSizeX()
		{
			cellSizeX = GetCellSizeX();
		}

		private void UpdateSpacing()
		{
			spacingX = cellSizeX + spacing;
		}

		private void UpdateFreeSpace()
		{
			freeSpaceX = rectTransform.rect.size.x - padding.horizontal;
		}

		private void UpdatePageSize()
		{
			PageSize = (int)(freeSpaceX / spacingX);
			SetChildrenDirty();
			UpdateIsCanGoToNextPage();
			UpdateIsCanGoToPreviousPage();
		}

		private void ReArrangeChildXAxis(IReadOnlyList<RectTransform> rectChildren)
		{
			int rectChildrenCount = rectChildren.Count;
			float startXPos = GetFirstChildXPos(rectChildrenCount);

			for (var i = 0; i < rectChildrenCount; i++)
			{
				SetChildAlongAxis(rectChildren[i], 0, startXPos + spacingX * i, cellSizeX);
			}
		}


		protected override void OnEnable()
		{
			base.OnEnable();
			pageIndex = 0;
		}


		protected override void Start()
		{
			base.Start();
			InitializeButtons();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			DeInitializeButtons();
		}

		#if UNITY_EDITOR
		protected override void OnValidate()
		{
			base.OnValidate();
			ReInitializeButtons();
			SetChildrenDirty();
		}

		private void ReInitializeButtons()
		{
			DeInitializeButtons();
			InitializeButtons();
		}
		#endif

		private void InitializeButtons()
		{
			#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				return;
			}
			#endif

			if (nextPageButton != null)
			{
				nextPageButton.onClick.AddListener(GoToNextPage);
			}

			if (previousPageButton != null)
			{
				previousPageButton.onClick.AddListener(GoToPreviousPage);
			}
		}

		private void DeInitializeButtons()
		{
			#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				return;
			}
			#endif

			if (nextPageButton != null)
			{
				nextPageButton.onClick.RemoveListener(GoToNextPage);
			}

			if (previousPageButton != null)
			{
				previousPageButton.onClick.RemoveListener(GoToPreviousPage);
			}
		}

		private void SetChildrenDirty()
		{
			if (!IsActive())
			{
				return;
			}

			if (IsAnimating)
			{
				return;
			}

			isDirty = true;
		}

		private void Update()
		{
			if (!isDirty)
			{
				return;
			}

			UpdateActiveChildren();

			isDirty = false;
		}

		private void UpdateActiveChildren()
		{
			SetActiveChildren(PageIndex, PageIndex + 1);
		}

		private void SetActiveChildren(int fromPage, int toPage)
		{
			int childCount = transform.childCount;

			int startIndex = fromPage * pageSize;
			int endIndex = toPage * pageSize;

			for (var i = 0; i < childCount; i++)
			{
				Transform childTransform = transform.GetChild(i);
				GameObject childGameObject = childTransform.gameObject;
				childGameObject.SetActive(i >= startIndex && i < endIndex);
			}
		}

		private void SwitchPage(int dir)
		{
			if (IsAnimating)
			{
				return;
			}

			if (animateScroll && enabled)
			{
				StartCoroutine(PageScrollAnimation(PageIndex, dir));
				UpdatePageIndex(dir);

				return;
			}

			UpdatePageIndex(dir);
		}

		private void UpdateIsCanGoToPreviousPage()
		{
			IsCanGoToPreviousPage = PageIndex - 1 >= 0;
			UpdatePreviousButtonInteractable();
		}

		private void UpdateIsCanGoToNextPage()
		{
			IsCanGoToNextPage = PageIndex + 1 < pageCount;
			UpdateNextButtonInteractable();
		}

		private void UpdatePreviousButtonInteractable()
		{
			#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				return;
			}
			#endif

			if (previousPageButton != null)
			{
				previousPageButton.interactable = IsCanGoToPreviousPage;
			}
		}

		private void UpdateNextButtonInteractable()
		{
			#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				return;
			}
			#endif

			if (nextPageButton != null)
			{
				nextPageButton.interactable = IsCanGoToNextPage;
			}
		}

		private IEnumerator PageScrollAnimation(int pageIndex, int dir)
		{
			StartAnimating();

			List<RectTransform> currentPageChildren = GetPageChildren(pageIndex);
			List<RectTransform> nextPageChildren = GetPageChildren(pageIndex + dir);

			int currentPageChildrenCount = currentPageChildren.Count;
			int nextPageChildrenCount = nextPageChildren.Count;
			int allAnimatedChildrenCount = currentPageChildrenCount + nextPageChildrenCount;

			float timer = 0;
			while (timer < scrollDuration)
			{
				int scrollIndex = dir > 0 ? 0 : currentPageChildrenCount - 1;

				AnimateChildrenAlongXAxis(currentPageChildren,
										  dir,
										  timer,
										  scrollIndex);

				scrollIndex = dir > 0 ? currentPageChildrenCount : allAnimatedChildrenCount - 1;

				AnimateChildrenAlongXAxis(nextPageChildren,
										  dir,
										  timer,
										  scrollIndex,
										  rectTransform.rect.x * 2 * -dir);

				timer += Time.deltaTime;

				yield return null;
			}

			StopAnimating();
			SetChildrenDirty();
		}

		private void StartAnimating()
		{
			isAnimating = true;
		}

		private void StopAnimating()
		{
			isAnimating = false;
		}

		private void AnimateChildrenAlongXAxis(IReadOnlyList<RectTransform> children,
											   int dir,
											   float timer,
											   int scrollIndex,
											   float offset = 0)
		{
			int count = children.Count;
			float startXPos = GetFirstChildXPos(count) + offset;
			for (var i = 0; i < count; i++, scrollIndex += dir)
			{
				float animatedOffset = GetAnimationChildOffset(timer, i, scrollIndex, dir);
				SetChildAlongAxis(children[i], 0, startXPos + animatedOffset, cellSizeX);
			}
		}

		private List<RectTransform> GetPageChildren(int pageIndex)
		{
			var nextPageChildren = new List<RectTransform>();

			for (int i = pageIndex * pageSize;
				 i < Mathf.Min(transform.childCount, pageIndex * pageSize + pageSize);
				 i++)
			{
				Transform child = transform.GetChild(i);
				child.gameObject.SetActive(true);
				nextPageChildren.Add(child as RectTransform);
			}

			return nextPageChildren;
		}

		private float GetAnimationChildOffset(float timer,
											  int index,
											  int scrollIndex,
											  float dir)
		{
			float childScrollDuration = scrollDuration / (pageSize * 2);
			float childDelay = childScrollDuration * scrollChildDelay;
			float delayedChildScrollDuration = Mathf.Lerp(scrollDuration,
														  childScrollDuration,
														  scrollChildDelay);

			float timeNormalized = (timer - scrollIndex * childDelay) / delayedChildScrollDuration;
			float t = scrollCurve.Evaluate(timeNormalized);

			float posOffset = Mathf.LerpUnclamped(0, rectTransform.rect.x * 2, t);

			return spacingX * index + posOffset * dir;
		}

		private void UpdatePageIndex(int dir)
		{
			pageIndex = PageIndex + dir;
			InvokePageChangedEvent();
		}

		private void InvokePageChangedEvent()
		{
			OnPageChanged?.Invoke();
		}

		private float GetFirstChildXPos(int childrenCount)
		{
			return padding.left + GetAlignmentOnAxis(0) * (freeSpaceX - childrenCount * spacingX);
		}

		private float GetCellSizeX()
		{
			if (isPageElementsCountLimitEnabled)
			{
				int count = (int)((rectTransform.rect.size.x - padding.horizontal) / contentSizeLimit);
				count = Mathf.Clamp(count, pageElementsCountLimitMin, pageElementsCountLimitMax);

				float size = rectTransform.rect.size.x - padding.horizontal;
				return aspectRatio * size / count;
			}

			return GetCellSizeY() * aspectRatio;
		}

		private float GetCellSizeY()
		{
			return Mathf.Max(rectTransform.rect.size.y - padding.vertical, 0);
		}
	}
}
