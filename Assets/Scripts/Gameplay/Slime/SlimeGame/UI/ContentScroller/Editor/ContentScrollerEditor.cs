using UnityEditor;
using UnityEngine;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.UI
{
	[CustomEditor(typeof(ContentScroller))]
	public class ContentScrollerEditor : UnityEditor.Editor
	{
		private ContentScroller contentScroller;

		private SerializedProperty paddingProp;
		private SerializedProperty childAlignmentProp;
		private SerializedProperty spacingProp;
		private SerializedProperty aspectRatioProp;
		private SerializedProperty pageIndexProp;
		private SerializedProperty pageCountProp;
		private SerializedProperty pageSizeProp;
		private SerializedProperty animateScrollProp;
		private SerializedProperty scrollDurationProp;
		private SerializedProperty scrollCurveProp;
		private SerializedProperty scrollChildDelayProp;
		private SerializedProperty nextPageButtonProp;
		private SerializedProperty previousPageButtonProp;
		private SerializedProperty isAnimatingProp;
		private SerializedProperty pageElementsCountEnabledProp;
		private SerializedProperty pageElementsCountMinProp;
		private SerializedProperty pageElementsCountMaxProp;
		private SerializedProperty pageContentSizeProp;

		private void OnEnable()
		{
			contentScroller = target as ContentScroller;

			paddingProp = serializedObject.FindProperty("m_Padding");
			childAlignmentProp = serializedObject.FindProperty("m_ChildAlignment");
			spacingProp = serializedObject.FindProperty("spacing");
			aspectRatioProp = serializedObject.FindProperty("aspectRatio");
			pageIndexProp = serializedObject.FindProperty("pageIndex");
			pageCountProp = serializedObject.FindProperty("pageCount");
			pageSizeProp = serializedObject.FindProperty("pageSize");
			animateScrollProp = serializedObject.FindProperty("animateScroll");
			scrollDurationProp = serializedObject.FindProperty("scrollDuration");
			scrollCurveProp = serializedObject.FindProperty("scrollCurve");
			scrollChildDelayProp = serializedObject.FindProperty("scrollChildDelay");
			nextPageButtonProp = serializedObject.FindProperty("nextPageButton");
			previousPageButtonProp = serializedObject.FindProperty("previousPageButton");
			isAnimatingProp = serializedObject.FindProperty("isAnimating");
			pageElementsCountEnabledProp = serializedObject.FindProperty("isPageElementsCountLimitEnabled");
			pageElementsCountMinProp = serializedObject.FindProperty("pageElementsCountLimitMin");
			pageElementsCountMaxProp = serializedObject.FindProperty("pageElementsCountLimitMax");
			pageContentSizeProp = serializedObject.FindProperty("contentSizeLimit");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(paddingProp);
			EditorGUILayout.PropertyField(childAlignmentProp);
			EditorGUILayout.PropertyField(spacingProp);

			EditorGUILayout.PropertyField(pageElementsCountEnabledProp);
			if (pageElementsCountEnabledProp.boolValue)
			{
				EditorGUILayout.PropertyField(pageElementsCountMinProp);
				EditorGUILayout.PropertyField(pageElementsCountMaxProp);
				EditorGUILayout.PropertyField(pageContentSizeProp);
			}

			EditorGUILayout.PropertyField(aspectRatioProp);
			EditorGUILayout.PropertyField(previousPageButtonProp);
			EditorGUILayout.PropertyField(nextPageButtonProp);

			DrawAnimateScrollProp();

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PrefixLabel(pageIndexProp.displayName);

				pageIndexProp.intValue = EditorGUI.IntSlider(EditorGUILayout.GetControlRect(),
															 pageIndexProp.intValue, 0,
															 pageCountProp.intValue - 1);

				EditorGUILayout.LabelField($"({pageCountProp.intValue})",
										   GUILayout.ExpandWidth(false));
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PrefixLabel("Navigation");
				EditorGUI.BeginDisabledGroup(!Application.isPlaying);
				{
					Rect navigationRect = EditorGUILayout.GetControlRect();
					float halfRectWidth = navigationRect.width / 2;

					EditorGUI.BeginDisabledGroup(!contentScroller.IsCanGoToPreviousPage);
					{
						var rect = new Rect(navigationRect.x,
											navigationRect.y,
											halfRectWidth,
											navigationRect.height);

						if (GUI.Button(rect, "Previous Page"))
						{
							contentScroller.GoToPreviousPage();
						}
					}
					EditorGUI.EndDisabledGroup();

					EditorGUI.BeginDisabledGroup(!contentScroller.IsCanGoToNextPage);
					{
						var rect = new Rect(navigationRect.x + halfRectWidth,
											navigationRect.y,
											halfRectWidth,
											navigationRect.height);

						if (GUI.Button(rect, "Next Page"))
						{
							contentScroller.GoToNextPage();
						}
					}
					EditorGUI.EndDisabledGroup();
				}
				EditorGUI.EndDisabledGroup();
			}
			EditorGUILayout.EndHorizontal();

			serializedObject.ApplyModifiedProperties();

			DrawInfo();
		}

		private void DrawAnimateScrollProp()
		{
			Rect rect = EditorGUILayout.GetControlRect();

			bool animate = animateScrollProp.boolValue;
			animate = animateScrollProp.boolValue =
						  EditorGUI.Toggle(rect, animateScrollProp.displayName, animate);

			if (!animate)
			{
				return;
			}

			bool isExpanded = animateScrollProp.isExpanded;
			isExpanded = animateScrollProp.isExpanded =
							 EditorGUI.Foldout(rect, isExpanded, "", true);

			if (!isExpanded)
			{
				return;
			}

			using (new EditorGUI.IndentLevelScope())
			{
				EditorGUILayout.PropertyField(scrollDurationProp);
				using (new EditorGUI.IndentLevelScope())
				{
					EditorGUILayout.Slider(scrollChildDelayProp, 0, 1, "Delay");
				}

				EditorGUILayout.PropertyField(scrollCurveProp);
			}
		}

		private void DrawInfo()
		{
			var message = $"Page Size: {pageSizeProp.intValue}";

			if (animateScrollProp.boolValue
				&& Application.isPlaying)
			{
				message += $"\nIs Animating: {isAnimatingProp.boolValue}";
			}

			EditorGUILayout.HelpBox(message, MessageType.Info);
		}
	}
}
