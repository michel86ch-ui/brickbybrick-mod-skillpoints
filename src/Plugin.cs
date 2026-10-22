using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using MultiplayerTemplate.Progression;
using UnityEngine;

namespace SkillPoints
{
    /// <summary>
    /// Adds skill points on a key press, in any world the player hosts, through the game's own
    /// <see cref="ProgressionService.GrantCurrencyForDevelopment"/>. The game then marks the run as practice
    /// (no Steam leaderboard score), as it does for its own development grants.
    /// </summary>
    [BepInPlugin(Guid, "SkillPoints", Version)]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string Guid = "michel.brickbybrick.skillpoints";
        public const string Version = "1.0.0";
        private const string Currency = "skill-point";

        internal static ManualLogSource Log;
        internal static ProgressionService Service;
        private static ConfigEntry<KeyboardShortcut> key;
        private static ConfigEntry<int> amount;

        private static string toast;
        private static float toastUntil;

        private void Awake()
        {
            Log = Logger;
            key = Config.Bind("General", "Key", new KeyboardShortcut(KeyCode.F10),
                "Adds skill points in the world you host. Once used, the world no longer counts for the Steam leaderboard.");
            amount = Config.Bind("General", "Amount", 10,
                new ConfigDescription("Skill points per key press.", new AcceptableValueRange<int>(1, 1000)));
            new Harmony(Guid).PatchAll(typeof(Plugin).Assembly);
            Log.LogInfo($"SkillPoints: {key.Value} adds {amount.Value} skill points.");
        }

        private void Update()
        {
            if (!key.Value.IsDown())
            {
                return;
            }
            if (Service == null)
            {
                Toast("Skill points: load a world first.");
                return;
            }
            ProgressionCommandResult result = Service.GrantCurrencyForDevelopment(Currency, amount.Value);
            if (result.Succeeded)
            {
                Log.LogInfo($"+{amount.Value} skill points, now {Service.GetCurrency(Currency)}.");
                Toast($"+{amount.Value} skill points (now {Service.GetCurrency(Currency)})");
            }
            else if (result.Status == ProgressionCommandStatus.NotAuthoritative)
            {
                Toast("Skill points: only in a world you host.");
            }
            else
            {
                Log.LogWarning($"Skill points not granted: {result.Status} {result.Message}");
                Toast("Skill points not granted: " + result.Message);
            }
        }

        private static void Toast(string text, float seconds = 4f)
        {
            toast = text;
            toastUntil = Time.unscaledTime + seconds;
        }

        private void OnGUI()
        {
            if (toast == null || Time.unscaledTime > toastUntil)
            {
                return;
            }
            var style = new GUIStyle(GUI.skin.box) { fontSize = 22, alignment = TextAnchor.MiddleCenter, wordWrap = true };
            GUI.Box(new Rect(Screen.width / 2f - 300f, 60f, 600f, 50f), toast, style);
        }
    }

    /// <summary>Remembers the game's progression service (one per world session).</summary>
    [HarmonyPatch(typeof(ProgressionService), MethodType.Constructor, typeof(ProgressionDefinitionCatalog), typeof(IProgressionAuthority))]
    internal static class ProgressionServicePatch
    {
        private static void Postfix(ProgressionService __instance)
        {
            Plugin.Service = __instance;
        }
    }
}
