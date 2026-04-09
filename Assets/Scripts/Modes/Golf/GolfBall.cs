using Fusion;
using UnityEngine;

/// <summary>
/// Networked golf ball.
///
/// TODO:
///   1. Attach Rigidbody + sphere collider.
///   2. Call Launch() with club-head direction and power.
///   3. Apply backspin / topspin torque for realism.
///   4. Detect when ball enters the hole cup trigger → call GolfGameMode.OnBallInCup().
///   5. Detect OOB (out of bounds) → add penalty stroke and respawn at last position.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class GolfBall : NetworkBehaviour
{
    [SerializeField] private float maxLaunchForce = 30f;

    private Rigidbody _rb;
    private GolfGameMode _gameMode;
    private PlayerRef    _owner;

    public override void Spawned()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Launch(Vector3 direction, float power, float loft, PlayerRef owner, GolfGameMode mode)
    {
        if (!Object.HasStateAuthority) return;

        _owner    = owner;
        _gameMode = mode;

        Vector3 launchDir = Quaternion.AngleAxis(-loft, Vector3.right) * direction;
        _rb.AddForce(launchDir.normalized * (maxLaunchForce * power), ForceMode.Impulse);

        _gameMode?.RecordStroke(_owner);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HoleCup"))
            _gameMode?.OnBallInCup(_owner);
    }
}
