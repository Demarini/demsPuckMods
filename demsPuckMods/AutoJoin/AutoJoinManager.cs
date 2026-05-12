using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace AutoJoin
{
    public class AutoJoinManager
    {
        private static AutoJoinManager _instance;
        public static AutoJoinManager Instance => _instance ?? (_instance = new AutoJoinManager());

        private readonly Dictionary<EndPoint, AutoJoinEntry> _watchedServers = new Dictionary<EndPoint, AutoJoinEntry>();
        private Coroutine _pollCoroutine;
        private MonoBehaviour _coroutineHost;
        private bool _running;

        public float PollIntervalSeconds { get; set; } = 10f;

        public IReadOnlyDictionary<EndPoint, AutoJoinEntry> WatchedServers => _watchedServers;
        public bool IsRunning => _running;

        public event Action<EndPoint, ServerPollResult> OnServerPolled;
        public event Action<EndPoint, ServerPollResult> OnSlotAvailable;
        public event Action<EndPoint> OnAutoJoinAttempt;

        public void Initialize()
        {
            Debug.Log("[AutoJoin] Manager initialized");
        }

        public void Shutdown()
        {
            StopPolling();
            _watchedServers.Clear();
            Debug.Log("[AutoJoin] Manager shut down");
        }

        public void WatchServer(EndPoint endPoint, string password = null, bool autoJoin = true)
        {
            if (_watchedServers.ContainsKey(endPoint))
            {
                Debug.Log($"[AutoJoin] Already watching {endPoint}");
                return;
            }

            _watchedServers[endPoint] = new AutoJoinEntry
            {
                EndPoint = endPoint,
                Password = password,
                AutoJoin = autoJoin
            };

            Debug.Log($"[AutoJoin] Now watching {endPoint} (autoJoin={autoJoin})");

            if (!_running)
            {
                StartPolling();
            }
        }

        public void UnwatchServer(EndPoint endPoint)
        {
            if (_watchedServers.Remove(endPoint))
            {
                Debug.Log($"[AutoJoin] Stopped watching {endPoint}");
            }

            if (_watchedServers.Count == 0)
            {
                StopPolling();
            }
        }

        public void ClearAll()
        {
            _watchedServers.Clear();
            StopPolling();
            Debug.Log("[AutoJoin] Cleared all watched servers");
        }

        private void StartPolling()
        {
            if (_running) return;

            _coroutineHost = MonoBehaviourSingleton<ConnectionManager>.Instance;
            if (_coroutineHost == null)
            {
                Debug.LogError("[AutoJoin] Cannot start polling - ConnectionManager not available");
                return;
            }

            _running = true;
            _pollCoroutine = _coroutineHost.StartCoroutine(PollLoop());
            Debug.Log($"[AutoJoin] Polling started (interval={PollIntervalSeconds}s)");
        }

        private void StopPolling()
        {
            if (!_running) return;

            if (_coroutineHost != null && _pollCoroutine != null)
            {
                _coroutineHost.StopCoroutine(_pollCoroutine);
            }

            _pollCoroutine = null;
            _running = false;
            Debug.Log("[AutoJoin] Polling stopped");
        }

        private IEnumerator PollLoop()
        {
            while (_running && _watchedServers.Count > 0)
            {
                var endpoints = new List<EndPoint>(_watchedServers.Keys);

                foreach (var endPoint in endpoints)
                {
                    if (!_watchedServers.ContainsKey(endPoint)) continue;
                    if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsClient) break;

                    ServerPollResult pollResult = null;
                    bool taskDone = false;

                    Task.Run(() =>
                    {
                        pollResult = ServerPoller.Poll(endPoint);
                        taskDone = true;
                    });

                    while (!taskDone)
                    {
                        yield return null;
                    }

                    if (pollResult == null) continue;

                    var entry = _watchedServers.ContainsKey(endPoint) ? _watchedServers[endPoint] : null;
                    if (entry == null) continue;

                    entry.LastPollResult = pollResult;
                    entry.LastPollTime = DateTime.UtcNow;

                    Debug.Log($"[AutoJoin] Polled {endPoint}: {pollResult.Name} ({pollResult.Players}/{pollResult.MaxPlayers}) reachable={pollResult.Reachable}");

                    OnServerPolled?.Invoke(endPoint, pollResult);

                    if (pollResult.HasOpenSlot)
                    {
                        Debug.Log($"[AutoJoin] Slot available on {pollResult.Name}!");
                        OnSlotAvailable?.Invoke(endPoint, pollResult);

                        if (entry.AutoJoin)
                        {
                            AttemptJoin(endPoint, entry.Password);
                            yield break;
                        }
                    }
                }

                yield return new WaitForSeconds(PollIntervalSeconds);
            }

            _running = false;
        }

        private void AttemptJoin(EndPoint endPoint, string password)
        {
            Debug.Log($"[AutoJoin] Attempting to join {endPoint}");
            OnAutoJoinAttempt?.Invoke(endPoint);

            UnwatchServer(endPoint);

            var connectionManager = MonoBehaviourSingleton<ConnectionManager>.Instance;
            if (connectionManager == null)
            {
                Debug.LogError("[AutoJoin] ConnectionManager not available for join");
                return;
            }

            connectionManager.Client_StartClient(endPoint.ipAddress, endPoint.port, password);
        }
    }

    public class AutoJoinEntry
    {
        public EndPoint EndPoint { get; set; }
        public string Password { get; set; }
        public bool AutoJoin { get; set; }
        public ServerPollResult LastPollResult { get; set; }
        public DateTime? LastPollTime { get; set; }
    }
}
