using Fusion;
using UnityEngine;

/// <summary>
/// Networked soccer ball.
///
/// TODO:
///   1. Attach Rigidbody + sphere collider (size ~0.22 m radius).
///   2. Implement kick force — direction from player facing + input power.
///   3. Add curve/spin (Magnus effect) for banana kicks (optional).
///   4. Reset ball to kick spot between penalty kicks.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class SoccerBall : NetworkBehaviour
{
    [SerializeField] private float kickForce = 25f;

    private Rigidbody _rb;

    public override void Spawned() => _rb = GetComponent<Rigidbody>();

    /// <summary>Apply a kick impulse in the given direction.</summary>
    public void Kick(Vector3 direction, float power)
    {
        if (!Object.HasStateAuthority) return;
        _rb.AddForce(direction.normalized * (kickForce * power), ForceMode.Impulse);
    }
}
