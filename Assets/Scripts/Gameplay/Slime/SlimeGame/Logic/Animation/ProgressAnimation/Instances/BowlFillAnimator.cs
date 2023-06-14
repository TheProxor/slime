namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic
{
	public class BowlFillAnimator : ProgressAnimator<float, AnimatedFloat, BowlFillAnimator>
	{
		private readonly BowlFacade bowl;

		protected BowlFillAnimator(BowlFacade bowl)
		{
			this.bowl = bowl;
		}

		protected override float GetStartValue()
		{
			return bowl.Fill;
		}

		protected override void Animate()
		{
			bowl.Fill = ReferenceValue;
		}
	}
}
