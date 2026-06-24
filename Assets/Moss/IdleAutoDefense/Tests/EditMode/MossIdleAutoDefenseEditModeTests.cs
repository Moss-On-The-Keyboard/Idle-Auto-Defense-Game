using System.IO;
using Deucarian.TemplateGameIdleAutoDefense;
using Moss.IdleAutoDefense;
using NUnit.Framework;

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
            AssertFileContains("Assets/Moss/IdleAutoDefense/Content/Overrides/Enemies/moss-enemies.json", "enemy.moss.basic");
            AssertFileContains("Assets/Moss/IdleAutoDefense/Content/Overrides/Weapons/moss-weapons.json", "weapon.moss.projectile");
            AssertFileContains("Assets/Moss/IdleAutoDefense/Content/Overrides/Waves/moss-waves.json", "encounter.moss.basic");
            AssertFileContains("Assets/Moss/IdleAutoDefense/Content/Overrides/Upgrades/moss-upgrades.json", "upgrade.moss.direct.damage");
            AssertFileContains("Assets/Moss/IdleAutoDefense/Content/Overrides/Progression/moss-progression.json", "moss-idle-auto-defense-profile");
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
    }
}
