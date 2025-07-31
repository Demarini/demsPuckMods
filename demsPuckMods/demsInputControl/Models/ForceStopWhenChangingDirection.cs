using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demsInputControl.Models
{
    public class ForceStopWhenChangingDirection
    {
        public bool ForceStopWhenForwardsToBackwards { get; set; } = false;
        public bool ForceStopWhenBackwardsToForwards { get; set; } = false;
    }
}
