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
        public IEnumerator BasicSampleSceneRunsDeterministicAuthoredContentSmoke()
        {
            SceneManager.LoadScene("BasicIdleAutoDefense", LoadSceneMode.Single);
            yield return null;

            BasicIdleAutoDefenseGameBootstrap controller = UnityEngine.Object.FindFirstObjectByType<BasicIdleAutoDefenseGameBootstrap>();
            Assert.IsNotNull(controller);
            controller.enabled = false;
            Assert.IsTrue(controller.UsingAssignedContentPack, controller.AssignedContentPackStatus);
            Assert.IsTrue(controller.UsingAssignedContentSet, controller.AssignedContentSetStatus);

            for (int i = 0; i < 1200; i++)
            {
                BuyAvailableLivePurchases(controller);
                controller.Step(1, 0.05f);
                if (controller.EncounterCompleted || controller.EncounterFailed)
                    break;
                if (i % 30 == 0) yield return null;
            }

            Assert.That(controller.SpawnedCount, Is.GreaterThanOrEqualTo(4));
            Assert.That(controller.ProjectileLaunchCount, Is.GreaterThan(0));
            Assert.That(controller.ProjectileVisualSpawnCount, Is.GreaterThan(0), controller.StatusSummary);
            Assert.That(controller.AttackVfxSpawnCount, Is.GreaterThan(0), controller.StatusSummary);
            Assert.That(controller.AttackAudioPlayCount, Is.GreaterThan(0), controller.StatusSummary);
            Assert.That(controller.EnemyPresentationEventCount, Is.GreaterThan(0), controller.StatusSummary);
            Assert.That(controller.DirectOrCombatKillCount + controller.ProjectileAdapterKillCount, Is.GreaterThan(0));
            Assert.That(controller.EncounterRewardCredits, Is.GreaterThan(0));
            Assert.That(controller.SelectedUpgradeCount, Is.GreaterThanOrEqualTo(4));
            Assert.That(controller.ModuleActivationCount, Is.GreaterThan(0));
            Assert.True(controller.PulseBeamUnlocked, "Smoke should unlock Pulse Beam.");
            Assert.True(controller.ArcBurstUnlocked, "Smoke should unlock Arc Burst.");
            Assert.True(controller.HomingPulseUnlocked, "Smoke should unlock Homing Pulse.");
            Assert.AreEqual(0, controller.DraftTickCount, "Sample upgrades should be explicit live purchases only.");
            Assert.True(controller.EncounterCompleted, "Assisted starter run should complete. " + controller.StatusSummary);

            controller.SimulateOfflineReward(DateTimeOffset.UnixEpoch, DateTimeOffset.UnixEpoch.AddHours(1));
            Assert.AreEqual(IdleProgressionResultCode.Success, controller.LastOfflineRewardCode);
            Assert.That(controller.OfflineRewardCredits, Is.GreaterThanOrEqualTo(900));
            Assert.That(controller.OfflineRewardParts, Is.GreaterThanOrEqualTo(12));

            BasicIdleAutoDefenseSave.WriteSnapshot("playmode-smoke", controller);
            Assert.IsTrue(BasicIdleAutoDefenseSave.HasSave);
            Assert.IsTrue(BasicIdleAutoDefenseSave.Reset());
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
            Assert.That(controller.ProjectileLaunchCount, Is.GreaterThan(0), controller.StatusSummary);
            Assert.That(controller.ProjectileVisualSpawnCount, Is.GreaterThan(0), controller.StatusSummary);
            Assert.That(controller.AttackVfxSpawnCount, Is.GreaterThan(0), controller.StatusSummary);
            Assert.That(controller.AttackAudioPlayCount, Is.GreaterThan(0), controller.StatusSummary);
            Assert.That(controller.EnemyPresentationEventCount, Is.GreaterThan(0), controller.StatusSummary);
            Assert.AreEqual("Running", controller.RuntimeStateName, controller.StatusSummary);
            Assert.IsFalse(scene.isDirty, "Imported sample scene play smoke should not dirty the scene.");

            BasicIdleAutoDefenseSave.Reset();
        }

        [UnityTest]
        public IEnumerator BasicSampleSceneCanFailWithoutLivePurchases()
        {
            SceneManager.LoadScene("BasicIdleAutoDefense", LoadSceneMode.Single);
            yield return null;

            BasicIdleAutoDefenseGameBootstrap controller = UnityEngine.Object.FindFirstObjectByType<BasicIdleAutoDefenseGameBootstrap>();
            Assert.IsNotNull(controller);
            controller.enabled = false;
            Assert.IsTrue(controller.UsingAssignedContentPack, controller.AssignedContentPackStatus);
            Assert.IsTrue(controller.UsingAssignedContentSet, controller.AssignedContentSetStatus);

            for (int i = 0; i < 1200; i++)
            {
                controller.Step(1, 0.05f);
                if (controller.EncounterCompleted || controller.EncounterFailed)
                    break;
                if (i % 30 == 0) yield return null;
            }

            Assert.AreEqual(0, controller.SelectedUpgradeCount);
            Assert.IsFalse(controller.PulseBeamUnlocked);
            Assert.That(controller.EnemyPresentationEventCount, Is.GreaterThan(0), controller.StatusSummary);
            Assert.That(controller.ObjectiveDamageEvents, Is.GreaterThan(0), controller.StatusSummary);
            Assert.True(controller.EncounterFailed, "No-upgrade sample run should be able to lose. " + controller.StatusSummary);

            BasicIdleAutoDefenseSave.Reset();
        }

        private static void BuyAvailableLivePurchases(BasicIdleAutoDefenseGameBootstrap controller)
        {
            if (controller.CanPurchasePulseBeamModule) controller.TryPurchasePulseBeamModule();
            if (controller.CanPurchaseDamageUpgrade) controller.TryPurchaseDamageUpgrade();
            if (controller.CanPurchaseAttackSpeedUpgrade) controller.TryPurchaseAttackSpeedUpgrade();
            if (controller.CanPurchaseArcBurstModule) controller.TryPurchaseArcBurstModule();
            if (controller.CanPurchaseRangeUpgrade) controller.TryPurchaseRangeUpgrade();
            if (controller.CanPurchaseHomingPulseModule) controller.TryPurchaseHomingPulseModule();
            if (controller.ObjectiveHealth < controller.ObjectiveMaximumHealth * 0.7d && controller.CanPurchaseRepairUpgrade)
                controller.TryPurchaseRepairUpgrade();
        }
    }
}
