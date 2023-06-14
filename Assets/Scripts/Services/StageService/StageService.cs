using System;
using System.Collections.Generic;
using TheProxor.Utils;
using UnityEngine;
using Zenject;


namespace TheProxor.Services.Stage
{
	public class StageService : IStageService<StageType, StageBehaviour, StageView>
	{
		private readonly Settings settings;
		private readonly IDictionary<StageType, StageBehaviour> stageBehaviours;

		private StageType currentStageType;
		private IStageBehaviour currentStageBehaviour;



		public StageService(Settings settings,
							IDictionary<StageType, StageBehaviour> stageBehaviours,
							IFactory<StageView, StageView> stageViewFactory)
		{
			this.settings = settings;
			this.stageBehaviours = stageBehaviours;

			foreach (var stageBehaviourKeyValue in stageBehaviours)
			{
				stageBehaviourKeyValue.Value.StageView
					= stageViewFactory.Create(settings.StageViews[stageBehaviourKeyValue.Key]);

				stageBehaviourKeyValue.Value.Initialize();
			}
		}



		public StageType CurrentState => currentStageType;



		public void SwitchState(StageType stateType)
		{
			if (!stageBehaviours.TryGetValue(stateType, out StageBehaviour stageBehaviour))
			{
				Debug.LogError($"Stage with type <b>{stateType}</b> does not supported");
				return;
			}

			currentStageBehaviour?.OnStateEnd();
			currentStageBehaviour = stageBehaviour;
			currentStageBehaviour.OnStateBegin();

			currentStageType = stateType;
		}



		public TTargetStateBehaviour GetStateBehaviour<TTargetStateBehaviour>(StageType stateType)
			where TTargetStateBehaviour : StageBehaviour
		{
			if (!stageBehaviours.TryGetValue(stateType, out StageBehaviour stageBehaviour))
			{
				Debug.LogError($"Stage with type <b>{stateType}</b> does not supported");
				return default;
			}

			return (TTargetStateBehaviour)stageBehaviour;
		}


		public TTargetStageView GetStageView<TTargetStageView>(StageType stateType)
			where TTargetStageView : StageView
		{
			if (!stageBehaviours.TryGetValue(stateType, out StageBehaviour stageBehaviour))
			{
				Debug.LogError($"Stage with type <b>{stateType}</b> does not supported");
				return default;
			}

			return (TTargetStageView)stageBehaviour.StageView;
		}


		void IStateMachine<StageType, StageBehaviour>.SwitchState<TState>() {}

		TTargetStateBehaviour IStateMachine<StageType, StageBehaviour>.GetStateBehaviour<TTargetStateBehaviour>() => default;



		[Serializable]
		public class Settings
		{
			[field: SerializeField] public SerializableDictionary<StageType, StageView> StageViews { get; private set; } = default;
		}
	}
}
