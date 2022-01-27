#if HOMA_BELLY_EDITOR_ANALYTICS_ENABLED
using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace HomaGames.HomaBelly
{
    /// <summary>
    /// Specific API model for Unity Editor Analytics events
    /// </summary>
    public class EditorAnalyticsEventModel : EventApiQueryModel
    {
        private static readonly string PARAM_ED = "description";
        private static readonly string PARAM_UNITY_VERSION = "unity_version";
        private static readonly string PARAM_UNITY_TARGET = "unity_target";
        private static readonly string PARAM_EST = "stack_trace";
        private static readonly string PARAM_EV = "value";
        private static readonly string PARAM_EFPS = "fps";

        public EditorAnalyticsEventModel(string eventName, string eventDescription, string eventStackTrace, float eventValue, float eventFps)
        {
            EventCategory = "unity_editor_event";
            InstallId = EditorAnalyticsSessionInfo.userId;
            SessionId = EditorAnalyticsSessionInfo.id.ToString(CultureInfo.InvariantCulture);
            EventName = eventName;
            EventValue.Add(PARAM_ED, Sanitize(eventDescription));
            EventValue.Add(PARAM_EST, Sanitize(eventStackTrace));
            EventValue.Add(PARAM_EV, Sanitize(eventValue));
            EventValue.Add(PARAM_EFPS, Sanitize(eventFps));
            EventValue.Add(PARAM_UNITY_VERSION, Application.unityVersion);
            
#if UNITY_ANDROID
            EventValue.Add(PARAM_UNITY_TARGET, "Android");
#elif UNITY_IOS
            EventValue.Add(PARAM_UNITY_TARGET, "iOS");
#else
            EventValue.Add(PARAM_UNITY_TARGET, "Other");
#endif
        }
    }
}
#endif