using TheProxor.Services.Audio;
using UnityEngine;


namespace TheProxor.Database
{
	[CreateAssetMenu(order = 0, fileName = nameof(SoundDatabase), menuName = "Database/" + nameof(SoundDatabase))]
	public class SoundDatabase : Database<SoundId, SoundDataInfo> {}
}
