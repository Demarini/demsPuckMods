using SceneryChanger.Behaviors;
using SceneryChanger.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine;
using SceneryLoader.Behaviors;

namespace SceneryChanger.Services
{
    public static class SceneLoadCoordinator
    {
        public static float ServerWindowSec = 0.5f;

        static bool _waiting;
        static float _deadline;
        static SceneInformation _serverSI;
        static SceneSignature _serverSig;
        static SceneSignature _pendingSig;
        static bool _practice;
        static Coroutine _waitCo;     // NEW: handle so we can cancel the waiter

        public static void OnSceneLoaded(Scene scene, bool isPractice, SceneInformation localSI)
        {
            if (!scene.name.Equals("level_1", StringComparison.OrdinalIgnoreCase)) { Reset(); return; }

            _practice = isPractice;
            _serverSI = null; _serverSig = null;
            _pendingSig = null;

            if (_practice)
            {
                if (localSI != null && localSI.useSceneLocally)
                    RequestLoad(scene, localSI, "LOCAL (practice)");
                return;
            }

            // start window
            _waiting = true;
            _deadline = Time.realtimeSinceStartup + ServerWindowSec;

            // ensure only one waiter running
            if (_waitCo != null) CoroutineRunner.Instance.StopCoroutine(_waitCo);
            _waitCo = CoroutineRunner.Instance.StartCoroutine(WaitForServerThenChoose(scene, localSI));
        }

        public static void OnServerSceneDirective(SceneInformation siFromServer)
        {
            if (siFromServer == null) return;

            var sig = new SceneSignature(siFromServer);

            // If already requested same scene, ignore
            if (_pendingSig != null && sig.Equals(_pendingSig)) return;

            _serverSI = siFromServer;
            _serverSig = sig;

            if (_waiting)
            {
                // PREEMPT: stop the waiter and load immediately from server
                _waiting = false;
                if (_waitCo != null) { CoroutineRunner.Instance.StopCoroutine(_waitCo); _waitCo = null; }

                // pick server instantly
                RequestLoad(RinkOnlyPruner.scene, siFromServer, "SERVER (preempt)");
            }
            else
            {
                // Window elapsed; policy: ignore or supersede.
                // If you want to supersede even after local load, do it here:
                // RequestLoad(RinkOnlyPruner.scene, siFromServer, "SERVER (late supersede)");
                Debug.Log("[Coordinator] Server directive arrived after window; ignoring.");
            }
        }

        public static void OnSceneUnloaded() => Reset();

        static void Reset()
        {
            _waiting = false;
            _serverSI = null; _serverSig = null; _pendingSig = null;
            if (_waitCo != null) { CoroutineRunner.Instance.StopCoroutine(_waitCo); _waitCo = null; }
        }

        static IEnumerator WaitForServerThenChoose(Scene scene, SceneInformation localSI)
        {
            while (_waiting && Time.realtimeSinceStartup < _deadline)
                yield return null;

            _waiting = false; _waitCo = null;

            // Prefer server if it arrived during window, else local (if allowed)
            var chosen = _serverSI ?? (localSI != null && localSI.useSceneLocally ? localSI : null);
            if (chosen == null) { Debug.Log("[Coordinator] No server directive and local disabled; idle."); yield break; }

            var sig = new SceneSignature(chosen);
            if (_pendingSig != null && sig.Equals(_pendingSig)) { Debug.Log("[Coordinator] Duplicate request; skip."); yield break; }

            RequestLoad(scene, chosen, _serverSI != null ? "SERVER" : "LOCAL (timeout)");
        }

        static void RequestLoad(Scene scene, SceneInformation si, string sourceTag)
        {
            var sig = new SceneSignature(si);
            _pendingSig = sig;
            Debug.Log($"[Coordinator] Requesting load: {sourceTag} -> {sig}");
            RinkSceneLoader.LoadSceneAsync(scene, si);
        }
    }
}
