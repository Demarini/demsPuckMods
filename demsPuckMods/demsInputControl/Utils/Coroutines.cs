using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace demsInputControl.Utils
{
    public static class Coroutines
    {
        public static IEnumerator ResendMoveInput(PlayerInput input, short x, short y)
        {
            // Step 1: Small delay
            yield return null;

            // Step 2: Optional second-frame delay
            yield return null;

            // Step 3: Force a brief stop just in case
            input.Client_StopInputRpc(true);
            Debug.Log("[InputControl] Sending second forced stop (reset)");

            // Step 4: Wait before re-sending movement
            yield return new WaitForSeconds(0.01f);

            // Step 5: Re-apply movement
            input.Client_MoveInputRpc(x, y);
            Debug.Log("[InputControl] Re-sending forward input after auto-stop.");
        }
    }
}
