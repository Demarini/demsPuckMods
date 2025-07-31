using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demsInputControl.Models
{
    public class DelayInputsModel
    {
        // === Configurable Settings ===
        public float ArtificialLatencyMs { get; set; } = 150f;
        public float JitterMs { get; set; } = 20f;
        public bool OnlyInPracticeMode { get; set; } = true;
    }
}
