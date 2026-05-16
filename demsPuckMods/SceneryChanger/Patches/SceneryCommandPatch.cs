using HarmonyLib;
using SceneryChanger.Behaviors;
using SceneryChanger.Services;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace SceneryChanger.Patches
{
    [HarmonyPatch(typeof(VoteManagerController), "Event_Server_OnChatCommand")]
    public static class SceneryCommandPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(Dictionary<string, object> message)
        {
            string command = ((string)message["command"]).ToLowerInvariant();
            string[] args = (string[])message["args"];
            ulong clientId = (ulong)message["clientId"];

            switch (command)
            {
                case "/sl":
                    if (args.Length == 0 || args[0].Equals("help", StringComparison.OrdinalIgnoreCase))
                    {
                        SendHelp(clientId);
                        return false;
                    }
                    return true;

                case "/slmusicvolume":
                    SetVolume(args, clientId, "Music", "/slMusicVolume",
                        v => {
                            SceneryAudioState.MusicVolume = v;
                            UpdateAudioSources.ApplyVolumesNow();
                        },
                        () => SceneryAudioState.MusicVolume);
                    return false;

                case "/slambientvolume":
                    SetVolume(args, clientId, "Ambient Audio", "/slAmbientVolume",
                        v => {
                            SceneryAudioState.AmbientAudioVolume = v;
                            if (SceneryAudioState.AmbientAudioSource != null)
                                SceneryAudioState.AmbientAudioSource.volume = v;
                        },
                        () => SceneryAudioState.AmbientAudioVolume);
                    return false;

                case "/slgoalcrowdnoise":
                    SetVolume(args, clientId, "Goal Crowd Noise", "/slGoalCrowdNoise",
                        v => SceneryAudioState.GoalCrowdNoiseVolume = v,
                        () => SceneryAudioState.GoalCrowdNoiseVolume);
                    return false;

                default:
                    return true;
            }
        }

        static void SetVolume(string[] args, ulong clientId, string label, string commandName,
            Action<float> apply, Func<float> getCurrent)
        {
            if (args.Length == 0)
            {
                float cur = getCurrent();
                Reply(clientId, $"{label} volume: {Mathf.RoundToInt(cur * 100)}");
                return;
            }

            int val;
            if (!int.TryParse(args[0], out val) || val < 0 || val > 100)
            {
                Reply(clientId, $"Usage: {commandName} <0-100>");
                return;
            }

            float volume = val / 100f;
            apply(volume);
            Reply(clientId, $"{label} volume set to {val}");
            Debug.Log($"[SceneryLoader] {label} volume set to {val} by client {clientId}");
        }

        static void SendHelp(ulong clientId)
        {
            int m = Mathf.RoundToInt(SceneryAudioState.MusicVolume * 100);
            int a = Mathf.RoundToInt(SceneryAudioState.AmbientAudioVolume * 100);
            int g = Mathf.RoundToInt(SceneryAudioState.GoalCrowdNoiseVolume * 100);

            Reply(clientId,
                "SceneryLoader Commands:\n" +
                "/sl help - Show this help\n" +
                $"/slMusicVolume <0-100> - Music volume (current: {m})\n" +
                $"/slAmbientVolume <0-100> - Ambient audio volume (current: {a})\n" +
                $"/slGoalCrowdNoise <0-100> - Goal crowd noise volume (current: {g})");
        }

        static void Reply(ulong clientId, string text)
        {
            UIChatControllerPatch.SendChatToClient(text, clientId);
        }
    }
}
