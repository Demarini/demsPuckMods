using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.Models
{
    [Serializable]
    public class ModalDoc
    {
        public int version = 1;
        public string title;
        public string richText;    // Unity rich text (we'll normalize <br> to \n)
        public string dontShowKey; // e.g. "motd-2025-08-19"
    }
}