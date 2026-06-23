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
    }
}
