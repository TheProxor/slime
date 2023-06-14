using System;


namespace TheProxor.Services.Ads
{
	public class AdsService : IAdsService<AdType>
	{
		public bool IsTestModeActive { get; set; }
		public bool IsNoAdsActive { get; set; }



		public bool IsAdReady(AdType adType) => true;


		public bool TryShowAd(AdType adType, Action<bool> callback, string placement = default)
		{
			callback?.Invoke(true);
			return true;
		}


		public bool TryHideAd(AdType adType, Action<bool> callback = null)
		{
			callback?.Invoke(true);
			return true;
		}
	}
}
