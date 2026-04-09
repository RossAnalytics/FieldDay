using Fusion;
using UnityEngine;

/// <summary>
/// Goal trigger — notifies SoccerGameMode when the ball crosses the goal line.
///
/// TODO:
///   1. Add a large trigger collider spanning the goal mouth.
///   2. Assign the correct scoring team in the Inspector.
///   3. Play goal celebration VFX/SFX.
/// </summary>
public class SoccerGoal : NetworkBehaviour
{
    [SerializeField] private TeamManager.Team scoringTeam; // which team scores when ball enters here

    private SoccerGameMode _gameMode;

    public override void Spawned()
        => _gameMode = FindObjectOfType<SoccerGameMode>();

    private void OnTriggerEnter(Collider other)
    {
        if (!Object.HasStateAuthority) return;
        if (other.CompareTag("SoccerBall"))
            _gameMode?.RecordGoal(scoringTeam);
    }
}
