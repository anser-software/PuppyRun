using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HomaGames.HomaBelly
{
    public static class DefineSymbolsUtility
    {
        private const string HOMA_DEVELOPMENT = "HOMA_DEVELOPMENT";
        private const string HOMA_BELLY_DEBUG_CLASS_OVERRIDE_ENABLED = "HOMA_BELLY_DEBUG_CLASS_OVERRIDE_ENABLED";
        private const string HOMA_BELLY_EDITOR_ANALYTICS_ENABLED = "HOMA_BELLY_EDITOR_ANALYTICS_ENABLED";

        public static bool DevelopmentModeEnabled
        {
            get => ContainsDefineSymbolForBothPlatforms(HOMA_DEVELOPMENT);
            set => SetDefineSymbolForAllPlatforms(HOMA_DEVELOPMENT,value);
        }
        
        public static bool HomaBellyDebugClassOverrideEnabled
        {
            get => ContainsDefineSymbolForBothPlatforms(HOMA_BELLY_DEBUG_CLASS_OVERRIDE_ENABLED);
            set => SetDefineSymbolForAllPlatforms(HOMA_BELLY_DEBUG_CLASS_OVERRIDE_ENABLED,value);
        }
        
        public static bool HomaBellyEditorAnalyticsEnabled
        {
            get => ContainsDefineSymbolForBothPlatforms(HOMA_BELLY_EDITOR_ANALYTICS_ENABLED);
            set => SetDefineSymbolForAllPlatforms(HOMA_BELLY_EDITOR_ANALYTICS_ENABLED, value);
        }

        private static bool ContainsDefineSymbolForBothPlatforms(string symbol)
        {
            return ContainsDefineSymbol(BuildTargetGroup.Android, symbol) &&
                   ContainsDefineSymbol(BuildTargetGroup.iOS, symbol);
        }
        
        private static bool ContainsDefineSymbol(BuildTargetGroup buildTargetGroup, string symbol)
        {
            var symbols = new List<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup).Split(';'));
            return symbols.Contains(symbol);
        }

        private static void SetDefineSymbolForAllPlatforms(string symbol, bool enabled)
        {
            SetDefineSymbol(BuildTargetGroup.Android, symbol, enabled);
            SetDefineSymbol(BuildTargetGroup.iOS, symbol, enabled);
        }
        
        private static void SetDefineSymbol(BuildTargetGroup buildTargetGroup, string symbol, bool enabled)
        {
            var symbols = new List<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup).Split(';'));
            var alreadySet = symbols.Contains(symbol);
            bool changed = false;
            if (enabled && !alreadySet)
            {
                symbols.Add(symbol);
                changed = true;
            }
            else if(!enabled && alreadySet)
            {
                symbols.Remove(symbol);
                changed = true;
            }

            if (changed)
            {
                var builtSymbols = String.Join(";", symbols);
                if (builtSymbols.StartsWith(";"))
                    builtSymbols = builtSymbols.Remove(0, 1);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup,builtSymbols);
                AssetDatabase.Refresh();
            }
        }
    }
}