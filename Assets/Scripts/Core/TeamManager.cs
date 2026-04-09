using System.Collections.Generic;
using Fusion;
using UnityEngine;

/// <summary>
/// Manages team assignments (Team A vs Team B).
/// Tracks which PlayerRefs belong to each team.
/// Extend to support more than 2 teams if needed.
/// </summary>
public class TeamManager : NetworkBehaviour
{
    public enum Team { None, A, B }

    // Fusion doesn't natively support Dictionary — store as parallel lists.
    // TODO: migrate to NetworkLinkedList or a custom data structure for full sync.

    private readonly Dictionary<PlayerRef, Team> _assignments = new();

    /// <summary>Assign a player to a team (call from server only).</summary>
    public void AssignTeam(PlayerRef player, Team team)
    {
        _assignments[player] = team;
        Debug.Log($"[TeamManager] {player} assigned to Team {team}");
    }

    /// <summary>Returns the team for a given player, or Team.None if unassigned.</summary>
    public Team GetTeam(PlayerRef player)
        => _assignments.TryGetValue(player, out var t) ? t : Team.None;

    /// <summary>Returns all players on a given team.</summary>
    public List<PlayerRef> GetTeamPlayers(Team team)
    {
        var result = new List<PlayerRef>();
        foreach (var kvp in _assignments)
            if (kvp.Value == team) result.Add(kvp.Key);
        return result;
    }
}
