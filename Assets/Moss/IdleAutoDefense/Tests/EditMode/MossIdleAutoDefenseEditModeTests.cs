using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Deucarian.GameContentAuthoring.Editor;
using Deucarian.TemplateGameIdleAutoDefense;
using Deucarian.Attacks.Authoring;
using Deucarian.RunUpgrades.Authoring;
using Deucarian.WeaponSystems.Authoring;
using Moss.IdleAutoDefense;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Moss.IdleAutoDefense.Tests
{
    public sealed class MossIdleAutoDefenseEditModeTests
    {
        private const string ScenePath = "Assets/Moss/IdleAutoDefense/Scenes/MossIdleAutoDefense.unity";
        private const string ContentRoot = "Assets/GameContent/MossOnTheKeyboard";
        private const string StarterPackPath = ContentRoot + "/ContentPacks/contentpack.moss-on-the-keyboard.starter/contentpack.moss-on-the-keyboard.starter_ContentPack.asset";
        private const string StarterSetPath = ContentRoot + "/ContentSets/contentset.moss-on-the-keyboard.starter-run/contentset.moss-on-the-keyboard.starter-run_GameContentSet.asset";

        [Test]
        public void BootstrapUsesTemplateController()
        {
            Assert.IsTrue(typeof(IdleAutoDefenseTemplateController).IsAssignableFrom(typeof(MossIdleAutoDefenseGameBootstrap)));
        }

        [Test]
        public void TemplateSaveProgressionSmokeStillPasses()
        {
            IdleAutoDefenseTemplateCompositionSmokeResult result = IdleAutoDefenseTemplateSaveProgressionComposition.RunSmoke();
            Assert.IsTrue(result.Succeeded);
        }

        [Test]
        public void MossSaveResetDeletesDevSave()
        {
            MossIdleAutoDefenseSave.Reset();
            Directory.CreateDirectory(MossIdleAutoDefenseSave.SaveDirectoryPath);
            File.WriteAllText(MossIdleAutoDefenseSave.SaveFilePath, "{}");

            Assert.IsTrue(MossIdleAutoDefenseSave.HasSave);
            Assert.IsTrue(MossIdleAutoDefenseSave.Reset());
            Assert.IsFalse(MossIdleAutoDefenseSave.HasSave);
        }

        [Test]
        public void SmokeSceneAndStarterContentAreProjectOwned()
        {
            Assert.IsTrue(File.Exists(ScenePath));
            Assert.IsTrue(File.Exists("Assets/Moss/IdleAutoDefense/Content/idle-auto-defense-starter.json"));
            Assert.IsTrue(File.Exists(StarterPackPath));
            Assert.IsTrue(File.Exists(StarterSetPath));
        }

        [Test]
        public void EditorMenuPathsUseDeucarianToolsRoot()
        {
            foreach (string script in Directory.GetFiles("Assets", "*.cs", SearchOption.AllDirectories))
            {
                string source = File.ReadAllText(script);
                foreach (Match match in Regex.Matches(source, "\\[MenuItem\\(\\s*\"([^\"]+)\""))
                {
                    string menuPath = match.Groups[1].Value;
                    Assert.IsTrue(menuPath.StartsWith("Tools/Deucarian/", StringComparison.Ordinal), script + " uses invalid menu path: " + menuPath);
                    Assert.IsFalse(menuPath.StartsWith("Deucarian/", StringComparison.Ordinal), script + " creates a top-level Deucarian menu: " + menuPath);
                    Assert.IsFalse(menuPath.StartsWith("Moss/", StringComparison.Ordinal) || menuPath.StartsWith("Tools/Moss/", StringComparison.Ordinal), script + " creates a Moss-root menu: " + menuPath);
                }
            }
        }

        [Test]
        public void MossStarterContentPackIsReadyAndConnected()
        {
            GameContentPackAsset pack = AssetDatabase.LoadAssetAtPath<GameContentPackAsset>(StarterPackPath);
            Assert.IsNotNull(pack);
            Assert.AreEqual("contentpack.moss-on-the-keyboard.starter", pack.Id);

            GameContentPackValidationReport report = GameContentPackValidator.Validate(pack);
            Assert.IsTrue(report.IsValid, FormatPackIssues(report));
            Assert.AreEqual(0, report.WarningCount, FormatPackIssues(report));

            GameContentPackDependencySummary dependencies = GameContentPackValidator.CollectDependencies(pack);
            Assert.AreEqual(1, dependencies.ContentSetCount);
            Assert.AreEqual(4, dependencies.AttackCount);
            Assert.AreEqual(4, dependencies.EnemyCount);
            Assert.AreEqual(6, dependencies.WaveCount);
            Assert.AreEqual(4, dependencies.WeaponCount);
            Assert.AreEqual(6, dependencies.UpgradeCount);
        }

        [Test]
        public void MossStarterRoundHasCompletePlaceholderPresentation()
        {
            GameContentSetAsset set = AssetDatabase.LoadAssetAtPath<GameContentSetAsset>(StarterSetPath);
            Assert.IsNotNull(set);
            Assert.IsNotNull(set.Icon);
            Assert.IsNotNull(set.Banner);
            Assert.AreEqual(4, set.AvailableWeapons.Count);
            Assert.AreEqual(4, set.EnemyPool.Count);
            Assert.AreEqual(6, set.WaveSet.Count);
            Assert.AreEqual(6, set.UpgradePool.Count);

            var attackModes = new HashSet<AttackRecipeDeliveryMode>();
            for (int i = 0; i < set.AvailableWeapons.Count; i++)
            {
                WeaponDefinitionAsset weapon = set.AvailableWeapons[i];
                Assert.IsNotNull(weapon, "Weapon entry " + i + " is missing.");
                Assert.IsNotNull(weapon.Icon, weapon.Id + " should have an icon.");
                Assert.IsNotNull(weapon.Presentation, weapon.Id + " should have presentation.");
                Assert.IsNotNull(weapon.Presentation.Prefab, weapon.Id + " should have a placeholder prefab.");
                Assert.IsNotNull(weapon.Presentation.PlacementAudio, weapon.Id + " should have placement audio.");
                Assert.IsNotNull(weapon.Presentation.PlacementVfxPrefab, weapon.Id + " should have placement VFX.");
                Assert.IsNotNull(weapon.Stats, weapon.Id + " should have stats.");
                Assert.IsNotNull(weapon.Stats.Attack, weapon.Id + " should reference an authored attack.");
                Assert.IsNotNull(weapon.Stats.Attack.Icon, weapon.Stats.Attack.Id + " should have an icon.");
                Assert.IsNotNull(weapon.Stats.Attack.Presentation, weapon.Stats.Attack.Id + " should have presentation.");
                Assert.That(weapon.Stats.Attack.Presentation.Events, Has.Some.Matches<AttackPresentationEventRecipe>(evt => evt != null && (evt.AudioClip != null || evt.VfxPrefab != null)));
                attackModes.Add(weapon.Stats.Attack.Delivery.Mode);
            }

            Assert.That(attackModes, Does.Contain(AttackRecipeDeliveryMode.Projectile));
            Assert.That(attackModes, Does.Contain(AttackRecipeDeliveryMode.Hitscan));
            Assert.That(attackModes, Does.Contain(AttackRecipeDeliveryMode.Aura));

            for (int i = 0; i < set.EnemyPool.Count; i++)
            {
                EnemyDefinitionAsset enemy = set.EnemyPool[i];
                Assert.IsNotNull(enemy, "Enemy entry " + i + " is missing.");
                Assert.IsNotNull(enemy.Icon, enemy.Id + " should have an icon.");
                Assert.IsNotNull(enemy.Presentation, enemy.Id + " should have presentation.");
                Assert.IsNotNull(enemy.Presentation.Prefab, enemy.Id + " should have a placeholder prefab.");
                Assert.That(enemy.Presentation.Events, Has.Some.Matches<EnemyPresentationEventRecipe>(evt => evt != null && (evt.AudioClip != null || evt.VfxPrefab != null)));
            }

            for (int i = 0; i < set.UpgradePool.Count; i++)
            {
                RunUpgradeDefinitionAsset upgrade = set.UpgradePool[i];
                Assert.IsNotNull(upgrade, "Upgrade entry " + i + " is missing.");
                Assert.IsNotNull(upgrade.Icon, upgrade.Id + " should have an icon.");
                Assert.IsNotNull(upgrade.Economy, upgrade.Id + " should have economy.");
                Assert.IsNotNull(upgrade.Effects, upgrade.Id + " should have effects.");
            }
        }

        [Test]
        public void ContentLibraryMarksMossStarterPackReady()
        {
            GameContentLibraryReport report = GameContentLibraryService.Scan("Assets/GameContent");

            Assert.AreEqual(0, report.BlockerCount, FormatLibraryIssues(report));
            Assert.AreEqual(0, report.WarningCount, FormatLibraryIssues(report));
            Assert.That(report.Items.Select(item => item.Id), Does.Contain("contentpack.moss-on-the-keyboard.starter"));
            Assert.That(report.Items.Select(item => item.Id), Does.Contain("contentset.moss-on-the-keyboard.starter-run"));
            Assert.AreEqual(1, report.ReadyContentPackCount);
            Assert.AreEqual(1, report.ReadyContentSetCount);
        }

        [Test]
        public void GameContentAuthoringProvidersAreAvailable()
        {
            string[] providerNames = GameContentAuthoringProviderRegistry.Providers
                .Select(provider => provider.DisplayName)
                .ToArray();

            Assert.That(providerNames, Does.Contain("Attack"));
            Assert.That(providerNames, Does.Contain("Enemy"));
            Assert.That(providerNames, Does.Contain("Wave"));
            Assert.That(providerNames, Does.Contain("Tower / Weapon"));
            Assert.That(providerNames, Does.Contain("Upgrade"));
            Assert.That(providerNames, Does.Contain("Game / Run Content Set"));
            Assert.That(providerNames, Does.Contain("Content Library"));
            Assert.That(providerNames, Does.Contain("Content Pack"));
        }

        [Test]
        public void SmokeSceneAssignsMossStarterContentPack()
        {
            GameContentPackAsset pack = AssetDatabase.LoadAssetAtPath<GameContentPackAsset>(StarterPackPath);
            GameContentSetAsset set = AssetDatabase.LoadAssetAtPath<GameContentSetAsset>(StarterSetPath);
            UnityEngine.SceneManagement.Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            MossIdleAutoDefenseGameBootstrap controller = UnityEngine.Object.FindFirstObjectByType<MossIdleAutoDefenseGameBootstrap>();

            Assert.IsNotNull(controller);
            var serialized = new SerializedObject(controller);
            Assert.AreSame(pack, serialized.FindProperty("_contentPack").objectReferenceValue);
            Assert.AreSame(set, serialized.FindProperty("_contentSet").objectReferenceValue);
            Assert.IsFalse(scene.isDirty, "Opening the smoke scene for validation should not dirty it.");
        }

        [Test]
        public void AssignedMossStarterPackRunsWithoutFallback()
        {
            GameContentPackAsset pack = AssetDatabase.LoadAssetAtPath<GameContentPackAsset>(StarterPackPath);
            GameContentSetAsset set = AssetDatabase.LoadAssetAtPath<GameContentSetAsset>(StarterSetPath);
            GameObject host = new GameObject("moss-authored-content-smoke");
            host.SetActive(false);
            var controller = host.AddComponent<MossIdleAutoDefenseGameBootstrap>();
            SetTemplateContent(controller, pack, set);
            host.SetActive(true);
            controller.RestartRun();
            UnityEngine.SceneManagement.Scene activeScene = EditorSceneManager.GetActiveScene();

            try
            {
                for (int i = 0; i < 80; i++)
                    controller.Step(1, 0.05f);

                Assert.IsTrue(controller.UsingAssignedContentPack, controller.AssignedContentPackStatus);
                Assert.IsTrue(controller.UsingAssignedContentSet, controller.AssignedContentSetStatus);
                Assert.AreEqual(0, controller.InvalidAssignedContentPackIssueCount);
                Assert.AreEqual(0, controller.InvalidAssignedContentSetIssueCount);
                Assert.That(
                    controller.SpawnedCount,
                    Is.GreaterThan(0),
                    "Expected authored waves to spawn. Selected upgrades: " + controller.SelectedUpgradeCount
                    + ", spawn delay ticks: " + controller.EnemySpawnDelayTicks
                    + ", unsupported upgrade intents: " + controller.UnsupportedUpgradeIntentCount
                    + ", runtime state: " + controller.RuntimeStateName);
                Assert.AreEqual("Running", controller.RuntimeStateName, controller.StatusSummary);
                Assert.IsFalse(activeScene.isDirty, "Authored content runtime smoke should not dirty the active scene.");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(host);
            }
        }

        [Test]
        public void TemplateFlowDocsAndMossOverrideContentAreProjectOwned()
        {
            AssertFileContains("Assets/Moss/IdleAutoDefense/Docs/template-game-flow.md", "Documentation~/canonical-game-flow.md");
            AssertFileContains("Assets/Moss/IdleAutoDefense/Docs/default-content-overrides.md", "Overrides/Enemies/moss-enemies.json");
            AssertFileContains("Assets/Moss/IdleAutoDefense/Content/moss-content-overrides.json", "overrideStatus");
            AssertFileContains("Assets/Moss/IdleAutoDefense/Content/Overrides/Stages/moss-stages.json", "stage.moss.boss-pulse");
            AssertFileContains("Assets/Moss/IdleAutoDefense/Content/Overrides/Enemies/moss-enemies.json", "enemy.moss.boss");
            AssertFileContains("Assets/Moss/IdleAutoDefense/Content/Overrides/Weapons/moss-weapons.json", "weapon.moss.shard-launcher");
            AssertFileContains("Assets/Moss/IdleAutoDefense/Content/Overrides/Waves/moss-waves.json", "encounter.moss.boss-pulse");
            AssertFileContains("Assets/Moss/IdleAutoDefense/Content/Overrides/Upgrades/moss-upgrades.json", "upgrade.moss.projectile-specialization");
            AssertFileContains("Assets/Moss/IdleAutoDefense/Content/Overrides/Progression/moss-progression.json", "moss-idle-auto-defense-profile");
            AssertFileContains("Assets/Moss/IdleAutoDefense/Content/Monetization/moss-monetization-overrides.json", "moss.rewarded.double-offline-reward");
            AssertFileContains("Assets/Moss/IdleAutoDefense/Docs/monetization-overrides.md", "no real ad SDKs");
        }

        [Test]
        public void MossOverrideStructureMirrorsTemplateDefaults()
        {
            AssertFileContains("Assets/Moss/IdleAutoDefense/Content/moss-content-overrides.json", "Overrides/Stages/moss-stages.json");
            AssertFileContains("Assets/Moss/IdleAutoDefense/Content/idle-auto-defense-starter.json", "stage.moss.first-orbit");
            AssertFileContains("Assets/Moss/IdleAutoDefense/Content/idle-auto-defense-starter.json", "weapon.moss.arc-emitter");

            string mossStages = File.ReadAllText("Assets/Moss/IdleAutoDefense/Content/Overrides/Stages/moss-stages.json");
            AssertContainsAll(mossStages, "stage.template.first-orbit", "stage.template.pressure-ring", "stage.template.boss-pulse", "stage.template.endless-placeholder");

            string mossWeapons = File.ReadAllText("Assets/Moss/IdleAutoDefense/Content/Overrides/Weapons/moss-weapons.json");
            AssertContainsAll(mossWeapons, "weapon.template.pulse-cannon", "weapon.template.shard-launcher", "weapon.template.arc-emitter", "weapon.template.orbital-shot");

            string mossUpgrades = File.ReadAllText("Assets/Moss/IdleAutoDefense/Content/Overrides/Upgrades/moss-upgrades.json");
            AssertContainsAll(
                mossUpgrades,
                "upgrade.template.damage-up",
                "upgrade.template.fire-rate-up",
                "upgrade.template.projectile-count-up",
                "upgrade.template.projectile-speed-up",
                "upgrade.template.objective-max-health-up",
                "upgrade.template.objective-repair",
                "upgrade.template.shield-restore-intent",
                "upgrade.template.enemy-reward-up",
                "upgrade.template.offline-gain-up",
                "upgrade.template.reroll-bonus",
                "upgrade.template.crit-chance-intent",
                "upgrade.template.crit-damage-intent",
                "upgrade.template.direct-specialization",
                "upgrade.template.projectile-specialization");
        }

        [Test]
        public void MossMockMonetizationConfigLoads()
        {
            string json = File.ReadAllText("Assets/Moss/IdleAutoDefense/Content/Monetization/moss-monetization-overrides.json");
            MossMonetizationConfig config = JsonUtility.FromJson<MossMonetizationConfig>(json);

            Assert.AreEqual("mock", config.provider);
            Assert.IsFalse(config.realSdksIncluded);
            AssertContainsPlacement("moss.rewarded.double-run-reward", config.rewardedPlacements);
            AssertContainsPlacement("moss.rewarded.reroll-upgrade-draft", config.rewardedPlacements);
            AssertContainsPlacement("moss.rewarded.revive-after-failure", config.rewardedPlacements);
            AssertContainsPlacement("moss.rewarded.double-offline-reward", config.rewardedPlacements);
            AssertContainsPlacement("moss.interstitial.after-run-completion", config.interstitialPlacements);
            AssertContainsPlaceholder("moss.iap.no-forced-ads", config.iapPlaceholders);
        }

        [Test]
        public void MossScriptsDoNotCopyTemplateNamespace()
        {
            foreach (string script in Directory.GetFiles("Assets/Moss/IdleAutoDefense/Scripts", "*.cs", SearchOption.AllDirectories))
            {
                string source = File.ReadAllText(script);
                StringAssert.DoesNotContain("namespace Deucarian.TemplateGameIdleAutoDefense", source);
                StringAssert.DoesNotContain("namespace Deucarian.TemplateGameIdleAutoDefense.Samples", source);
            }
        }

        private static void SetTemplateContent(IdleAutoDefenseTemplateController controller, GameContentPackAsset pack, GameContentSetAsset set)
        {
            FieldInfo packField = typeof(IdleAutoDefenseTemplateController).GetField("_contentPack", BindingFlags.Instance | BindingFlags.NonPublic);
            FieldInfo setField = typeof(IdleAutoDefenseTemplateController).GetField("_contentSet", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(packField);
            Assert.IsNotNull(setField);
            packField.SetValue(controller, pack);
            setField.SetValue(controller, set);
        }

        private static void AssertFileContains(string path, string expected)
        {
            Assert.IsTrue(File.Exists(path), "Expected file to exist: " + path);
            StringAssert.Contains(expected, File.ReadAllText(path));
        }

        private static void AssertContainsAll(string actual, params string[] expected)
        {
            for (int i = 0; i < expected.Length; i++)
                StringAssert.Contains(expected[i], actual);
        }

        private static void AssertContainsPlacement(string expectedId, MossMonetizationPlacement[] placements)
        {
            Assert.IsNotNull(placements);
            for (int i = 0; i < placements.Length; i++)
            {
                if (placements[i].id == expectedId)
                    return;
            }

            Assert.Fail("Expected placement to exist: " + expectedId);
        }

        private static void AssertContainsPlaceholder(string expectedId, MossPurchasePlaceholder[] placeholders)
        {
            Assert.IsNotNull(placeholders);
            for (int i = 0; i < placeholders.Length; i++)
            {
                if (placeholders[i].id == expectedId)
                    return;
            }

            Assert.Fail("Expected placeholder to exist: " + expectedId);
        }

        private static string FormatPackIssues(GameContentPackValidationReport report)
        {
            return string.Join(Environment.NewLine, report.Issues.Select(issue => issue.Path + ": " + issue.Message));
        }

        private static string FormatLibraryIssues(GameContentLibraryReport report)
        {
            return string.Join(Environment.NewLine, report.AllIssues.Select(issue => issue.Path + ": " + issue.Message));
        }

        [Serializable]
        private sealed class MossMonetizationConfig
        {
            public string provider;
            public bool realSdksIncluded;
            public MossMonetizationPlacement[] rewardedPlacements;
            public MossMonetizationPlacement[] interstitialPlacements;
            public MossPurchasePlaceholder[] iapPlaceholders;
        }

        [Serializable]
        private sealed class MossMonetizationPlacement
        {
            public string id;
            public string sourceTemplateId;
            public string productDecision;
            public string cooldownGroup;
            public int cooldownSeconds;
            public int sessionCap;
            public bool transitionOnly;
            public bool blockedDuringCombat;
            public bool blockedBeforeFirstTerminalRun;
            public bool blockedByNoAdsEntitlement;
        }

        [Serializable]
        private sealed class MossPurchasePlaceholder
        {
            public string id;
            public string sourceTemplateId;
            public string productDecision;
        }
    }
}
