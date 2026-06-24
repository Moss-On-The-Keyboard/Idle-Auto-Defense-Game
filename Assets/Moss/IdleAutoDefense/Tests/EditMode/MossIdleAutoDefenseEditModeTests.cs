using System;
using System.IO;
using Deucarian.TemplateGameIdleAutoDefense;
using Moss.IdleAutoDefense;
using NUnit.Framework;
using UnityEngine;

namespace Moss.IdleAutoDefense.Tests
{
    public sealed class MossIdleAutoDefenseEditModeTests
    {
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
            Assert.IsTrue(File.Exists("Assets/Moss/IdleAutoDefense/Scenes/MossIdleAutoDefense.unity"));
            Assert.IsTrue(File.Exists("Assets/Moss/IdleAutoDefense/Content/idle-auto-defense-starter.json"));
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
