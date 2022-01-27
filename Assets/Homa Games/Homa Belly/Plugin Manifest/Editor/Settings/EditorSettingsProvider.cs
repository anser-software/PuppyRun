using UnityEditor;
using UnityEngine;

namespace HomaGames.HomaBelly
{
    public class EditorSettingsProvider : ISettingsProvider
    {
        private const string EDITOR_ANALYTICS_ENABLED_ON_FIRST_INSTALL_PREF_KEY =
            "homagames.editor_analytics_enabled_on_first_install";

        [InitializeOnLoadMethod]
        static void RegisterSettings()
        {
            Settings.RegisterSettings(new EditorSettingsProvider());
            EnsureEditorAnalyticsEnabledOnce();
        }

        public string Name => "Unity Editor";
        public string Version => "";
        public int Order => 2;

        public void Draw()
        {
            DefineSymbolsUtility.HomaBellyEditorAnalyticsEnabled = EditorGUILayout.Toggle(new GUIContent("Editor Analytics", "Enable Unity Editor Analytics to be sent to Homa Games to improve its products"),
                DefineSymbolsUtility.HomaBellyEditorAnalyticsEnabled);
        }
        
        private static void EnsureEditorAnalyticsEnabledOnce()
        {
            var enabledOnce = EditorPrefs.GetInt(EDITOR_ANALYTICS_ENABLED_ON_FIRST_INSTALL_PREF_KEY, 0) == 1;
            if (!enabledOnce)
            {
                DefineSymbolsUtility.HomaBellyEditorAnalyticsEnabled = true;
                EditorPrefs.SetInt(EDITOR_ANALYTICS_ENABLED_ON_FIRST_INSTALL_PREF_KEY, 1);
            }
        }
    }
}