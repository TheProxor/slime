using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Infrastructure;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Input;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic
{
	public class SlimeInteractionController
	{
		private readonly SlimeFacade slime;
		private readonly CreationInput input;

		private bool isEnabled;

		public bool IsEnabled
		{
			get => isEnabled;

			set
			{
				if (isEnabled == value)
				{
					return;
				}

				isEnabled = value;

				UpdateInputInitialization();
			}
		}

		public SlimeInteractionController(SlimeFacade slime, CreationInput input)
		{
			this.slime = slime;
			this.input = input;
			UpdateInputInitialization();
		}

		private void UpdateInputInitialization()
		{
			if (isEnabled)
			{
				InitializeInput();
			}
			else
			{
				DeInitializeInput();
			}
		}

		private void InitializeInput()
		{
			input.OnInteractionStart += MakeSlimeInteractable;
			input.OnInteractionFinish += MakeSlimeNotInteractable;
		}

		private void DeInitializeInput()
		{
			input.OnInteractionStart -= MakeSlimeInteractable;
			input.OnInteractionFinish -= MakeSlimeNotInteractable;
		}

		private void MakeSlimeInteractable()
		{
			slime.SetInteractable(true);
		}

		private void MakeSlimeNotInteractable()
		{
			slime.SetInteractable(false);
		}
	}
}
