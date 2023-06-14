using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Infrastructure;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.SaveSystem;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.States
{
	public class SaveSlimeState : State
	{
		private readonly SlimeFacade slime;
		private readonly SlimeSaver slimeSaver;

		public SaveSlimeState(SlimeFacade slime, SlimeSaver slimeSaver)
		{
			this.slime = slime;
			this.slimeSaver = slimeSaver;
		}


		public override void Enter()
		{
			base.Enter();
			SaveSlime();
			Finish();
		}

		private void SaveSlime()
		{
			SlimeSaveData saveData = slime.GetSaveData();
			slimeSaver.AddSlimeSave(saveData);
		}
	}
}
