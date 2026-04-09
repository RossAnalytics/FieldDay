using UnityEngine;

/// <summary>
/// Handles the shot aiming UI and input for Golf.
///
/// TODO:
///   1. Show a power bar (hold and release) and directional arrow.
///   2. Optional: add a swing timing minigame (click at the right moment for accuracy).
///   3. On shot confirmed, call GolfBall.Launch() with computed direction/power/loft.
/// </summary>
public class GolfShot : MonoBehaviour
{
    [Header("Shot Settings")]
    [SerializeField] private float maxPower = 1f;
    [SerializeField] private float chargeRate = 0.5f; // power per second while holding

    private float _currentPower;
    private bool  _isCharging;

    private void Update()
    {
        // TODO: replace with Input System actions
        if (Input.GetKeyDown(KeyCode.Space)) _isCharging = true;

        if (_isCharging)
            _currentPower = Mathf.Min(_currentPower + chargeRate * Time.deltaTime, maxPower);

        if (Input.GetKeyUp(KeyCode.Space))
        {
            _isCharging = false;
            Debug.Log($"[GolfShot] Shot with power {_currentPower:F2}");
            // TODO: call GolfBall.Launch(...)
            _currentPower = 0f;
        }
    }
}
