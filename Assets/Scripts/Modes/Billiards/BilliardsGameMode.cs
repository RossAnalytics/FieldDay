using System.Collections.Generic;
using Fusion;
using UnityEngine;

/// <summary>
/// Billiards game mode — 8-ball or 9-ball rules.
///
/// TODO:
///   1. Build the pool table scene: table mesh, pockets (6 trigger zones), ball rack.
///   2. Spawn 15 numbered ball NetworkObjects + cue ball.
///   3. Implement turn: player shoots cue ball, wait for all balls to stop.
///   4. Detect fouls (scratch, wrong ball first, etc.).
///   5. 8-ball: assign solids/stripes after first pocket; win by potting your group then the 8.
///   6. 9-ball: must hit lowest numbered ball; pocket balls in order; 9-ball wins.
/// </summary>
public class BilliardsGameMode : GameModeBase
{
    public enum BilliardsVariant { EightBall, NineBall }

    [Header("Billiards Config")]
    [SerializeField] private BilliardsVariant variant = BilliardsVariant.EightBall;

    private List<PlayerRef> _turnOrder   = new();
    private TurnManager     _turnManager;

    public override void StartMode()
    {
        ModeName     = "Billiards";
        _turnManager = FindObjectOfType<TurnManager>();

        foreach (var player in Runner.ActivePlayers)
            _turnOrder.Add(player);

        _turnManager?.Initialise(_turnOrder, this);
        Debug.Log($"[BilliardsGameMode] Started — {variant}.");
    }

    public override void EndMode()
    {
        _turnManager?.StopTurns();
        Debug.Log("[BilliardsGameMode] Mode ended.");
    }

    public override void OnTurnStart(PlayerRef player)
    {
        // TODO: enable BilliardsAiming for the active player
        Debug.Log($"[BilliardsGameMode] {player}'s shot.");
    }

    public override void OnTurnEnd(PlayerRef player)
    {
        // TODO: evaluate potted balls, switch turn or continue if player made a pot
        Debug.Log($"[BilliardsGameMode] {player}'s turn ended.");
    }

    /// <summary>Called by a pocket trigger when a ball is potted.</summary>
    public void OnBallPotted(BilliardsBall ball, PlayerRef pocketedBy)
    {
        Debug.Log($"[BilliardsGameMode] Ball #{ball.BallNumber} potted by {pocketedBy}");
        // TODO: apply 8-ball / 9-ball rules
    }

    public override PlayerRef CalculateWinner()
    {
        // TODO: return player who legally potted the 8-ball (or 9-ball)
        return PlayerRef.None;
    }
}
