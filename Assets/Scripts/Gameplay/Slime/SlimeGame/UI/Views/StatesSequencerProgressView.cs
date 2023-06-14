using System.Collections.Generic;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.States;
using UnityEngine;
using Zenject;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.UI.Views
{
	public class StatesSequencerProgressView : MonoBehaviour
	{
		[SerializeField]
		private SlicedFilledImage progressBar = null;

		private readonly List<CheckBox> checkBoxes = new();

		private StatesSequencer statesSequencer;
		private CheckBox.Pool checkMarkPool;
		private float checkBoxesRectXMin;
		private float checkBoxesRectWidth;
		private float overallProgress;
		private ProgressState currentHandlingState;


		[Inject]
		public void Construct(
			StatesSequencer statesSequencer,
			CheckBox.Pool checkMarkPool
		)
		{
			this.statesSequencer = statesSequencer;
			this.checkMarkPool = checkMarkPool;
			InitializeStatesSequencer();
		}


		private void Start()
		{
			InitializeCheckBoxesPlaceData();
			PrepareCheckBoxes();
		}


		private void InitializeStatesSequencer()
		{
			statesSequencer.OnStart += PrepareForProgressDisplay;
			statesSequencer.OnStartNextState += StartHandleState;
			statesSequencer.OnSequenceEnd += UpdateOverallProgress;
		}


		private void PrepareForProgressDisplay()
		{
			UpdateOverallProgress();
			UpdateProgressBar();
			PrepareCheckBoxes();
		}


		private void UpdateOverallProgress()
		{
			int currentExecutedStateIndex = statesSequencer.CurrentExecutedStateIndex;
			overallProgress = (float) currentExecutedStateIndex / statesSequencer.SubStatesCount;

			if (currentExecutedStateIndex == 0)
			{
				return;
			}

			checkBoxes[currentExecutedStateIndex - 1].SetStatus(true);
		}


		private void UpdateProgressBar()
		{
			progressBar.fillAmount = GetProgress();
		}


		private float GetProgress()
		{
			return overallProgress + currentHandlingState.Progress / statesSequencer.SubStatesCount;
		}


		private void StartHandleState()
		{
			if (currentHandlingState != null)
			{
				currentHandlingState.OnUpdateProgress -= UpdateProgressBar;
			}

			currentHandlingState = statesSequencer.ExecutedState;
			currentHandlingState.OnUpdateProgress += UpdateProgressBar;

			UpdateOverallProgress();
		}


		private void InitializeCheckBoxesPlaceData()
		{
			Rect progressBarRect = progressBar.rectTransform.rect;
			checkBoxesRectXMin = progressBarRect.xMin;
			checkBoxesRectWidth = progressBarRect.width;
		}


		private void PrepareCheckBoxes()
		{
			ClearCheckBoxes();
			SpawnCheckBoxes();
		}


		private void ClearCheckBoxes()
		{
			foreach (CheckBox checkBox in checkBoxes)
			{
				checkMarkPool.Despawn(checkBox);
			}

			checkBoxes.Clear();
		}


		private void SpawnCheckBoxes()
		{
			int subStatesCount = statesSequencer.SubStatesCount;

			for (var i = 0; i < subStatesCount; i++)
			{
				CheckBox checkBox = checkMarkPool.Spawn(false);
				PlaceCheckBoxOnProgressBar(checkBox.RectTransform, i, subStatesCount);
				checkBoxes.Add(checkBox);
			}
		}


		private void PlaceCheckBoxOnProgressBar(RectTransform rectTransform, int index, int count)
		{
			float offset = checkBoxesRectWidth * (index + 1) / count;
			rectTransform.anchoredPosition = new Vector2(checkBoxesRectXMin + offset, 0);
		}
	}
}
