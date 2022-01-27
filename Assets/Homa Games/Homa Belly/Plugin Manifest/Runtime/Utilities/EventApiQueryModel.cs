using System;
using System.Collections.Generic;
using System.Linq;
using HomaGames.HomaBelly.Utilities;

namespace HomaGames.HomaBelly
{
    /// <summary>
    /// Base class for all Homa Analytics API queries: editor and runtime.
    /// </summary>
    public abstract class EventApiQueryModel : ApiQueryModel
    {
        public static readonly string EVENT_API_ENDPOINT = HomaBellyConstants.API_HOST + "/appevent";

        // Install id
        private static readonly string PARAM_IID = "iid";
        // Session id
        private static readonly string PARAM_SID = "sid";
        // Event unique id
        private static readonly string PARAM_EID = "eid";
        // N-testing id
        private static readonly string PARAM_NTESTING_ID = "ntesting_id";
        // N-testing override id
        private static readonly string PARAM_NTESTING_OID = "ntesting_oid";
        // Event client timestamp
        private static readonly string PARAM_ECT = "ect";
        // Event client timezone
        private static readonly string PARAM_ECTZ = "ectz";
        // Event category
        private static readonly string PARAM_EVC = "evc";
        // Event name
        private static readonly string PARAM_EVN = "evn";
        // Event value (JSON)
        private static readonly string PARAM_EVV = "evv";

        // Frequently changed from event to event
        protected string EventCategory;
        protected string EventName;
        protected string InstallId;
        protected string SessionId;
        protected Dictionary<string, object> EventValue = new Dictionary<string, object>();

        public override string ToString()
        {
            return Json.Serialize(ToDictionary());
        }

        public new Dictionary<string, object> ToDictionary()
        {
            // Set base dictionary values
            Dictionary<string, object> dictionary = base.ToDictionary();
            
            // Set specific dictionary values
            dictionary.Add(PARAM_IID, InstallId);
            dictionary.Add(PARAM_SID, SessionId);
            dictionary.Add(PARAM_EID, Guid.NewGuid());
            dictionary.Add(PARAM_NTESTING_ID, "" /* TODO for runtime analytics */);
            dictionary.Add(PARAM_NTESTING_OID, "" /* TODO for runtime analytics */);
            dictionary.Add(PARAM_ECT, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            dictionary.Add(PARAM_ECTZ, TimeZoneInfo.Local.StandardName);
            dictionary.Add(PARAM_EVN, Sanitize(EventName));
            dictionary.Add(PARAM_EVC, Sanitize(EventCategory));
            dictionary.Add(PARAM_EVV, EventValue);
            
            return dictionary;
        }
    }
}