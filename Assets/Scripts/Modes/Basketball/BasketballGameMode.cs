using System.Collections.Generic;
using Fusion;
using UnityEngine;

/// <summary>
/// Basketball game mode — timed shooting rounds. Highest score wins.
///
/// TODO:
///   1. Build court scene with hoop, backboard, ball spawn.
///   2. Implement shot clock or round timer (e.g., 60 seconds per player).
///   3. Award 2 pts for regular shots, 3 pts from beyond the arc (optional).
///   4. Play a highlight animation on successful hoops.
///   5. Optional: H-O-R-S-E variant where players must match each other's shots.
/// </summary>
public class BasketballGameMode : GameModeBase
{
    [Header("Basketball Config")]
    [SerializeField] private float roundDurationSeconds = 60f;

    [Networked] private float TimeRemaining { get; set; }
    [Networked] private NetworkBool TimerRunning { get; set; }

    private List<PlayerRef>      _turnOrder = new();
    private Dictionary<int, int> _scores    = new();
    private TurnManager          _turnManager;

    public override void StartMode()
    {
        ModeName     = "Basketball";
        _turnManager = FindObjectOfType<TurnManager>();
        TimeRemaining = roundDurationSeconds;

        foreach (var player in Runner.ActivePlayers)
        {
            _turnOrder.Add(player);
            _scores[player.PlayerId] = 0;
        }

        _turnManager?.Initialise(_turnOrder, this);
        TimerRunning = true;
        Debug.Log("[BasketballGameMode] Started.");
    }

    public override void FixedUpdateNetwork()
    {
        if (!TimerRunning || !Object.HasStateAuthority) return;

        TimeRemaining -= Runner.DeltaTime;
        if (TimeRemaining <= 0f)
        {
            TimerRunning = false;
            SignalModeComplete();
        }
    }

    public override void EndMode()
    {
        TimerRunning = false;
        _turnManager?.StopTurns();
    }

    public override void OnTurnStart(PlayerRef player) { }
    public override void OnTurnEnd(PlayerRef player) { }

    /// <summary>Called by BasketballHoop when ball passes through.</summary>
    public void RecordScore(PlayerRef player, int points)
    {
        _scores[player.PlayerId] += points;
        Debug.Log($"[BasketballGameMode] {player} scored {points} pts. Total: {_scores[player.PlayerId]}");
    }

    public override PlayerRef CalculateWinner()
    {
        PlayerRef winner = PlayerRef.None;
        int best = -1;
        foreach (var p in _turnOrder)
        {
            int s = _scores[p.PlayerId];
            if (s > best) { best = s; winner = p; }
        }
        return winner;
    }
}
