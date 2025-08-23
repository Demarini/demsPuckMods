using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TestProject.Models
{
    public static class ModalPrefs
    {
        const string Prefix = "YourMod.Modal.Hide.";
        public static bool GetHide(string key) => PlayerPrefs.GetInt(Prefix + key, 0) == 1;
        public static void SetHide(string key, bool hide)
        {
            PlayerPrefs.SetInt(Prefix + key, hide ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
