using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using System;

/// <summary>
/// Manages the Photon Fusion 2 NetworkRunner.
/// Handles room creation/joining, player spawning, and team assignment.
/// Attach to a persistent GameObject in the bootstrap scene.
/// </summary>
public class PhotonManager : MonoBehaviour, INetworkRunnerCallbacks
{
    // ─── Singleton ─────────────────────────────────────────────────────────────

    public static PhotonManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ─── Configuration ─────────────────────────────────────────────────────────

    [Header("Configuration")]
    [Tooltip("Prefab with NetworkedPlayer component — spawned for each joining player.")]
    [SerializeField] private NetworkObject playerPrefab;

    [Tooltip("Max players per room (2–12 for 2–6 teams of 2).")]
    [SerializeField] private int maxPlayers = 6;

    // ─── Runner ────────────────────────────────────────────────────────────────

    private NetworkRunner _runner;
    public  NetworkRunner  Runner => _runner;

    // ─── Room Management ───────────────────────────────────────────────────────

    /// <summary>Create a new room as host (State Authority).</summary>
    public async void CreateRoom(string roomName)
    {
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        var args = new StartGameArgs
        {
            GameMode    = GameMode.Host,
            SessionName = roomName,
            PlayerCount = maxPlayers,
            Scene       = SceneRef.FromIndex(1), // TODO: set correct scene index
        };

        var result = await _runner.StartGame(args);
        if (!result.Ok)
            Debug.LogError($"[PhotonManager] Failed to start host: {result.ShutdownReason}");
    }

    /// <summary>Join an existing room by name.</summary>
    public async void JoinRoom(string roomName)
    {
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        var args = new StartGameArgs
        {
            GameMode    = GameMode.Client,
            SessionName = roomName,
        };

        var result = await _runner.StartGame(args);
        if (!result.Ok)
            Debug.LogError($"[PhotonManager] Failed to join room: {result.ShutdownReason}");
    }

    // ─── INetworkRunnerCallbacks ───────────────────────────────────────────────

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsServer) return;

        // Spawn the networked player avatar
        NetworkObject no = runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, player);

        // Assign to a team (A = even slots, B = odd slots)
        var np = no.GetComponent<NetworkedPlayer>();
        if (np != null)
        {
            int playerIndex = runner.ActivePlayers.GetHashCode() % 2; // simple alternating
            np.Team = (playerIndex == 0) ? TeamManager.Team.A : TeamManager.Team.B;
        }

        Debug.Log($"[PhotonManager] Player joined: {player} — spawned avatar.");
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"[PhotonManager] Player left: {player}");
        // TODO: handle mid-game disconnect (pause, replace with AI, etc.)
    }

    // ── Stub implementations of remaining INetworkRunnerCallbacks ─────────────

    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessage message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
}
