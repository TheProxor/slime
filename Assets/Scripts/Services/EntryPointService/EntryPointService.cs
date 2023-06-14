using System;
using TheProxor.Services.Stage;
using TheProxor.UI;
using UnityEngine;
using Zenject;


namespace TheProxor.Services.EntryPoint
{

	public class EntryPointService : IEntryPointService, IInitializable
	{
		private readonly IStageService<StageType, StageBehaviour, StageView> stageService;
		private readonly Settings settings;



		public EntryPointService(IStageService<StageType, StageBehaviour, StageView> stageService,
								 Settings settings)
		{
			this.stageService = stageService;
			this.settings = settings;
		}


		public void Initialize() =>
			OnEntryPointReached();


		public void OnEntryPointReached()
		{
			Application.targetFrameRate = settings.TargetFrameRate;

			stageService.SwitchState(StageType.CraftTable);
		}



		[Serializable]
		public class Settings
		{
			[field: SerializeField] public int TargetFrameRate { get; private set; } = 60;
		}
	}
}
