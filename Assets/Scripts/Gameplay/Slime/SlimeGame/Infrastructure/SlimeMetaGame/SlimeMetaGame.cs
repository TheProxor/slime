using System.Collections.Generic;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.DataBase;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Input;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.SaveSystem;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.States;
using TheProxor.PanelSystem;
using TheProxor.SlimeSimulation.InputSystem.MultiTouchService;
using UnityEngine;
using Zenject;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Infrastructure
{
	public partial class SlimeMetaGame
	{
		private readonly SlimeFacade slime;
		private readonly BowlFacade bowl;
		private readonly Camera camera;
		private readonly SlimeSelectionState slimeSelectionState;
		private readonly PanelManager panelManager;
		private readonly TickableManager tickableManager;
		private readonly IMultiTouchService touchService;
		private readonly CreationInput creationInput;
		private readonly ProgressEvaluator progressEvaluator;
		private readonly States.StateMachine stateMachine;
		private readonly ISlimeIngredientsManager<SlimeIngredientInfo> slimeIngredientsManager;


		public SlimeMetaGame(
			SlimeFacade slime,
			BowlFacade bowl,
			Camera camera,
			States.StateMachine.Factory stateMachineFactory,
			SlimeSelectionState slimeSelectionState,
			SlimeCreationState slimeCreationState,
			SlimeRecreationState slimeRecreationState,
			TickableManager tickableManager,
			PanelManager panelManager,
			IMultiTouchService touchService,
			PlaySlimeState playSlimeState,
			CreationInput creationInput,
			ProgressEvaluator progressEvaluator,
			ISlimeIngredientsManager<SlimeIngredientInfo> slimeIngredientsManager)
		{
			this.slime = slime;
			this.bowl = bowl;
			this.camera = camera;
			this.slimeSelectionState = slimeSelectionState;
			this.panelManager = panelManager;
			this.tickableManager = tickableManager;
			this.touchService = touchService;
			this.creationInput = creationInput;
			this.progressEvaluator = progressEvaluator;
			this.slimeIngredientsManager = slimeIngredientsManager;

			stateMachine = stateMachineFactory.Create(
				new ExitableState[]
				{
					slimeSelectionState,
					slimeCreationState,
					slimeRecreationState,
					playSlimeState
				}
			);

			slimeSelectionState.OnNewSlimeCreateSelected += GoToSlimeCreation;
			slimeSelectionState.OnDataSelected += GoToPlayState;
			slimeSelectionState.OnRecreate += GoToSlimeRecreationState;
			slimeCreationState.OnFinishStatus += OnSlimeCreationStateFinished;
			playSlimeState.OnFinish += GoToSlimeSelection;

			HideGameGraphics();
		}



		public void Initialize()
		{
			panelManager.Initialize();

			tickableManager.Add(panelManager);
			tickableManager.Add(touchService);
			tickableManager.Add(creationInput);
			tickableManager.Add(progressEvaluator);

			ShowGameGraphics();
			GoToSlimeSelection();
		}


		public void Dispose()
		{
			tickableManager.Remove(panelManager);
			tickableManager.Remove(touchService);
			tickableManager.Remove(creationInput);
			tickableManager.Remove(progressEvaluator);

			panelManager.Deinitialize();
			slime.DeInitialize();
			Object.Destroy(slime.Transform.gameObject);
			Object.Destroy(bowl.Transform.gameObject);
			Object.Destroy(slime.Transform.parent.gameObject);

			HideGameGraphics();
			StopStateMachine();
		}


		private void ShowGameGraphics()
		{
			slime.Show();
		}


		private void HideGameGraphics()
		{
			slime.Hide();
			slime.RemoveDecorations();
			bowl.SetActive(false);
		}


		private void GoToSlimeSelection()
		{
			stateMachine.TryEnter<SlimeSelectionState>();
			slimeSelectionState.ReloadSelectedSlime();
		}


		private void GoToSlimeCreation()
		{
			stateMachine.TryEnter<SlimeCreationState>();
		}


		private void GoToSlimeRecreationState(SlimeSaveData saveData)
		{
			stateMachine.TryEnter<SlimeRecreationState, SlimeSaveData>(saveData);
		}


		private void OnSlimeCreationStateFinished(FinishStatus finishStatus)
		{
			if (finishStatus != FinishStatus.Success)
			{
				slimeSelectionState.ReloadSelectedSlime();
				GoToSlimeSelection();
				return;
			}

			List<SlimeIngredientInfo> slimeIngredients = new()
			{
				slime.GetBasicsInfo(),
				slime.GetColorInfo()
			};
			slimeIngredients.AddRange(slime.GetDecorationInfos());
			slimeIngredientsManager.ConsumeIngredients(slimeIngredients);

			GoToSlimeSelection();
		}


		private void GoToPlayState(SlimeSaveData saveData)
		{
			stateMachine.TryEnter<PlaySlimeState>();
		}


		private void StopStateMachine()
		{
			stateMachine.ExitActiveState();
		}

		public class Factory : PlaceholderFactory<SlimeMetaGame> {}
	}
}
