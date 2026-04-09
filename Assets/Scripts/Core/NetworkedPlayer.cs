using Fusion;
using UnityEngine;

/// <summary>
/// Represents a connected player's persistent state across scenes.
/// Spawned by PhotonManager when a player joins; lives until they disconnect.
/// </summary>
public class NetworkedPlayer : NetworkBehaviour
{
    // ─── Networked Properties ──────────────────────────────────────────────────

    /// <summary>The player's display name (set from the lobby UI).</summary>
    [Networked(OnChanged = nameof(OnNameChanged))]
    public NetworkString<_32> PlayerName { get; set; }

    /// <summary>Which team this player belongs to.</summary>
    [Networked(OnChanged = nameof(OnTeamChanged))]
    public TeamManager.Team Team { get; set; }

    /// <summary>Running score for this session (mode wins).</summary>
    [Networked]
    public int Score { get; set; }

    /// <summary>Whether the player has clicked "Ready" in the lobby.</summary>
    [Networked(OnChanged = nameof(OnReadyChanged))]
    public NetworkBool IsReady { get; set; }

    // ─── Fusion Lifecycle ──────────────────────────────────────────────────────

    public override void Spawned()
    {
        // Local player: set name from prefs / UI
        if (Object.HasInputAuthority)
        {
            string name = PlayerPrefs.GetString("PlayerName", $"Player{Object.InputAuthority.PlayerId}");
            RPC_SetName(name);
        }
    }

    // ─── RPCs ──────────────────────────────────────────────────────────────────

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetName(NetworkString<_32> name) => PlayerName = name;

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetReady(bool ready) => IsReady = ready;

    // ─── Change Handlers ───────────────────────────────────────────────────────

    private static void OnNameChanged(Changed<NetworkedPlayer> changed)
        => Debug.Log($"[NetworkedPlayer] Name changed to: {changed.Behaviour.PlayerName}");

    private static void OnTeamChanged(Changed<NetworkedPlayer> changed)
        => Debug.Log($"[NetworkedPlayer] Team changed to: {changed.Behaviour.Team}");

    private static void OnReadyChanged(Changed<NetworkedPlayer> changed)
        => Debug.Log($"[NetworkedPlayer] Ready state: {changed.Behaviour.IsReady}");
}
