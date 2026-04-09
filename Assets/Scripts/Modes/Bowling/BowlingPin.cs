using UnityEngine;

/// <summary>
/// Individual bowling pin.
/// Detects when it has been knocked down via collision impulse threshold.
///
/// TODO:
///   1. Place 10 instances in the pin formation (standard triangle arrangement).
///   2. Assign a Rigidbody + capsule/box collider.
///   3. On game reset (new frame), call Reset() to stand the pin back up.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class BowlingPin : MonoBehaviour
{
    [Header("Knock Detection")]
    [SerializeField] private float knockImpulseThreshold = 2f; // N*s minimum to count as knocked

    private Rigidbody  _rb;
    private Vector3    _startPosition;
    private Quaternion _startRotation;

    public bool IsDown { get; private set; }

    private void Awake()
    {
        _rb            = GetComponent<Rigidbody>();
        _startPosition = transform.position;
        _startRotation = transform.rotation;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsDown) return;

        // Check impulse magnitude to filter out gentle grazes
        if (collision.impulse.magnitude >= knockImpulseThreshold)
        {
            IsDown = true;
            Debug.Log($"[BowlingPin] {gameObject.name} knocked down.");
            // TODO: play knock SFX / VFX
        }
    }

    /// <summary>Stand the pin back up at its original position.</summary>
    public void Reset()
    {
        IsDown = false;
        _rb.linearVelocity  = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        transform.SetPositionAndRotation(_startPosition, _startRotation);
    }
}
