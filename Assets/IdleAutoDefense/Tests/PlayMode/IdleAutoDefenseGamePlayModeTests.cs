using System;
using System.Collections;
using Deucarian.IdleProgression;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace IdleAutoDefenseGame.PlayModeTests
{
    public sealed class IdleAutoDefenseGamePlayModeTests
    {
        [UnityTest]
        public IEnumerator BasicBootstrapRunsDeterministicTemplateSmoke()
        {
            GameObject host = new GameObject("idle-auto-defense-game-smoke");
            var controller = host.AddComponent<BasicIdleAutoDefenseGameBootstrap>();
            controller.enabled = false;

            for (int i = 0; i < 720; i++)
            {
                controller.Step(1, 0.05f);
                if (controller.EncounterCompleted || controller.EncounterFailed)
                    break;
                if (i % 30 == 0) yield return null;
            }

            Assert.That(controller.SpawnedCount, Is.GreaterThanOrEqualTo(4));
            Assert.That(controller.ProjectileLaunchCount, Is.GreaterThan(0));
            Assert.That(controller.DirectOrCombatKillCount + controller.ProjectileAdapterKillCount, Is.GreaterThan(0));
            Assert.That(controller.SelectedUpgradeCount, Is.GreaterThan(0));
            Assert.True(controller.EncounterCompleted, "The starter run should complete in deterministic smoke. " + controller.StatusSummary);

            controller.SimulateOfflineReward(DateTimeOffset.UnixEpoch, DateTimeOffset.UnixEpoch.AddHours(1));
            Assert.AreEqual(IdleProgressionResultCode.Success, controller.LastOfflineRewardCode);
            Assert.That(controller.OfflineRewardCredits, Is.GreaterThanOrEqualTo(900));
            Assert.That(controller.OfflineRewardParts, Is.GreaterThanOrEqualTo(12));

            BasicIdleAutoDefenseSave.WriteSnapshot("playmode-smoke", controller);
            Assert.IsTrue(BasicIdleAutoDefenseSave.HasSave);
            Assert.IsTrue(BasicIdleAutoDefenseSave.Reset());

            UnityEngine.Object.Destroy(host);
        }

        [UnityTest]
        public IEnumerator BasicSampleSceneUsesAssignedPackWithoutFallback()
        {
            SceneManager.LoadScene("BasicIdleAutoDefense", LoadSceneMode.Single);
            yield return null;

            BasicIdleAutoDefenseGameBootstrap controller = UnityEngine.Object.FindFirstObjectByType<BasicIdleAutoDefenseGameBootstrap>();
            Assert.IsNotNull(controller);
            controller.enabled = false;

            Scene scene = SceneManager.GetActiveScene();
            Assert.IsFalse(scene.isDirty, "Loading the imported sample scene for validation should not dirty it.");

            for (int i = 0; i < 80; i++)
                controller.Step(1, 0.05f);

            Assert.IsTrue(controller.UsingAssignedContentPack, controller.AssignedContentPackStatus);
            Assert.IsTrue(controller.UsingAssignedContentSet, controller.AssignedContentSetStatus);
            Assert.AreEqual(0, controller.InvalidAssignedContentPackIssueCount);
            Assert.AreEqual(0, controller.InvalidAssignedContentSetIssueCount);
            Assert.That(controller.SpawnedCount, Is.GreaterThan(0), controller.StatusSummary);
            Assert.AreEqual("Running", controller.RuntimeStateName, controller.StatusSummary);
            Assert.IsFalse(scene.isDirty, "Imported sample scene play smoke should not dirty the scene.");

            BasicIdleAutoDefenseSave.Reset();
        }
    }
}
