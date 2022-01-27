using System;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace HomaGames.HomaBelly
{
    /// <summary>
    /// Class holding:<br />
    /// 
    /// - IDFV, IDFA and DeviceId for iOS<br/>
    /// - GAID and DeviceId Android<br/><br/>
    ///
    /// Public properties within this class guarantee the value at anytime. They fetched automatically
    /// on runtime and are updated when they are susceptible to change.
    /// </summary>
    public static class Identifiers
    {
        private const string ADVERTISING_ID_EMPTY = "00000000-0000-0000-0000-000000000000";
        public static string Idfv { get; private set; } = ADVERTISING_ID_EMPTY;
        public static string Idfa { get; private set; } = ADVERTISING_ID_EMPTY;
        public static string Gaid { get; private set; } = ADVERTISING_ID_EMPTY;
        public static string DeviceId { get; private set; } = SystemInfo.deviceUniqueIdentifier;

        [RuntimeInitializeOnLoadMethod]
        public static void Initialize()
        {
            FetchAdvertisingIdentifier();
            
#if UNITY_IOS
            Idfv = Device.vendorIdentifier;
#endif
        }

        public static async void FetchAdvertisingIdentifier()
        {
#if UNITY_EDITOR
            Idfv = "editor_idfv";
            Idfa = "editor_idfa";
            Gaid = "editor_gaid";
#elif UNITY_IOS
            if (Idfa == ADVERTISING_ID_EMPTY)
            {

                try
                {
                    // This method was removed on Unity 2020+ for Android
                    Application.RequestAdvertisingIdentifierAsync((string advertisingId, bool trackingEnabled, string error) =>
                        {
                            if (!string.IsNullOrEmpty(error) && !string.IsNullOrWhiteSpace(advertisingId))
                            {
                                Idfa = advertisingId;
                            }
                        }
                    );
                }
                catch (Exception e)
                {
                    HomaGamesLog.Warning($"[Identifiers] Exception while fetching IDFA: {e.Message}");
                }
            }
#elif UNITY_ANDROID

            if (Gaid == ADVERTISING_ID_EMPTY)
            {
                try
                {
                    await Task.Run(delegate
                    {
                        // Attach thread
                        AndroidJNI.AttachCurrentThread();
                        
                        // Fetch GAID from native
                        var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                        var context = player.GetStatic<AndroidJavaObject>("currentActivity");
                        var advertisingIdClientJavaClass =
                            new AndroidJavaClass("com.google.android.gms.ads.identifier.AdvertisingIdClient");
                        var advertisingIdInfoJavaObject =
                            advertisingIdClientJavaClass.CallStatic<AndroidJavaObject>("getAdvertisingIdInfo", context);
                        Gaid = advertisingIdInfoJavaObject.Call<string>("getId");
                        
                        // Detach thread
                        AndroidJNI.DetachCurrentThread();
                        
                    }).ContinueWith((result) =>
                    {
                        if (result.IsFaulted && result.Exception != null)
                        {
                            foreach (var e in result.Exception.InnerExceptions) {
                                HomaGamesLog.Warning($"[Identifiers] Exception while fetching GAID: {e.Message}");
                            }    
                        }
                        else
                        {
                            HomaGamesLog.Debug($"[Identifiers] GAID: {Gaid}");    
                        }
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
                catch (AggregateException ae)
                {
                    foreach (var e in ae.InnerExceptions) {
                        HomaGamesLog.Warning($"[Identifiers] Exception while fetching GAID: {e.Message}");
                    }
                }
                catch (Exception e)
                {
                    HomaGamesLog.Warning($"[Identifiers] Exception while fetching GAID: {e.Message}");
                }
            }
#endif
        }
    }
}
