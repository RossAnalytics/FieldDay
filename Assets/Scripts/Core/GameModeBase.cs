using Fusion;
using UnityEngine;

/// <summary>
/// Abstract base class for all FieldDay game modes.
/// Every sport (Darts, Bowling, Golf, etc.) extends this class.
/// Inherits from Fusion's NetworkBehaviour so mode state is network-synced.
/// </summary>
public abstract class GameModeBase : NetworkBehaviour
{
    // ─── Networked State ───────────────────────────────────────────────────────

    /// <summary>Display name shown in the UI (e.g. "Darts", "Bowling").</summary>
    [Networked] public NetworkString<_32> ModeName { get; protected set; }

    /// <summary>True once the mode has finished and a winner is determined.</summary>
    [Networked] public NetworkBool IsComplete { get; protected set; }

    // ─── Lifecycle ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Called by GameManager when this mode becomes active.
    /// Initialise scores, spawn objects, show UI.
    /// </summary>
    public abstract void StartMode();

    /// <summary>
    /// Called when the mode ends (time up, win condition met, etc.).
    /// Clean up networked objects, hide mode-specific UI.
    /// </summary>
    public abstract void EndMode();

    // ─── Turn Hooks ────────────────────────────────────────────────────────────

    /// <summary>
    /// Invoked by TurnManager at the start of a player's turn.
    /// Enable input, show aiming UI, reset throwable/ball.
    /// </summary>
    /// <param name="player">The Fusion PlayerRef whose turn is beginning.</param>
    public abstract void OnTurnStart(PlayerRef player);

    /// <summary>
    /// Invoked by TurnManager at the end of a player's turn.
    /// Disable input, tally score, advance state.
    /// </summary>
    /// <param name="player">The Fusion PlayerRef whose turn just ended.</param>
    public abstract void OnTurnEnd(PlayerRef player);

    // ─── Scoring ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Determines and returns the winning player after the mode finishes.
    /// GameManager calls this to update the team leaderboard.
    /// </summary>
    /// <returns>The PlayerRef of the winner, or PlayerRef.None for a draw.</returns>
    public abstract PlayerRef CalculateWinner();

    // ─── Shared Helpers ────────────────────────────────────────────────────────

    /// <summary>
    /// Convenience: broadcast an RPC announcing the mode is complete.
    /// Subclasses call this when their win condition is met.
    /// </summary>
    protected void SignalModeComplete()
    {
        IsComplete = true;
        RPC_NotifyModeComplete();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_NotifyModeComplete()
    {
        GameManager.Instance?.OnModeComplete(this);
    }
}
