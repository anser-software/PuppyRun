using System;
using System.Collections.Generic;
using System.Globalization;
using HomaGames.HomaBelly.Utilities;
using UnityEngine;

namespace HomaGames.HomaBelly
{
    /// <summary>
    /// Base class for all Homa Games API queries: editor and runtime.
    ///
    /// This class holds and configures mandatory or common parameters for Homa Games API
    /// </summary>
    public abstract class ApiQueryModel
    {
        private static readonly string PARAM_CV = "cv";
        private static readonly string PARAM_SV = "sv";
        private static readonly string PARAM_AV = "av";
        private static readonly string PARAM_AI = "ai";
        private static readonly string PARAM_TI = "ti";
        private static readonly string PARAM_UA = "ua";
        private static readonly string PARAM_MVI = "mvi";
        // Device OS
        private static readonly string PARAM_DOS = "dos";
        // Device Type
        private static readonly string PARAM_DTP = "dtp";
        // Device version (name)
        private static readonly string PARAM_DV = "dv";
        // IDFA
        private static readonly string PARAM_IDFA = "idfa";
        // IDFV
        private static readonly string PARAM_IDFV = "idfv";
        // GAID
        private static readonly string PARAM_GAID = "gaid";

        // TokenIdentifier and ManifestVersionId are statically stored and used across all events
        // TODO: For runtime analytic events, read these values from TrackingData
        public static string TokenIdentifier = "t0000001";
        public static string ManifestVersionId;

        // Frequently changed from event to event. Is made protected
        // to be overwritten when necessary
        protected string UserAgent = HomaGames.HomaBelly.UserAgent.GetUserAgent();

        // Dictionary instance to be reused for all events, avoiding new object creation
        private static Dictionary<string, object> _asDictionary = new Dictionary<string, object>();
        
        public override string ToString()
        {
            return Json.Serialize(ToDictionary());
        }
        
        protected double Sanitize(float value)
        {
            return Convert.ToDouble(value, CultureInfo.InvariantCulture);
        }

        protected string Sanitize(string input)
        {
            // '=': causes server error 500
            // ' ': causes Unity Editor Console window to go crazy
            // '\n': affects readability and formatting for API communication
            return !string.IsNullOrWhiteSpace(input) ? input.Replace("=", "").Replace("\n", "").Replace(" ","") : "";
        }

        protected Dictionary<string, object> ToDictionary()
        {
            // Clear any possible previous values
            _asDictionary.Clear();
            
            _asDictionary.Add(PARAM_CV, HomaBellyConstants.API_VERSION);
            _asDictionary.Add(PARAM_SV, HomaBellyConstants.PRODUCT_VERSION);
            _asDictionary.Add(PARAM_AV, Application.version);
            _asDictionary.Add(PARAM_AI, Application.identifier);
            _asDictionary.Add(PARAM_DOS, SystemInfo.operatingSystem);
            _asDictionary.Add(PARAM_DTP, SystemInfo.deviceType);
            _asDictionary.Add(PARAM_DV, SystemInfo.deviceModel);
            _asDictionary.Add(PARAM_TI, TokenIdentifier);
            _asDictionary.Add(PARAM_UA, UserAgent);
            _asDictionary.Add(PARAM_MVI, ManifestVersionId);
            
            /*
             TODO: Move to Runtime Api Query Model. Identifiers only apply on devices and make
             TODO: sense to be in Core instead of the installer
            _asDictionary.Add(PARAM_IDFA, Identifiers.Idfa);
            _asDictionary.Add(PARAM_IDFV, Identifiers.Idfv);
            _asDictionary.Add(PARAM_GAID, Identifiers.Gaid);
            */

            return _asDictionary;
        }
    }
}