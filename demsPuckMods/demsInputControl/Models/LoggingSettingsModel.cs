using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demsInputControl.Models
{
    public class LoggingSettingsModel
    {
        public bool VelocityLogging { get; set; } = false;
        public bool RPCLogging { get; set; } = false;
        public bool SprintControlLogging { get; set; } = false;
        public bool PracticeModeDetectionLogging { get; set; } = false;
        public bool StopControlLogging { get; set; } = false;
    }
}
