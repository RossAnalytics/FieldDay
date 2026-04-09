using Fusion;
using UnityEngine;

/// <summary>
/// Networked basketball.
///
/// TODO:
///   1. Attach Rigidbody + sphere collider.
///   2. Implement shot arc: apply upward angle on launch.
///   3. Detect swish vs. bank shot using rim collision data.
///   4. Reset ball to player's hands between shots.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class BasketballBall : NetworkBehaviour
{
    [SerializeField] private float launchForce = 15f;
    [SerializeField] private float arcHeight   = 45f; // degrees above horizontal

    private Rigidbody _rb;

    public override void Spawned() => _rb = GetComponent<Rigidbody>();

    public void Shoot(Vector3 targetPosition, float power)
    {
        if (!Object.HasStateAuthority) return;

        Vector3 dir = (targetPosition - transform.position).normalized;
        dir = Quaternion.AngleAxis(-arcHeight, Vector3.Cross(dir, Vector3.up)) * dir;

        _rb.AddForce(dir * (launchForce * power), ForceMode.Impulse);
        Debug.Log("[BasketballBall] Shot!");
    }
}
