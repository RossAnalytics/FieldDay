using System.Collections.Generic;
using Fusion;
using UnityEngine;

/// <summary>
/// Golf game mode — stroke-play across one or more holes.
///
/// TODO:
///   1. Design hole scenes: tee, fairway, green, hole cup.
///   2. Implement par values per hole.
///   3. Track stroke count per player per hole.
///   4. Detect ball in cup → end hole, advance to next or end mode.
///   5. Winner = fewest total strokes (lowest score wins in golf).
/// </summary>
public class GolfGameMode : GameModeBase
{
    [Header("Golf Config")]
    [SerializeField] private int numberOfHoles = 3;
    [SerializeField] private int[] parPerHole  = { 3, 3, 4 }; // set per hole in Inspector

    private List<PlayerRef>             _turnOrder = new();
    private Dictionary<int, List<int>>  _strokes   = new(); // playerId → strokes per hole
    private int                         _currentHole;

    private TurnManager _turnManager;

    public override void StartMode()
    {
        ModeName    = "Golf";
        _turnManager = FindObjectOfType<TurnManager>();
        _currentHole = 0;

        foreach (var player in Runner.ActivePlayers)
        {
            _turnOrder.Add(player);
            _strokes[player.PlayerId] = new List<int>(new int[numberOfHoles]);
        }

        _turnManager?.Initialise(_turnOrder, this);
        Debug.Log($"[GolfGameMode] Started — {numberOfHoles} holes.");
    }

    public override void EndMode()
    {
        _turnManager?.StopTurns();
        Debug.Log("[GolfGameMode] Mode ended.");
    }

    public override void OnTurnStart(PlayerRef player)
        => Debug.Log($"[GolfGameMode] {player}'s shot — hole {_currentHole + 1}");

    public override void OnTurnEnd(PlayerRef player)
        => Debug.Log($"[GolfGameMode] {player} finished hole {_currentHole + 1}");

    /// <summary>Record a stroke for the active player.</summary>
    public void RecordStroke(PlayerRef player)
    {
        _strokes[player.PlayerId][_currentHole]++;
    }

    /// <summary>Ball entered the cup — hole complete.</summary>
    public void OnBallInCup(PlayerRef player)
    {
        _currentHole++;
        if (_currentHole >= numberOfHoles)
            SignalModeComplete();
        else
            Debug.Log($"[GolfGameMode] Advancing to hole {_currentHole + 1}");
    }

    public override PlayerRef CalculateWinner()
    {
        PlayerRef winner = PlayerRef.None;
        int fewest = int.MaxValue;

        foreach (var player in _turnOrder)
        {
            int total = 0;
            foreach (int s in _strokes[player.PlayerId]) total += s;
            if (total < fewest) { fewest = total; winner = player; }
        }

        return winner;
    }
}
