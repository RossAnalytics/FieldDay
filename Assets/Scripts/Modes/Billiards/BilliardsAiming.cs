using Fusion;
using UnityEngine;

/// <summary>
/// Cue aiming and shot power mechanic for Billiards.
///
/// TODO:
///   1. Render a cue stick that orbits the cue ball based on mouse/stick input.
///   2. Show a dotted trajectory line (reflect off cushions if possible).
///   3. Implement a power-bar (hold Space/button, release to shoot).
///   4. On release, apply force to the cue ball via its Rigidbody.
///   5. End the turn (RPC_EndCurrentTurn) once all balls have stopped moving.
/// </summary>
public class BilliardsAiming : MonoBehaviour
{
    [Header("Aim Settings")]
    [SerializeField] private float maxShotPower = 20f;
    [SerializeField] private float chargeRate   = 10f; // power per second

    private float _chargedPower;
    private bool  _isCharging;

    [Header("References")]
    [SerializeField] private Rigidbody cueBall;

    public bool IsActive { get; set; }

    private void Update()
    {
        if (!IsActive) return;

        // TODO: replace with Input System actions
        if (Input.GetKeyDown(KeyCode.Space)) _isCharging = true;

        if (_isCharging)
            _chargedPower = Mathf.Min(_chargedPower + chargeRate * Time.deltaTime, maxShotPower);

        if (Input.GetKeyUp(KeyCode.Space))
        {
            ExecuteShot();
            _isCharging   = false;
            _chargedPower = 0f;
        }
    }

    private void ExecuteShot()
    {
        if (cueBall == null) return;

        // Direction from cue to cue ball (rotate based on mouse/stick — TODO)
        Vector3 shotDir = transform.forward;
        cueBall.AddForce(shotDir * _chargedPower, ForceMode.Impulse);

        Debug.Log($"[BilliardsAiming] Shot with power {_chargedPower:F1}");
    }
}
