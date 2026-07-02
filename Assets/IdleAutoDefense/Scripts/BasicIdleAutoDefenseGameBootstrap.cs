using System;
using System.Globalization;
using System.IO;
using Deucarian.TemplateGameIdleAutoDefense;
using UnityEngine;

namespace IdleAutoDefenseGame
{
    public sealed class BasicIdleAutoDefenseGameBootstrap : IdleAutoDefenseTemplateController
    {
        private const float HudWidth = 360f;
        private string _lastSaveMessage = "No snapshot saved";

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(12f, 12f, HudWidth, 500f), GUI.skin.box);
            GUILayout.Label("Idle Auto Defense");
            GUILayout.Label("State: " + RuntimeStateName);
            GUILayout.Label("Tower HP: " + ObjectiveHealthText + "  Lives: " + ObjectiveLivesRemaining);
            GUILayout.Label("Runtime Credits: " + RuntimeCurrency);
            GUILayout.Label("Progression Credits: " + EncounterRewardCredits + "  Parts: " + EncounterRewardParts);
            GUILayout.Label("Time: " + SurvivalSeconds.ToString("0", CultureInfo.InvariantCulture) + "s");
            GUILayout.Label("Spawn Profile: " + CurrentSpawnProfileName);
            GUILayout.Label("Enemies: " + ActiveEnemyCount + " active / " + SpawnedCount + " spawned");
            GUILayout.Label("Kills: " + (DirectOrCombatKillCount + ProjectileAdapterKillCount) + "  Projectiles: " + ProjectileLaunchCount);
            GUILayout.Label("Purchases: " + SelectedUpgradeCount + "  Modules: " + UnlockedModuleCount + "/4  Objective Hits: " + ObjectiveDamageEvents);
            GUILayout.Space(4f);
            GUILayout.Label("Upgrades");
            DrawUpgradeButton("Damage", DamageUpgradeRank, DamageUpgradeCost, CanPurchaseDamageUpgrade, TryPurchaseDamageUpgrade);
            DrawUpgradeButton("Attack Speed", AttackSpeedUpgradeRank, AttackSpeedUpgradeCost, CanPurchaseAttackSpeedUpgrade, TryPurchaseAttackSpeedUpgrade);
            DrawUpgradeButton("Range", RangeUpgradeRank, RangeUpgradeCost, CanPurchaseRangeUpgrade, TryPurchaseRangeUpgrade);
            DrawUpgradeButton("Repair / Max HP", RepairUpgradeRank, RepairUpgradeCost, CanPurchaseRepairUpgrade, TryPurchaseRepairUpgrade);
            GUILayout.Space(4f);
            GUILayout.Label("Modules");
            DrawModuleButton("Pulse Beam", PulseBeamUnlocked, PulseBeamUnlockCost, CanPurchasePulseBeamModule, TryPurchasePulseBeamModule);
            DrawModuleButton("Arc Burst", ArcBurstUnlocked, ArcBurstUnlockCost, CanPurchaseArcBurstModule, TryPurchaseArcBurstModule);
            DrawModuleButton("Homing Pulse", HomingPulseUnlocked, HomingPulseUnlockCost, CanPurchaseHomingPulseModule, TryPurchaseHomingPulseModule);
            GUILayout.Space(4f);
            GUILayout.Label("Save: " + _lastSaveMessage + (BasicIdleAutoDefenseSave.HasSave ? " (file present)" : string.Empty));
            if (GUILayout.Button("Save Snapshot")) WriteSampleSnapshot("manual");
            if (GUILayout.Button("Reset Save")) ResetSampleSave();
            GUILayout.Space(4f);
            if (EncounterCompleted) GUILayout.Label("Run complete");
            else if (EncounterFailed) GUILayout.Label("Tower destroyed");
            if ((EncounterCompleted || EncounterFailed) && GUILayout.Button("Restart Run")) RestartRun();
            GUILayout.EndArea();
        }

        private void WriteSampleSnapshot(string reason)
        {
            BasicIdleAutoDefenseSave.WriteSnapshot(reason, this);
            _lastSaveMessage = "Saved " + DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
        }

        private void ResetSampleSave()
        {
            bool deleted = BasicIdleAutoDefenseSave.Reset();
            _lastSaveMessage = deleted ? "reset complete" : "nothing to reset";
            Debug.Log("[Idle Auto Defense] " + _lastSaveMessage + ".");
        }

        private static void DrawUpgradeButton(string label, int rank, int cost, bool enabled, Func<bool> purchase)
        {
            bool wasEnabled = GUI.enabled;
            GUI.enabled = enabled;
            if (GUILayout.Button(label + "  Lv " + rank.ToString(CultureInfo.InvariantCulture) + "  " + cost.ToString(CultureInfo.InvariantCulture)))
                purchase?.Invoke();
            GUI.enabled = wasEnabled;
        }

        private static void DrawModuleButton(string label, bool unlocked, int cost, bool enabled, Func<bool> purchase)
        {
            bool wasEnabled = GUI.enabled;
            GUI.enabled = enabled;
            string state = unlocked ? "Unlocked" : cost.ToString(CultureInfo.InvariantCulture);
            if (GUILayout.Button(label + "  " + state))
                purchase?.Invoke();
            GUI.enabled = wasEnabled;
        }
    }

    public static class BasicIdleAutoDefenseSave
    {
        private const string SampleFolderName = "BasicIdleAutoDefense";
        private const string SampleFileName = "sample-state.json";

        public static string SaveDirectoryPath => Path.Combine(Application.persistentDataPath, "Deucarian", SampleFolderName);
        public static string SaveFilePath => Path.Combine(SaveDirectoryPath, SampleFileName);
        public static bool HasSave => File.Exists(SaveFilePath);

        public static void WriteSnapshot(string reason, IdleAutoDefenseTemplateController controller)
        {
            if (controller == null) throw new ArgumentNullException(nameof(controller));
            Directory.CreateDirectory(SaveDirectoryPath);
            File.WriteAllText(SaveFilePath, CreateSnapshotJson(reason, controller));
        }

        public static bool Reset()
        {
            bool existed = Directory.Exists(SaveDirectoryPath);
            if (existed)
                Directory.Delete(SaveDirectoryPath, true);
            return existed;
        }

        private static string CreateSnapshotJson(string reason, IdleAutoDefenseTemplateController controller)
        {
            return "{\n" +
                   "  \"reason\": \"" + Escape(reason) + "\",\n" +
                   "  \"savedUtc\": \"" + DateTimeOffset.UtcNow.ToString("O") + "\",\n" +
                   "  \"runtimeState\": \"" + controller.RuntimeStateName + "\",\n" +
                   "  \"spawned\": " + controller.SpawnedCount + ",\n" +
                   "  \"kills\": " + (controller.DirectOrCombatKillCount + controller.ProjectileAdapterKillCount) + ",\n" +
                   "  \"projectileLaunches\": " + controller.ProjectileLaunchCount + ",\n" +
                   "  \"selectedUpgrades\": " + controller.SelectedUpgradeCount + ",\n" +
                   "  \"objectiveHits\": " + controller.ObjectiveDamageEvents + "\n" +
                   "}\n";
        }

        private static string Escape(string value)
        {
            return (value ?? string.Empty).Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}
