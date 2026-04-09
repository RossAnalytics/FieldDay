using System.Collections.Generic;
using Fusion;
using UnityEngine;

/// <summary>
/// Reusable networked turn system for sequential modes: Darts, Bowling, Golf, Billiards.
/// The active GameModeBase registers its players; TurnManager advances through them.
/// </summary>
public class TurnManager : NetworkBehaviour
{
    // ─── Networked State ───────────────────────────────────────────────────────

    [Networked] public PlayerRef ActivePlayer { get; private set; }
    [Networked] public int       TurnIndex    { get; private set; }

    // ─── Local State ───────────────────────────────────────────────────────────

    private List<PlayerRef>  _players = new();
    private GameModeBase     _currentMode;

    // ─── Setup ─────────────────────────────────────────────────────────────────

    /// <summary>
    /// Register the turn order. Call from the mode's StartMode().
    /// </summary>
    public void Initialise(List<PlayerRef> players, GameModeBase mode)
    {
        _players     = new List<PlayerRef>(players);
        _currentMode = mode;
        TurnIndex    = 0;

        if (_players.Count > 0)
            BeginTurn(_players[0]);
    }

    // ─── Turn Advancement ──────────────────────────────────────────────────────

    /// <summary>
    /// Call this when the current player's action is complete.
    /// Advances to the next player (wraps around).
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_EndCurrentTurn()
    {
        _currentMode?.OnTurnEnd(ActivePlayer);

        TurnIndex = (TurnIndex + 1) % _players.Count;
        BeginTurn(_players[TurnIndex]);
    }

    private void BeginTurn(PlayerRef player)
    {
        ActivePlayer = player;
        _currentMode?.OnTurnStart(player);
        Debug.Log($"[TurnManager] Turn started for player {player}");
    }

    /// <summary>Stop turn rotation (call on mode end).</summary>
    public void StopTurns()
    {
        ActivePlayer = PlayerRef.None;
        _players.Clear();
        _currentMode = null;
    }
}
