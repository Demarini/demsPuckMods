using PuckAIPractice.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PuckAIPractice.GameModes
{
    public static class Goalies
    {
        public static bool GoaliesAreRunning = false;
        public static IEnumerator StartGoalieSession()
        {
            BotSpawning.SpawnFakePlayer(1, PlayerRole.Goalie, PlayerTeam.Red);
            yield return new WaitForSeconds(1f);
            BotSpawning.SpawnFakePlayer(0, PlayerRole.Goalie, PlayerTeam.Blue);
            //Debug.Log($"[BotSpawning] Fake registry contains {FakePlayerRegistry.All.Count()} bots");

            GoaliesAreRunning = true;
        }
        public static void StartGoalieSessionViaCoroutine()
        {
            GoalieRunner.Instance.StartCoroutine(StartGoalieSession());
        }
        public static void EndGoalieSession()
        {
            BotSpawning.DespawnBots();
            GoaliesAreRunning = false;
        }

    }
}
