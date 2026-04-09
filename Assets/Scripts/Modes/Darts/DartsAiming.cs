using Fusion;
using UnityEngine;

/// <summary>
/// Handles the aiming and throwing mechanic for Darts.
///
/// Mechanic:
///   - A crosshair/reticle oscillates across the dartboard (sin wave, increasing speed).
///   - Player presses Throw at the right moment to stop the reticle — position = aim point.
///   - A second power bar determines throw strength (affects deviation from aim point).
///   - Result is a world-space hit position passed to DartsGameMode.RegisterThrow().
///
/// TODO:
///   1. Add a UI canvas with reticle image and power bar slider.
///   2. Hook into Unity's new Input System for Throw action.
///   3. Calculate dart landing position from reticle + power deviation.
///   4. Animate dart flying toward the board (optional).
/// </summary>
public class DartsAiming : NetworkBehaviour
{
    [Header("Reticle Settings")]
    [SerializeField] private float reticleSpeed     = 1f;   // base oscillation speed
    [SerializeField] private float reticleAmplitude = 0.5f; // oscillation width in metres
    [SerializeField] private float speedIncrement   = 0.1f; // speed added each dart thrown

    [Header("Power Bar")]
    [SerializeField] private float powerBarSpeed = 1f; // power bar fill rate

    [Header("References")]
    [SerializeField] private Transform dartboardCenter; // world position of bullseye

    private float _reticleAngle;
    private float _powerBarValue; // 0–1
    private bool  _waitingForThrow;
    private int   _dartsThrown;

    public bool IsActive { get; set; } // set by DartsGameMode on turn start/end

    private void Update()
    {
        if (!IsActive || !Object.HasInputAuthority) return;

        // Oscillate reticle
        _reticleAngle += Time.deltaTime * (reticleSpeed + _dartsThrown * speedIncrement);
        float reticleOffset = Mathf.Sin(_reticleAngle) * reticleAmplitude;

        // Fill power bar (ping-pong)
        _powerBarValue = Mathf.PingPong(Time.time * powerBarSpeed, 1f);

        // TODO: update UI elements to reflect _reticleOffset and _powerBarValue

        // Throw input (replace with Input System action)
        if (Input.GetKeyDown(KeyCode.Space))
            ExecuteThrow(reticleOffset, _powerBarValue);
    }

    private void ExecuteThrow(float reticleOffset, float power)
    {
        // Convert reticle offset + power into a board hit position
        // Low power = large random deviation; high power = accurate
        float deviation = (1f - power) * 0.15f; // max 15 cm deviation
        Vector3 hitPoint = dartboardCenter.position
                         + new Vector3(reticleOffset, UnityEngine.Random.Range(-deviation, deviation), 0);

        // TODO: spawn dart projectile, animate flight, then call RegisterThrow
        Debug.Log($"[DartsAiming] Dart thrown — hit point: {hitPoint}");
        _dartsThrown++;

        // Notify game mode (find via singleton or inject reference)
        FindObjectOfType<DartsGameMode>()?.RegisterThrow(Object.InputAuthority, 0 /* scoring TBD */);
    }
}
