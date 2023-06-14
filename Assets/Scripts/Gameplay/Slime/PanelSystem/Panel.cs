using System;
using UnityEngine;
using Zenject;

namespace TheProxor.PanelSystem
{
	public class Panel : MonoBehaviour, IPanel
	{
		public class Factory : PlaceholderFactory<Panel, Panel> {}

		public event Action<Panel> OnBeginShow;
		public event Action<Panel> OnBeginHide;
		public event Action<Panel> OnEndShow;
		public event Action<Panel> OnEndHide;

		[SerializeField] private PanelType type;

		private PanelManager panelManager;

		private bool hasPanelManager = false;

		public PanelState State { get; private set; }
		public PanelType PanelType => type;
		public PanelType SubstitutedPanel { get; protected set; } = PanelType.None;
		public PanelTransitionInfo NextPanelInfo { get; private set; }

		public void Show(bool instantly)
		{
			if (State == PanelState.Shown
				|| (State == PanelState.ShowProcess && !instantly))
			{
				return;
			}

			gameObject.SetActive(true);
			if (!instantly)
			{
				BeginShow();
				State = PanelState.ShowProcess;
			}
			else
			{
				if (State != PanelState.ShowProcess)
				{
					BeginShow();
				}

				EndShow();
				State = PanelState.Shown;
			}
		}

		public void Hide(bool instantly)
		{
			if (State == PanelState.Hidden
				|| (State == PanelState.HidingProcess && !instantly))
			{
				return;
			}

			if (!instantly)
			{
				if (State != PanelState.None)
				{
					BeginHide();
				}

				State = PanelState.HidingProcess;
			}
			else
			{
				if (State != PanelState.HidingProcess
					&& State != PanelState.None)
				{
					BeginHide();
				}

				EndHide();
				State = PanelState.Hidden;
			}
		}

		public void ResetNext()
		{
			NextPanelInfo = PanelTransitionInfo.Empty;
		}

		public void SetSubstitutedPanel(PanelType value)
		{
			SubstitutedPanel = value;
		}

		public void OpenSubstitutedPanel(PanelTransitionType transitionType)
		{
			if (SubstitutedPanel != PanelType.None)
			{
				SetNext(SubstitutedPanel, transitionType);
				SubstitutedPanel = PanelType.None;
			}

			Hide(false);
		}

		internal void SetPanelManager(PanelManager value)
		{
			panelManager = value;
			hasPanelManager = !ReferenceEquals(panelManager, null);
		}

		protected void SetNext(PanelType nextPanel,
							   PanelTransitionType transitionType = PanelTransitionType.Instantly)
		{
			NextPanelInfo = new PanelTransitionInfo(nextPanel, transitionType);
		}

		protected virtual void UpdateProcessing() {}

		protected virtual void BeginShowProcess() {}

		protected virtual void BeginHideProcess() {}

		protected virtual void InstantlyHide() {}

		protected virtual void InstantlyShow() {}

		protected virtual void ShowProcess(out bool panelIsShownToEnd)
		{
			panelIsShownToEnd = true;
		}

		protected virtual void HideProcess(out bool panelIsHiddenToEnd)
		{
			panelIsHiddenToEnd = true;
		}

		protected virtual void BackButtonHandler() {}

		internal void Processing()
		{
			switch (State)
			{
				case PanelState.HidingProcess:
				{
					HideProcess(out bool panelIsHiddenToEnd);
					if (panelIsHiddenToEnd)
					{
						EndHide();
						State = PanelState.Hidden;
					}
				}

					break;

				case PanelState.ShowProcess:
				{
					ShowProcess(out bool panelIsShownToEnd);
					if (panelIsShownToEnd)
					{
						EndShow();
						State = PanelState.Shown;
					}
				}

					break;

				case PanelState.Shown:
				{
					UpdateProcessing();
					if (hasPanelManager && panelManager.ActivateType == PanelType)
					{
						BackButtonProcessing();
					}
				}

					break;
			}
		}

		private void BackButtonProcessing()
		{
			#if UNITY_ANDROID || UNITY_EDITOR
			if (UnityEngine.Input.GetKeyDown(KeyCode.Escape) == false)
			{
				return;
			}

			BackButtonHandler();
			#endif
		}

		private void BeginShow()
		{
			if (OnBeginShow != null)
			{
				OnBeginShow(this);
			}

			BeginShowProcess();
		}

		private void BeginHide()
		{
			if (OnBeginHide != null)
			{
				OnBeginHide(this);
			}

			BeginHideProcess();
		}

		private void EndShow()
		{
			bool end = State != PanelState.Shown;
			InstantlyShow();
			if (end && OnEndShow != null)
			{
				OnEndShow(this);
			}
		}

		private void EndHide()
		{
			bool end = State != PanelState.Hidden;
			InstantlyHide();
			NextPanelInfo = PanelTransitionInfo.Empty;
			gameObject.SetActive(false);
			if (end && OnEndHide != null)
			{
				OnEndHide(this);
			}
		}
	}
}
