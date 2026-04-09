using Fusion;
using UnityEngine;

/// <summary>
/// Networked billiards ball (cue ball = number 0; object balls 1-15).
///
/// TODO:
///   1. Attach Rigidbody + sphere collider (radius ~0.028 m, standard pool ball).
///   2. Use Fusion's NetworkRigidbody for physics interpolation.
///   3. Detect when ball enters a pocket trigger → call BilliardsGameMode.OnBallPotted().
///   4. Cue ball (number 0) scratch: respawn on the head string.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class BilliardsBall : NetworkBehaviour
{
    [Networked] public int BallNumber { get; set; } // 0 = cue, 1-15 = object balls

    private Rigidbody         _rb;
    private BilliardsGameMode _gameMode;

    public override void Spawned()
    {
        _rb       = GetComponent<Rigidbody>();
        _gameMode = FindObjectOfType<BilliardsGameMode>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!Object.HasStateAuthority) return;
        if (other.CompareTag("Pocket"))
            _gameMode?.OnBallPotted(this, Object.LastReceivingAuthority);
    }
}
