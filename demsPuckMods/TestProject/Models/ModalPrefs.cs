using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MOTD.Models
{
    public static class ModalPrefs
    {
        const string VersionPrefix = "YourMod.Modal.DismissedVersion.";

        public static int GetDismissedVersion(string key) =>
            PlayerPrefs.GetInt(VersionPrefix + key, 0);

        public static void SetDismissedVersion(string key, int version)
        {
            PlayerPrefs.SetInt(VersionPrefix + key, version);
            PlayerPrefs.Save();
        }
    }
}
