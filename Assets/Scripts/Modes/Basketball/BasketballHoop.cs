using Fusion;
using UnityEngine;

/// <summary>
/// Detects when a basketball passes through the hoop and notifies the game mode.
///
/// Setup:
///   - Add two trigger colliders (top of rim, just below rim) to form a "shot detector".
///   - Ball must pass through the top trigger first, then the bottom, in order = valid basket.
///
/// TODO:
///   1. Add a NetTriggerZone or two Trigger Colliders on the hoop prefab.
///   2. Track ball entry direction to distinguish a valid basket from a ball bouncing out.
///   3. Determine point value (2 or 3) and call BasketballGameMode.RecordScore().
/// </summary>
public class BasketballHoop : NetworkBehaviour
{
    [SerializeField] private int pointValue = 2; // 2 or 3 pts — set per hoop in scene

    private BasketballGameMode _gameMode;

    public override void Spawned()
        => _gameMode = FindObjectOfType<BasketballGameMode>();

    private void OnTriggerEnter(Collider other)
    {
        if (!Object.HasStateAuthority) return;
        if (!other.CompareTag("Basketball")) return;

        // TODO: verify ball came from above (direction check)
        var ball = other.GetComponent<NetworkObject>();
        if (ball != null)
        {
            PlayerRef scorer = ball.InputAuthority;
            _gameMode?.RecordScore(scorer, pointValue);
        }
    }
}
