using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Deucarian.GameContentAuthoring.Editor;
using Deucarian.TemplateGameIdleAutoDefense;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace IdleAutoDefenseGame.Tests
{
    public sealed class IdleAutoDefenseGameEditModeTests
    {
        private const string ScenePath = "Assets/IdleAutoDefense/Scenes/BasicIdleAutoDefense.unity";
        private const string ContentRoot = "Assets/GameContent/IdleAutoDefense";
        private const string PackPath = ContentRoot + "/ContentPacks/contentpack.template.basic-idle-auto-defense/contentpack.template.basic-idle-auto-defense_ContentPack.asset";
        private const string SetPath = ContentRoot + "/ContentSets/contentset.template.basic-idle-auto-defense/contentset.template.basic-idle-auto-defense_GameContentSet.asset";

        [Test]
        public void BootstrapUsesTemplateController()
        {
            Assert.IsTrue(typeof(IdleAutoDefenseTemplateController).IsAssignableFrom(typeof(BasicIdleAutoDefenseGameBootstrap)));
        }

        [Test]
        public void TemplateSaveProgressionSmokePasses()
        {
            IdleAutoDefenseTemplateCompositionSmokeResult result = IdleAutoDefenseTemplateSaveProgressionComposition.RunSmoke();
            Assert.IsTrue(result.Succeeded);
        }

        [Test]
        public void ProjectSaveResetDeletesDevSave()
        {
            BasicIdleAutoDefenseSave.Reset();
            Directory.CreateDirectory(BasicIdleAutoDefenseSave.SaveDirectoryPath);
            File.WriteAllText(BasicIdleAutoDefenseSave.SaveFilePath, "{}");

            Assert.IsTrue(BasicIdleAutoDefenseSave.HasSave);
            Assert.IsTrue(BasicIdleAutoDefenseSave.Reset());
            Assert.IsFalse(BasicIdleAutoDefenseSave.HasSave);
        }

        [Test]
        public void BuildSettingsUseOnlyCanonicalScene()
        {
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            Assert.AreEqual(1, scenes.Length);
            Assert.IsTrue(scenes[0].enabled);
            Assert.AreEqual(ScenePath, scenes[0].path);
        }

        [Test]
        public void ImportedSceneAndAuthoredGameContentArePresent()
        {
            Assert.IsTrue(File.Exists(ScenePath));
            Assert.IsTrue(File.Exists(PackPath));
            Assert.IsTrue(File.Exists(SetPath));
            Assert.IsTrue(AssetDatabase.IsValidFolder(ContentRoot));
            Assert.IsTrue(Directory.Exists("Assets/IdleAutoDefense/Prefabs"));
            Assert.IsTrue(Directory.Exists("Assets/IdleAutoDefense/Visuals"));
            Assert.IsFalse(Directory.Exists("Assets/IdleAutoDefense/Content"));
            Assert.IsFalse(Directory.Exists("Assets/Games"));
            Assert.IsFalse(Directory.Exists("Assets/Samples"));
        }

        [Test]
        public void ImportedContentPackAndSetValidate()
        {
            GameContentPackAsset pack = AssetDatabase.LoadAssetAtPath<GameContentPackAsset>(PackPath);
            GameContentSetAsset set = AssetDatabase.LoadAssetAtPath<GameContentSetAsset>(SetPath);

            Assert.IsNotNull(pack);
            Assert.IsNotNull(set);
            Assert.AreEqual("contentpack.template.basic-idle-auto-defense", pack.Id);
            Assert.AreEqual("contentset.template.basic-idle-auto-defense", set.Id);

            GameContentPackValidationReport packReport = GameContentPackValidator.Validate(pack);
            GameContentSetValidationReport setReport = GameContentSetValidator.Validate(set);
            Assert.IsTrue(packReport.IsValid, FormatPackIssues(packReport));
            Assert.IsTrue(setReport.IsValid, FormatSetIssues(setReport));

            GameContentPackDependencySummary dependencies = GameContentPackValidator.CollectDependencies(pack);
            Assert.AreEqual(1, dependencies.ContentSetCount);
            Assert.AreEqual(4, dependencies.AttackCount);
            Assert.AreEqual(4, dependencies.EnemyCount);
            Assert.AreEqual(5, dependencies.WaveCount);
            Assert.AreEqual(4, dependencies.WeaponCount);
            Assert.AreEqual(6, dependencies.UpgradeCount);
        }

        [Test]
        public void GameContentAuthoringLibrarySeesReadyAuthoredContent()
        {
            GameContentLibraryReport report = GameContentLibraryService.Scan("Assets/GameContent");

            Assert.AreEqual(0, report.BlockerCount, FormatLibraryIssues(report));
            Assert.AreEqual(1, report.ReadyContentPackCount, FormatLibraryIssues(report));
            Assert.AreEqual(1, report.ReadyContentSetCount, FormatLibraryIssues(report));
            Assert.AreEqual(4, CountKind(report, GameContentLibraryKind.Attack));
            Assert.AreEqual(4, CountKind(report, GameContentLibraryKind.Enemy));
            Assert.AreEqual(5, CountKind(report, GameContentLibraryKind.Wave));
            Assert.AreEqual(4, CountKind(report, GameContentLibraryKind.Weapon));
            Assert.AreEqual(6, CountKind(report, GameContentLibraryKind.Upgrade));
            Assert.AreEqual(1, CountKind(report, GameContentLibraryKind.ContentSet));
            Assert.AreEqual(1, CountKind(report, GameContentLibraryKind.ContentPack));

            GameContentLibraryContentSetSummary setSummary = report.ContentSetSummaries.Single();
            Assert.IsTrue(setSummary.Ready, setSummary.Message);
            Assert.AreEqual(4, setSummary.WeaponCount);
            Assert.AreEqual(4, setSummary.EnemyCount);
            Assert.AreEqual(5, setSummary.WaveCount);
            Assert.AreEqual(6, setSummary.UpgradeCount);

            GameContentLibraryContentPackSummary packSummary = report.ContentPackSummaries.Single();
            Assert.IsTrue(packSummary.Ready, packSummary.Message);
            Assert.AreEqual(1, packSummary.ContentSetCount);
            Assert.AreEqual(4, packSummary.WeaponCount);
            Assert.AreEqual(4, packSummary.EnemyCount);
            Assert.AreEqual(5, packSummary.WaveCount);
            Assert.AreEqual(6, packSummary.UpgradeCount);
        }

        [Test]
        public void SmokeSceneAssignsImportedTemplateContent()
        {
            GameContentPackAsset pack = AssetDatabase.LoadAssetAtPath<GameContentPackAsset>(PackPath);
            GameContentSetAsset set = AssetDatabase.LoadAssetAtPath<GameContentSetAsset>(SetPath);
            UnityEngine.SceneManagement.Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            BasicIdleAutoDefenseGameBootstrap controller = UnityEngine.Object.FindFirstObjectByType<BasicIdleAutoDefenseGameBootstrap>();

            Assert.IsNotNull(controller);
            var serialized = new SerializedObject(controller);
            Assert.AreSame(pack, serialized.FindProperty("_contentPack").objectReferenceValue);
            Assert.AreSame(set, serialized.FindProperty("_contentSet").objectReferenceValue);
            Assert.IsFalse(scene.isDirty, "Opening the imported scene for validation should not dirty it.");
        }

        [Test]
        public void AssignedImportedTemplateContentRunsWithoutFallback()
        {
            GameContentPackAsset pack = AssetDatabase.LoadAssetAtPath<GameContentPackAsset>(PackPath);
            GameContentSetAsset set = AssetDatabase.LoadAssetAtPath<GameContentSetAsset>(SetPath);
            GameObject host = new GameObject("idle-auto-defense-authored-content-smoke");
            host.SetActive(false);
            var controller = host.AddComponent<BasicIdleAutoDefenseGameBootstrap>();
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
                Assert.That(controller.SpawnedCount, Is.GreaterThan(0), controller.StatusSummary);
                Assert.AreEqual("Running", controller.RuntimeStateName, controller.StatusSummary);
                Assert.IsFalse(activeScene.isDirty, "Runtime smoke should not dirty the active scene.");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(host);
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

        private static string FormatPackIssues(GameContentPackValidationReport report)
        {
            return string.Join(Environment.NewLine, report.Issues.Select(issue => issue.Path + ": " + issue.Message));
        }

        private static string FormatSetIssues(GameContentSetValidationReport report)
        {
            return string.Join(Environment.NewLine, report.Issues.Select(issue => issue.Path + ": " + issue.Message));
        }

        private static int CountKind(GameContentLibraryReport report, GameContentLibraryKind kind)
        {
            return report.Items.Count(item => item.Kind == kind);
        }

        private static string FormatLibraryIssues(GameContentLibraryReport report)
        {
            return string.Join(Environment.NewLine, report.AllIssues.Select(issue => issue.Path + ": " + issue.Message));
        }
    }
}
