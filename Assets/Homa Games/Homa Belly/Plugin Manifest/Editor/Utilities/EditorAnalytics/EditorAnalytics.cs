using System.Diagnostics;
using System.Threading.Tasks;
using HomaGames.HomaBelly.Utilities;
using Debug = UnityEngine.Debug;

namespace HomaGames.HomaBelly
{
    /// <summary>
    /// Class to send Unity Editor Analytics to Homa Games servers.
    /// </summary>
    public static class EditorAnalytics
    {

        /// <summary>
        /// Track a Unity Editor Event with eventName and other optional values
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="eventDescription"></param>
        /// <param name="eventStackTrace"></param>
        /// <param name="eventValue"></param>
        /// <param name="eventFps"></param>
        [Conditional("HOMA_BELLY_EDITOR_ANALYTICS_ENABLED")]
        public static async void TrackEditorAnalyticsEvent(string eventName, string eventDescription = null, string eventStackTrace = null, float eventValue = 0, float eventFps = 0)
        {
#if HOMA_BELLY_EDITOR_ANALYTICS_ENABLED
            if (string.IsNullOrWhiteSpace(eventName))
            {
                Debug.LogError($"[Editor Analytics] Tracking empty event name");
                return;
            }

            EditorHttpCaller<EditorAnalyticsResponseModel> editorHttpCaller = new EditorHttpCaller<EditorAnalyticsResponseModel>();

            string editorEventUrl = string.Format(EventApiQueryModel.EVENT_API_ENDPOINT);
            EditorAnalyticsEventModel eventModel = new EditorAnalyticsEventModel(eventName, eventDescription, eventStackTrace, eventValue, eventFps);

            #if HOMA_BELLY_DEV_ENV
            Debug.Log($"[Editor Analytics] Tracking: {editorEventUrl}. With body: {eventModel}");
            #endif
            
            try
            {
                EditorAnalyticsResponseModel responseModel = await editorHttpCaller.Post(editorEventUrl, eventModel.ToDictionary(), new EditorAnalyticsModelDeserializer());
                if (responseModel != null)
                {
                    #if HOMA_BELLY_DEV_ENV
                    Debug.Log($"[Editor Analytics] Response: {responseModel}");
                    #endif
                }
            }
            catch (EditorHttpCallerException e)
            {
                #if HOMA_BELLY_DEV_ENV
                Debug.LogError($"[Editor Analytics] Error while sending the event. Reason: {e.Message}");
                #endif
            }
#endif
        }
    }
}
