using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerBrowserInGame.Utilities
{
    public static class FavoriteFilter
    {
        public static bool Enabled;

        // open-instance delegate lets us call the private instance method SortServers(instance)
        private static Action<UIServerBrowser> _callSort;

        public static void BindSorterOnce()
        {
            if (_callSort != null) return;
            var mi = HarmonyLib.AccessTools.Method(typeof(UIServerBrowser), "SortServers");
            if (mi != null)
                _callSort = HarmonyLib.AccessTools.MethodDelegate<Action<UIServerBrowser>>(mi);
        }

        public static void ApplyNow()
        {
            BindSorterOnce();
            var browser = UIManager.Instance?.ServerBrowser;
            if (browser != null) _callSort?.Invoke(browser);
        }
    }
}
