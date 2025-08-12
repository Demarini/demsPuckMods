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
            BotSpawning.SpawnFakePlayer(0, PlayerRole.Goalie, PlayerTeam.Blue);
            yield return new WaitForSeconds(.1f);
            BotSpawning.SpawnFakePlayer(1, PlayerRole.Goalie, PlayerTeam.Red);
            
            
            //Debug.Log($"[BotSpawning] Fake registry contains {FakePlayerRegistry.All.Count()} bots");

            GoaliesAreRunning = true;
        }
        public static IEnumerator StartGoalieSessionRed()
        {
            yield return new WaitForSeconds(.1f);
            BotSpawning.SpawnFakePlayer(1, PlayerRole.Goalie, PlayerTeam.Red);


            //Debug.Log($"[BotSpawning] Fake registry contains {FakePlayerRegistry.All.Count()} bots");

            GoaliesAreRunning = true;
        }
        public static IEnumerator StartGoalieSessionBlue()
        {
            yield return new WaitForSeconds(.1f);
            BotSpawning.SpawnFakePlayer(0, PlayerRole.Goalie, PlayerTeam.Blue);


            //Debug.Log($"[BotSpawning] Fake registry contains {FakePlayerRegistry.All.Count()} bots");

           GoaliesAreRunning = true;
        }
        public static void StartGoalieSessionViaCoroutine(GoalieSession session)
        {
            switch (session)
            {
                case GoalieSession.Blue:
                    GoalieRunner.Instance.StartCoroutine(StartGoalieSessionBlue());
                    break;
                case GoalieSession.Red:
                    GoalieRunner.Instance.StartCoroutine(StartGoalieSessionRed());
                    break;
                case GoalieSession.Both:
                    GoalieRunner.Instance.StartCoroutine(StartGoalieSession());
                    break;
            }
        }
        public static void EndGoalieSession(GoalieSession type)
        {
            //Debug.Log("End Goalie Session");
            BotSpawning.DespawnBots(type);
            GoaliesAreRunning = false;
        }
    }
    public enum GoalieSession
    {
        Red,
        Blue,
        Both
    }
}