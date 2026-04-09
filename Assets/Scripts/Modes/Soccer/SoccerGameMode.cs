using System.Collections.Generic;
using Fusion;
using UnityEngine;

/// <summary>
/// Soccer game mode — penalty shootout or open play. Most goals wins.
///
/// TODO:
///   1. Build the pitch/penalty area scene.
///   2. Choose variant: penalty shootout (alternating kicks) or timed open play.
///   3. Implement goalkeeper AI or player-controlled keeper.
///   4. Track goals per team, not per player (Soccer is team-oriented).
///   5. Tie-break: sudden death penalty round.
/// </summary>
public class SoccerGameMode : GameModeBase
{
    [Header("Soccer Config")]
    [SerializeField] private float matchDuration = 120f; // seconds
    [SerializeField] private bool  penaltyMode   = true; // true = shootout, false = open play

    [Networked] private float        TimeRemaining { get; set; }
    [Networked] private NetworkBool  TimerRunning  { get; set; }
    [Networked] private int          TeamAGoals    { get; set; }
    [Networked] private int          TeamBGoals    { get; set; }

    private List<PlayerRef> _turnOrder = new();
    private TurnManager     _turnManager;

    public override void StartMode()
    {
        ModeName      = "Soccer";
        _turnManager  = FindObjectOfType<TurnManager>();
        TimeRemaining = penaltyMode ? 0f : matchDuration;
        TeamAGoals    = 0;
        TeamBGoals    = 0;

        foreach (var player in Runner.ActivePlayers)
            _turnOrder.Add(player);

        if (penaltyMode)
            _turnManager?.Initialise(_turnOrder, this);
        else
        {
            TimerRunning = true;
            // TODO: spawn all players simultaneously for open play
        }

        Debug.Log($"[SoccerGameMode] Started — {(penaltyMode ? "Penalty Shootout" : "Open Play")}");
    }

    public override void FixedUpdateNetwork()
    {
        if (!TimerRunning || !Object.HasStateAuthority) return;
        TimeRemaining -= Runner.DeltaTime;
        if (TimeRemaining <= 0f) { TimerRunning = false; SignalModeComplete(); }
    }

    public override void EndMode()
    {
        TimerRunning = false;
        _turnManager?.StopTurns();
    }

    public override void OnTurnStart(PlayerRef player) { }
    public override void OnTurnEnd(PlayerRef player) { }

    /// <summary>Called by SoccerGoal when a goal is detected.</summary>
    public void RecordGoal(TeamManager.Team scoringTeam)
    {
        if (scoringTeam == TeamManager.Team.A) TeamAGoals++;
        else                                   TeamBGoals++;
        Debug.Log($"[SoccerGameMode] Goal! A:{TeamAGoals} B:{TeamBGoals}");
    }

    public override PlayerRef CalculateWinner()
    {
        // TODO: return the winning team's captain / MVP player
        // For now return PlayerRef.None (team scoring handled separately)
        return PlayerRef.None;
    }
}
