using System;
using UnityEngine;
using Zenject;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.States
{
	public partial class SlimeCreationPlayState
	{
		public class Installer : Installer<Installer>
		{
			[Serializable]
			public class Settings
			{
				[SerializeField]
				private SlimePlayEvaluationState.Settings slimePlayEvaluationState = null;

				public SlimePlayEvaluationState.Settings SlimePlayEvaluationState =>
					slimePlayEvaluationState;
			}

			private readonly Settings settings;

			public Installer(Settings settings)
			{
				this.settings = settings;
			}

			public override void InstallBindings()
			{
				InstallSlimeCreationPlayState();
				InstallSlimePlayEvaluationState();
			}

			private void InstallSlimeCreationPlayState()
			{
				Container.Bind<SlimeCreationPlayState>()
						 .AsSingle();
			}

			private void InstallSlimePlayEvaluationState()
			{
				Container.Bind<SlimePlayEvaluationState>()
						 .AsSingle()
						 .WithArguments(settings.SlimePlayEvaluationState);
			}
		}
	}
}
