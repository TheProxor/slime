using System;
using System.Collections.Generic;
using System.Linq;
using TheProxor.Services.Preference;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.SaveSystem
{
	public partial class SlimeSaver
	{
		public event Action<SlimeSaveData> OnNewSlimeSaved;
		public event Action<SlimeSaveData> OnSlimeRemoved;

		private const string SAVE_KEY = "SlimeSystemSave";

		private readonly IPreferenceService preferenceService;

		private SaveValue Save
		{
			get => preferenceService.LoadValue(SAVE_KEY, new SaveValue());
			set => preferenceService.SaveValue(SAVE_KEY, value);
		}

		public SlimeSaver(IPreferenceService preferenceService)
		{
			this.preferenceService = preferenceService;
		}

		public void AddSlimeSave(SlimeSaveData saveData)
		{
			SaveValue save = Save;
			save.AddSave(saveData);
			Save = save;
			OnNewSlimeSaved?.Invoke(Save.GetSave().Last());
		}

		public List<SlimeSaveData> GetAllSlimes() =>
			Save.GetSave();

		public void RemoveSlimeSave(SlimeSaveData slimeSaveData)
		{
			SaveValue saveValue = Save;
			if (!saveValue.HasId(slimeSaveData, out int removeIndex))
			{
				return;
			}

			saveValue.RemoveSaveAt(removeIndex);
			Save = saveValue;

			OnSlimeRemoved?.Invoke(slimeSaveData);
		}


		public void ForceSaveAll() =>
			preferenceService.SaveValue(Save);
	}
}
