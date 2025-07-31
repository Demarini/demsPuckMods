using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demsInputControl.Models
{
    public class ModifySprintControlModel
    {
        public float MinimumSpeedToSprint { get; set; } = 0f;
        public bool StopInputWhenPlayerMovingBackwardsAndMovingForward { get; set; }//talk about a verbose name
    }
}