using System;
using System.Collections.Generic;
using TheProxor.Services.UI;
using UnityEngine;
using Zenject;


namespace TheProxor.PanelSystem
{
	public sealed class PanelManager : IInitializable, ITickable
	{
		private readonly Settings settings;
		private readonly Panel.Factory panelFactory;
		private readonly Transform panelsParent;
		private readonly Dictionary<PanelType, Panel> panelsDictionary = new();
		private readonly OpenedPanelController openedPanelController = new();
		private readonly PanelOpenerManager openerManager = new();

		private PanelType startPanelType = PanelType.None;

		public PanelType ActivateType
		{
			get
			{
				if (openedPanelController.TryGetLastPanel(out Panel currentPanel)
					&& currentPanel.State != PanelState.Hidden)
				{
					return currentPanel.PanelType;
				}

				return PanelType.None;
			}
		}


		public PanelManager(
			Settings settings,
			Panel.Factory panelFactory,
			IWindowsService windowsService
		)
		{
			this.settings = settings;
			this.panelFactory = panelFactory;
			panelsParent = windowsService.RootCanvas.transform;
		}


		public void Initialize()
		{
			foreach (Panel panelPrefab in settings.PanelsPrefabs)
			{
				var panel = panelFactory.Create(panelPrefab);
				AddPanel(panel);
			}

			openedPanelController.Setup(panelsDictionary.Values);
		}


		public void Deinitialize()
		{
			foreach (var panel in panelsDictionary.Values)
				GameObject.Destroy(panel.gameObject);
		}


		public void Tick()
		{
			openerManager.UpdateProcessing();
			openedPanelController.UpdateProcessing();
			bool hasOpenedPanel = openedPanelController.TryGetLastPanel(out Panel lastOpenedPanel);
			if (!hasOpenedPanel
				|| lastOpenedPanel.State == PanelState.Hidden)
			{
				return;
			}

			PanelTransitionInfo nextPanelInfo = lastOpenedPanel.NextPanelInfo;
			if (nextPanelInfo.NextPanel != PanelType.None)
			{
				TryShow(nextPanelInfo.NextPanel, nextPanelInfo.TransitionType);
			}
		}


		public bool TryShow(
			PanelType nextType,
			PanelTransitionType transitionType = PanelTransitionType.Instantly
		)
		{
			if (!TryGetPanel(nextType, out Panel nextPanel))
			{
				Debug.LogError("Can't get panel");

				return false;
			}

			// openerManager.SetTransition(transitionType, nextPanel,
			// 							openedPanelController.OpenedPanels);
			nextPanel.Show(false);

			return true;
		}


		public void HideCurrentPanel(bool instantly = true)
		{
			if (openedPanelController.TryGetLastPanel(out Panel currentPanel))
			{
				currentPanel.Hide(instantly);
			}
		}


		public void HidePanel(PanelType type, bool instantly = false)
		{
			if (!TryGetPanel(type, out Panel panel))
			{
				return;
			}

			panel.Hide(instantly);
		}


		public bool TryGetPanel(PanelType type, out Panel panel)
		{
			panel = null;
			return type != PanelType.None && panelsDictionary.TryGetValue(type, out panel);
		}


		private void AddPanel(Panel panel)
		{
			panel.transform.SetParent(panelsParent, false);
			panel.SetPanelManager(this);
			PanelType panelType = panel.PanelType;
			panelsDictionary.Add(panelType, panel);

			if (panelType == startPanelType)
				panel.Show(true);
			else
				panel.Hide(true);
		}



		[Serializable]
		public class Settings
		{
			[field: SerializeField] public List<Panel> PanelsPrefabs { get; set; }
		}
	}
}
