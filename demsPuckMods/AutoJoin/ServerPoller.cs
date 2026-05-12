using System;
using System.Net;
using System.Text.Json;
using System.Threading;
using UnityEngine;

namespace AutoJoin
{
    public class ServerPollResult
    {
        public EndPoint EndPoint { get; set; }
        public string Name { get; set; }
        public int Players { get; set; }
        public int MaxPlayers { get; set; }
        public bool IsPasswordProtected { get; set; }
        public bool Reachable { get; set; }

        public bool HasOpenSlot => Reachable && Players < MaxPlayers;
    }

    public static class ServerPoller
    {
        private const int ConnectTimeoutMs = 3000;
        private const int ResponseTimeoutMs = 3000;
        private const int ReadTimeoutMs = 3000;

        public static ServerPollResult Poll(EndPoint endPoint)
        {
            var result = new ServerPollResult { EndPoint = endPoint, Reachable = false };

            try
            {
                var client = new TCPClient(endPoint, ConnectTimeoutMs, ReadTimeoutMs);
                ServerPreviewData previewData = null;
                var responseEvent = new ManualResetEventSlim(false);

                client.OnConnected += () =>
                {
                    var request = new TCPServerPreviewRequest();
                    var message = JsonSerializer.Serialize(request, (JsonSerializerOptions)null);
                    client.SendMessage(message);
                };

                client.OnMessageReceived += (string message) =>
                {
                    try
                    {
                        var serverMessage = JsonSerializer.Deserialize<TCPServerMessage>(message, (JsonSerializerOptions)null);
                        if (serverMessage.type == TCPServerMessageType.PreviewResponse)
                        {
                            var response = JsonSerializer.Deserialize<TCPServerPreviewResponse>(message, (JsonSerializerOptions)null);
                            previewData = new ServerPreviewData
                            {
                                name = response.name,
                                players = response.players,
                                maxPlayers = response.maxPlayers,
                                isPasswordProtected = response.isPasswordProtected,
                                clientRequiredModIds = response.clientRequiredModIds
                            };
                            responseEvent.Set();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"[AutoJoin] Error parsing response from {endPoint}: {ex.Message}");
                    }
                };

                client.Connect();

                if (client.IsConnected)
                {
                    responseEvent.Wait(ResponseTimeoutMs);
                    client.Disconnect();
                }

                if (previewData != null)
                {
                    result.Reachable = true;
                    result.Name = previewData.name;
                    result.Players = previewData.players;
                    result.MaxPlayers = previewData.maxPlayers;
                    result.IsPasswordProtected = previewData.isPasswordProtected;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AutoJoin] Failed to poll {endPoint}: {ex.Message}");
            }

            return result;
        }
    }
}
