using System.Collections.Generic;
using Fusion;
using UnityEngine;

/// <summary>
/// Bowling game mode — 10-frame scoring with spares and strikes.
///
/// Rules:
///   - 10 frames per player.
///   - 2 rolls per frame (1 if strike on first roll).
///   - Strike (all 10 on first roll): 10 + next 2 rolls as bonus.
///   - Spare  (all 10 on second roll): 10 + next 1 roll as bonus.
///   - 10th frame: up to 3 rolls if strike/spare is achieved.
///   - Perfect game: 300 (12 consecutive strikes).
///
/// TODO:
///   1. Build the lane scene: alley mesh, 10 pin positions, ball spawn.
///   2. Wire BowlingBall to TurnManager — ball settled = turn ends.
///   3. Implement the bonus roll logic for strikes/spares.
///   4. Display frame scores in a traditional bowling scorecard UI.
/// </summary>
public class BowlingGameMode : GameModeBase
{
    // ─── Networked State ───────────────────────────────────────────────────────

    [Networked] private int CurrentFrame { get; set; }
    [Networked] private int CurrentRoll  { get; set; }

    // ─── Local State ───────────────────────────────────────────────────────────

    private List<PlayerRef>              _turnOrder = new();
    private Dictionary<int, int[]>       _rolls     = new(); // playerId → roll array (21 rolls max)
    private Dictionary<int, int>         _rollIndex = new(); // playerId → next roll index

    private TurnManager _turnManager;

    // ─── GameModeBase ──────────────────────────────────────────────────────────

    public override void StartMode()
    {
        ModeName = "Bowling";
        _turnManager = FindObjectOfType<TurnManager>();

        foreach (var player in Runner.ActivePlayers)
        {
            _turnOrder.Add(player);
            _rolls[player.PlayerId]     = new int[21];
            _rollIndex[player.PlayerId] = 0;
        }

        _turnManager?.Initialise(_turnOrder, this);
        Debug.Log("[BowlingGameMode] Started — 10 frames, good luck!");
    }

    public override void EndMode()
    {
        _turnManager?.StopTurns();
        Debug.Log("[BowlingGameMode] Mode ended.");
    }

    public override void OnTurnStart(PlayerRef player)
    {
        CurrentRoll = _rollIndex[player.PlayerId];
        // TODO: reset standing pins, enable BowlingBall for this player
        Debug.Log($"[BowlingGameMode] Turn start — {player}, frame {CurrentFrame + 1}");
    }

    public override void OnTurnEnd(PlayerRef player)
    {
        Debug.Log($"[BowlingGameMode] Turn end — {player}, score so far: {CalculateTotal(player.PlayerId)}");
    }

    /// <summary>Called by BowlingBall once pins have settled after a roll.</summary>
    public void RecordRoll(PlayerRef player, int pinsKnocked)
    {
        int id = player.PlayerId;
        int ri = _rollIndex[id];
        _rolls[id][ri] = pinsKnocked;
        _rollIndex[id]++;

        // TODO: handle spare / strike second-roll logic and 10th frame extra rolls
        // Check if all players have finished 10 frames → SignalModeComplete()
    }

    private int CalculateTotal(int playerId)
    {
        int[] r = _rolls[playerId];
        int score = 0, rollIndex = 0;

        for (int frame = 0; frame < 10; frame++)
        {
            if (rollIndex >= r.Length) break;

            if (r[rollIndex] == 10) // Strike
            {
                score     += 10 + r[rollIndex + 1] + r[rollIndex + 2];
                rollIndex += 1;
            }
            else if (r[rollIndex] + r[rollIndex + 1] == 10) // Spare
            {
                score     += 10 + r[rollIndex + 2];
                rollIndex += 2;
            }
            else
            {
                score     += r[rollIndex] + r[rollIndex + 1];
                rollIndex += 2;
            }
        }

        return score;
    }

    public override PlayerRef CalculateWinner()
    {
        PlayerRef winner = PlayerRef.None;
        int best = -1;

        foreach (var player in _turnOrder)
        {
            int total = CalculateTotal(player.PlayerId);
            if (total > best) { best = total; winner = player; }
        }

        return winner;
    }
}
