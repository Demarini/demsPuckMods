using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demsInputControl.Models
{
    public class ModifySprintControlModel
    {
        public bool AllowModifySprintControl { get; set; } = true;
        public bool AllowSprintWhileMovingBackwards { get; set; } = true;
        public float MinimumSpeedToSprint { get; set; } = 0f;
    }
}