using System.Collections.Generic;
using Fusion;
using UnityEngine;

/// <summary>
/// Tracks per-player and per-team scores across all modes in a session.
/// Lives on the GameManager's NetworkObject so scores are authority-synced.
/// </summary>
public class ScoreManager : NetworkBehaviour
{
    // Keyed by PlayerRef.PlayerId (int) — Fusion-safe primitive.
    private readonly Dictionary<int, int> _playerScores = new();

    /// <summary>Award one mode-win point to a player.</summary>
    public void AddPoint(PlayerRef player)
    {
        int id = player.PlayerId;
        _playerScores.TryGetValue(id, out int current);
        _playerScores[id] = current + 1;
        Debug.Log($"[ScoreManager] Player {id} now has {_playerScores[id]} point(s).");
    }

    /// <summary>Returns total mode-win points for a player.</summary>
    public int GetScore(PlayerRef player)
    {
        _playerScores.TryGetValue(player.PlayerId, out int score);
        return score;
    }

    /// <summary>Resets all scores — call at the start of a new session.</summary>
    public void ResetAll()
    {
        _playerScores.Clear();
        Debug.Log("[ScoreManager] All scores reset.");
    }

    /// <summary>Returns a copy of the full scoreboard dictionary (playerId → wins).</summary>
    public Dictionary<int, int> GetScoreboard() => new(_playerScores);
}
