using System.Collections.Generic;
using Fusion;
using UnityEngine;

/// <summary>
/// Darts game mode — 301 or 501 countdown scoring.
///
/// Rules:
///   - Each player starts at 301 (or 501).
///   - Players alternate turns; each turn = 3 darts thrown.
///   - Score is subtracted from the remaining total.
///   - A player wins by reaching exactly 0. Going below 0 is a "bust" — turn score ignored.
///   - Double-out variant: final dart must hit a double segment.
///
/// TODO (next steps):
///   1. Spawn the dartboard NetworkObject and position it in the Darts scene.
///   2. Wire DartsAiming to the local player's input.
///   3. Hook dart landing position to DartsScoring.CalculateScore(hitPoint).
///   4. After 3 darts, call TurnManager.RPC_EndCurrentTurn().
/// </summary>
public class DartsGameMode : GameModeBase
{
    // ─── Configuration ─────────────────────────────────────────────────────────

    [Header("Darts Config")]
    [SerializeField] private int startingScore = 301; // 301 or 501
    [SerializeField] private bool doubleOut    = false;

    // ─── Networked State ───────────────────────────────────────────────────────

    [Networked] private int DartsThisRound { get; set; }

    // ─── Local State ───────────────────────────────────────────────────────────

    private List<PlayerRef>       _turnOrder = new();
    private Dictionary<int, int>  _scores    = new(); // playerId → remaining score

    private TurnManager  _turnManager;
    private DartsScoring _scoring;

    // ─── GameModeBase Implementation ───────────────────────────────────────────

    public override void StartMode()
    {
        ModeName = "Darts";
        _turnManager = FindObjectOfType<TurnManager>();
        _scoring     = GetComponent<DartsScoring>();

        // Collect players and initialise scores
        foreach (var player in Runner.ActivePlayers)
        {
            _turnOrder.Add(player);
            _scores[player.PlayerId] = startingScore;
        }

        DartsThisRound = 0;
        _turnManager?.Initialise(_turnOrder, this);

        Debug.Log($"[DartsGameMode] Started — {startingScore} down, {_turnOrder.Count} players.");
    }

    public override void EndMode()
    {
        _turnManager?.StopTurns();
        // TODO: hide darts UI, despawn dartboard
        Debug.Log("[DartsGameMode] Mode ended.");
    }

    public override void OnTurnStart(PlayerRef player)
    {
        DartsThisRound = 0;
        // TODO: enable DartsAiming for the local player if player == Runner.LocalPlayer
        Debug.Log($"[DartsGameMode] Turn started for {player} — 3 darts remaining.");
    }

    public override void OnTurnEnd(PlayerRef player)
    {
        // TODO: show score update animation
        Debug.Log($"[DartsGameMode] Turn ended for {player}. Remaining: {_scores[player.PlayerId]}");
    }

    /// <summary>
    /// Called by DartsAiming when a dart lands. Apply score delta.
    /// </summary>
    public void RegisterThrow(PlayerRef thrower, int pointsScored)
    {
        int id = thrower.PlayerId;
        int current = _scores[id];
        int next    = current - pointsScored;

        if (next < 0 || (doubleOut && next == 0 /* TODO: check double hit */))
        {
            Debug.Log($"[DartsGameMode] BUST — {thrower} stays at {current}.");
        }
        else
        {
            _scores[id] = next;
            if (next == 0) { SignalModeComplete(); return; }
        }

        DartsThisRound++;
        if (DartsThisRound >= 3)
            _turnManager?.RPC_EndCurrentTurn();
    }

    public override PlayerRef CalculateWinner()
    {
        // Winner is whoever reached 0 first (tracked via SignalModeComplete)
        foreach (var kvp in _scores)
            if (kvp.Value == 0)
                foreach (var p in _turnOrder)
                    if (p.PlayerId == kvp.Key) return p;

        return PlayerRef.None;
    }
}
