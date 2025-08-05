using PuckAIPractice.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuckAIPractice.GameModes
{
    public static class Goalies
    {
        public static bool GoaliesAreRunning = false;
        public static void StartGoalieSession()
        {
            BotSpawning.SpawnFakePlayer(1, PlayerRole.Goalie, PlayerTeam.Red);
            BotSpawning.SpawnFakePlayer(0, PlayerRole.Goalie, PlayerTeam.Blue);
            
            GoaliesAreRunning = true;
        }
        public static void EndGoalieSession()
        {
            BotSpawning.DespawnBots();
            GoaliesAreRunning = false;
        }
    }
}
