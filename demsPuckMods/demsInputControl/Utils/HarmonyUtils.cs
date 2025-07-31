using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demsInputControl.Utils
{
    public static class HarmonyUtils
    {
        public static PlayerBodyV2 GetPlayerBody(PlayerInput input)
        {
            var playerProp = AccessTools.Property(input.GetType(), "Player");
            var playerObj = playerProp?.GetValue(input);
            if (playerObj == null)
                return null;

            var bodyProp = AccessTools.Property(playerObj.GetType(), "PlayerBody");
            var playerBody = bodyProp?.GetValue(playerObj) as PlayerBodyV2;
            return playerBody;
        }
    }
}
