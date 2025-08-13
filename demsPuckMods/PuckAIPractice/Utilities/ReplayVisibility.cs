using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuckAIPractice.Utilities
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Unity.Netcode;

    public static class ReplayVisibility
    {
        // Cache last live pose per player
        private static readonly Dictionary<ulong, (Vector3 pos, Quaternion rot)> _savedPose = new Dictionary<ulong, (Vector3 pos, Quaternion rot)>();

        public static void EnterReplayStasis(Player p)
        {
            if (!p || !p.NetworkObject || !NetworkManager.Singleton.IsServer) return;

            // 1) save pose
            _savedPose[p.OwnerClientId] = (p.transform.position, p.transform.rotation);

            // 2) freeze physics (server-side only)
            var rb = p.PlayerBody?.Rigidbody;
            if (rb)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
                rb.Sleep();
            }

            // 3) hide from every connected client (including host's client)
            foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                p.NetworkObject.NetworkHide(clientId);
                // If the stick/body are separate NetworkObjects, hide them too:
                p.PlayerBody?.NetworkObject?.NetworkHide(clientId);
                p.Stick?.NetworkObject?.NetworkHide(clientId);
            }
        }

        public static IEnumerator ExitReplayStasis(Player p)
        {
            if (!p || !p.NetworkObject || !NetworkManager.Singleton.IsServer) yield break;

            // Set pose BEFORE making visible again
            if (_savedPose.TryGetValue(p.OwnerClientId, out var pose))
            {
                var rb = p.PlayerBody?.Rigidbody;
                if (rb)
                {
                    rb.position = pose.pos;
                    rb.rotation = pose.rot;
                }
                else
                {
                    p.transform.SetPositionAndRotation(pose.pos, pose.rot);
                }
            }
            Physics.SyncTransforms();
            yield return new WaitForFixedUpdate();

            // Show back to all clients
            foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                p.NetworkObject.NetworkShow(clientId);
                p.PlayerBody?.NetworkObject?.NetworkShow(clientId);
                p.Stick?.NetworkObject?.NetworkShow(clientId);
            }

            // Unfreeze physics
            var rb2 = p.PlayerBody?.Rigidbody;
            if (rb2)
            {
                rb2.isKinematic = false;
                rb2.WakeUp();
            }
            _savedPose.Remove(p.OwnerClientId);
        }
    }
}
