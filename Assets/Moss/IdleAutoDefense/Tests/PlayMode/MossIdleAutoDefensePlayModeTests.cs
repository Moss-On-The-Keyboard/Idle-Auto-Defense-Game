using System;
using System.Collections;
using Deucarian.IdleProgression;
using Moss.IdleAutoDefense;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Moss.IdleAutoDefense.PlayModeTests
{
    public sealed class MossIdleAutoDefensePlayModeTests
    {
        [UnityTest]
        public IEnumerator MossBootstrapRunsDeterministicTemplateSmoke()
        {
            GameObject host = new GameObject("moss-idle-auto-defense-smoke");
            var controller = host.AddComponent<MossIdleAutoDefenseGameBootstrap>();
            controller.enabled = false;

            for (int i = 0; i < 240; i++)
            {
                controller.Step(1, 0.05f);
                if (i % 30 == 0) yield return null;
            }

            Assert.That(controller.SpawnedCount, Is.GreaterThanOrEqualTo(4));
            Assert.That(controller.ProjectileLaunchCount, Is.GreaterThan(0));
            Assert.That(controller.DirectOrCombatKillCount + controller.ProjectileAdapterKillCount, Is.GreaterThan(0));
            Assert.That(controller.SelectedUpgradeCount, Is.GreaterThan(0));
            Assert.True(controller.EncounterCompleted || controller.EncounterFailed, "Moss smoke run should reach a terminal state.");

            controller.SimulateOfflineReward(DateTimeOffset.UnixEpoch, DateTimeOffset.UnixEpoch.AddHours(1));
            Assert.AreEqual(IdleProgressionResultCode.Success, controller.LastOfflineRewardCode);
            Assert.That(controller.OfflineRewardCredits, Is.GreaterThanOrEqualTo(900));
            Assert.That(controller.OfflineRewardParts, Is.GreaterThanOrEqualTo(12));

            Assert.That(controller.EncounterRewardCredits, Is.GreaterThanOrEqualTo(25));
            Assert.That(controller.EncounterRewardParts, Is.GreaterThanOrEqualTo(1));

            MossIdleAutoDefenseSave.WriteSnapshot("playmode-smoke", controller);
            Assert.IsTrue(MossIdleAutoDefenseSave.HasSave);
            Assert.IsTrue(MossIdleAutoDefenseSave.Reset());

            UnityEngine.Object.Destroy(host);
        }
    }
}
