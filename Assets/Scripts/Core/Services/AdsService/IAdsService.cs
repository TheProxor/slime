using System;


namespace TheProxor.Services.Ads
{
	public interface IAdsService<TAdType> where TAdType : Enum
	{
		bool IsTestModeActive { get; set; }

		bool IsNoAdsActive { get; set; }


		bool IsAdReady(TAdType adType);

		bool TryShowAd(TAdType adType, Action<bool> callback, string placement = default);

		bool TryHideAd(TAdType adType, Action<bool> callback = null);
	}
}
