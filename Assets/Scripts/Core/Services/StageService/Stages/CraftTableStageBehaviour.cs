using Cinemachine;
using DG.Tweening;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Infrastructure;
using TheProxor.Services.Camera;
using TheProxor.Services.UI;
using TheProxor.UI;



namespace TheProxor.Services.Stage
{
	public class CraftTableStageBehaviour : StageBehaviour<CraftTableStageView>
	{
		private readonly IWindowsService windowsService;
		private readonly ICameraService<CinemachineVirtualCamera> cameraService;
		private readonly SlimeFactory factory;

		private SlimeMetaGame slime;


		public CraftTableStageBehaviour(IWindowsService windowsService,
										ICameraService<CinemachineVirtualCamera> cameraService,
										SlimeFactory factory)
		{
			this.windowsService = windowsService;
			this.cameraService = cameraService;
			this.factory = factory;
		}



		public override void Initialize()
		{

		}


		public override void Deinitialize()
		{
			throw new System.NotImplementedException();
		}


		public override void OnStateBegin()
		{
			StageView.EnableView();
			windowsService.TryShowWindow<CraftTableWindow, ImmediatelyTransitionController>();
			cameraService.SetVirtualCameraActive(StageView.FrontVirtualCamera);

			DOVirtual.DelayedCall(0.0f, () =>
			{
				slime = factory.Create();
				slime.Initialize();
			});
		}


		public override void OnStateSyncUpdate()
		{
			throw new System.NotImplementedException();
		}


		public override void OnStateEnd()
		{

		}


		public void SetupSlimeInteractionsMode()
		{
			cameraService.SetVirtualCameraActive(StageView.TopVirtualCamera);
		}
	}
}
