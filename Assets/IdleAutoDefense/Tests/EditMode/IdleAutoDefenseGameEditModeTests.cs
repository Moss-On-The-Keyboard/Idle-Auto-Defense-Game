using System;
using System.IO;
using System.Linq;
using System.Reflection;
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
        private const string ContentRoot = "Assets/IdleAutoDefense/Content";
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
        public void ImportedSceneAndTemplateContentArePresent()
        {
            Assert.IsTrue(File.Exists(ScenePath));
            Assert.IsTrue(File.Exists(PackPath));
            Assert.IsTrue(File.Exists(SetPath));
            Assert.IsTrue(Directory.Exists("Assets/IdleAutoDefense/Prefabs"));
            Assert.IsTrue(Directory.Exists("Assets/IdleAutoDefense/Visuals"));
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
            Assert.That(dependencies.AttackCount, Is.GreaterThanOrEqualTo(2));
            Assert.That(dependencies.EnemyCount, Is.GreaterThanOrEqualTo(6));
            Assert.That(dependencies.WaveCount, Is.GreaterThanOrEqualTo(2));
            Assert.That(dependencies.WeaponCount, Is.GreaterThanOrEqualTo(2));
            Assert.That(dependencies.UpgradeCount, Is.GreaterThanOrEqualTo(4));
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
    }
}
