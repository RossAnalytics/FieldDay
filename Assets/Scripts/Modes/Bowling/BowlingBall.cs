using Fusion;
using UnityEngine;

/// <summary>
/// Networked bowling ball.
/// The host applies physics; all clients receive interpolated position via Fusion.
///
/// TODO:
///   1. Attach a Rigidbody and sphere collider sized to match your ball mesh.
///   2. Call Launch() from BowlingGameMode.OnTurnStart() with direction + power.
///   3. Detect when the ball has stopped (velocity below threshold) and call
///      BowlingGameMode.RecordRoll() with the pin count.
///   4. Reset the ball to the approach area at the start of each turn.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class BowlingBall : NetworkBehaviour
{
    [Header("Physics")]
    [SerializeField] private float maxLaunchForce = 20f;

    [Header("State Detection")]
    [SerializeField] private float settleSpeedThreshold = 0.1f; // m/s below which ball is "stopped"
    [SerializeField] private float settleDelay          = 1.5f; // seconds to wait after slowing

    private Rigidbody _rb;
    private bool      _launched;
    private float     _settleTimer;

    private BowlingGameMode _gameMode;
    private PlayerRef       _owner;

    public override void Spawned()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true; // static until Launch() is called
    }

    /// <summary>
    /// Launch the ball down the lane.
    /// </summary>
    /// <param name="direction">Normalised direction vector toward the pins.</param>
    /// <param name="power">0–1 power multiplier.</param>
    /// <param name="owner">Player who threw this ball.</param>
    public void Launch(Vector3 direction, float power, PlayerRef owner, BowlingGameMode mode)
    {
        if (!Object.HasStateAuthority) return;

        _owner    = owner;
        _gameMode = mode;
        _launched = true;

        _rb.isKinematic = false;
        _rb.AddForce(direction * (maxLaunchForce * power), ForceMode.Impulse);
    }

    public override void FixedUpdateNetwork()
    {
        if (!_launched || !Object.HasStateAuthority) return;

        if (_rb.linearVelocity.magnitude < settleSpeedThreshold)
        {
            _settleTimer += Runner.DeltaTime;
            if (_settleTimer >= settleDelay)
            {
                _launched = false;
                NotifyRollComplete();
            }
        }
        else
        {
            _settleTimer = 0f;
        }
    }

    private void NotifyRollComplete()
    {
        // TODO: count knocked pins and pass the value here
        int pinsKnocked = CountKnockedPins();
        _gameMode?.RecordRoll(_owner, pinsKnocked);
    }

    private int CountKnockedPins()
    {
        // TODO: query BowlingPin.IsDown in the scene and count trues
        return 0; // placeholder
    }
}
