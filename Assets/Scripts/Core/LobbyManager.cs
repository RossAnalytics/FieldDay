using Fusion;
using UnityEngine;

/// <summary>
/// Handles the pre-game lobby phase.
/// Waits for all connected players to set "ready", then triggers GameManager.StartGame().
/// </summary>
public class LobbyManager : NetworkBehaviour
{
    [Networked] private int ReadyCount { get; set; }

    private int _totalPlayers;

    public override void Spawned()
    {
        _totalPlayers = Runner.ActivePlayers.GetHashCode(); // replace with actual count
        ReadyCount = 0;
    }

    /// <summary>Called by the local player's Ready button via LobbyUI.</summary>
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_PlayerReady()
    {
        ReadyCount++;
        Debug.Log($"[LobbyManager] Ready: {ReadyCount}/{_totalPlayers}");

        // TODO: compare ReadyCount to actual connected player count
        if (ReadyCount >= 2) // minimum 2 players to start
            GameManager.Instance?.StartGame();
    }
}
