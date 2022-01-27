using UnityEditor;
using UnityEngine;

namespace HomaGames.HomaBelly
{
    public class GlobalSettingsProvider : ISettingsProvider
    {
        private const string DEVELOPMENT_MODE_ENABLED_ON_FIRST_INSTALL_PREF_KEY =
            "homagames.development_mode_enabled_on_first_install";
        private const string DEBUG_OVERRIDE_ENABLED_ON_FIRST_INSTALL_PREF_KEY =
            "homagames.debug_override_enabled_on_first_install";

        [InitializeOnLoadMethod]
        static void RegisterSettings()
        {
            Settings.RegisterSettings(new GlobalSettingsProvider());
            EnsureDevelopmentModeEnabledOnce();
            EnsureDebugOverrideEnabledOnce();
        }

        public string Name => "Global";
        public string Version => "";
        public int Order => 1;

        public void Draw()
        {
            DefineSymbolsUtility.DevelopmentModeEnabled =
                EditorGUILayout.Toggle(new GUIContent("Development Mode","Development Mode will enable Unity Logs. Disable it for release builds to benefit from multiple Homa Belly optimizations"), 
                    DefineSymbolsUtility.DevelopmentModeEnabled);
            DefineSymbolsUtility.HomaBellyDebugClassOverrideEnabled = EditorGUILayout.Toggle(new GUIContent("Debug Override", "Homa Belly by default does override Unity's Debug class for optimization reasons. Disabling it will affect performance on mobile devices"), 
                DefineSymbolsUtility.HomaBellyDebugClassOverrideEnabled);
        }

        private static void EnsureDevelopmentModeEnabledOnce()
        {
            var enabledOnce = EditorPrefs.GetInt(DEVELOPMENT_MODE_ENABLED_ON_FIRST_INSTALL_PREF_KEY, 0) == 1;
            if (!enabledOnce)
            {
                DefineSymbolsUtility.DevelopmentModeEnabled = true;
                EditorPrefs.SetInt(DEVELOPMENT_MODE_ENABLED_ON_FIRST_INSTALL_PREF_KEY, 1);
            }
        }

        private static void EnsureDebugOverrideEnabledOnce()
        {
            var enabledOnce = EditorPrefs.GetInt(DEBUG_OVERRIDE_ENABLED_ON_FIRST_INSTALL_PREF_KEY, 0) == 1;
            if (!enabledOnce)
            {
                DefineSymbolsUtility.HomaBellyDebugClassOverrideEnabled = true;
                EditorPrefs.SetInt(DEBUG_OVERRIDE_ENABLED_ON_FIRST_INSTALL_PREF_KEY, 1);
            }
        }
    }
}