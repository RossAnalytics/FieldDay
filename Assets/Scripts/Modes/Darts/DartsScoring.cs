using UnityEngine;

/// <summary>
/// Pure scoring calculator for the Darts mode.
/// Given a world-space hit point on the dartboard, returns the point value.
///
/// Dartboard zones (distance from bullseye in metres — adjust to match your board mesh):
///   0.00 – 0.006  → Bullseye (50 pts)
///   0.006 – 0.016 → Bull     (25 pts)
///   0.016 – 0.095 → Single zone (numbered segments 1–20)
///   0.095 – 0.105 → Treble ring (3× segment value)
///   0.105 – 0.162 → Single zone (outer)
///   0.162 – 0.172 → Double ring (2× segment value) — used for double-out win condition
///   > 0.172       → Miss (0 pts)
///
/// Segment angles: the board is divided into 20 × 18° segments.
/// Segment values clockwise from top: 20,1,18,4,13,6,10,15,2,17,3,19,7,16,8,11,14,9,12,5
/// </summary>
public class DartsScoring : MonoBehaviour
{
    // Segment values clockwise starting from the top (20 o'clock position)
    private static readonly int[] SegmentValues = { 20,1,18,4,13,6,10,15,2,17,3,19,7,16,8,11,14,9,12,5 };

    [Header("Board Geometry (metres from bullseye)")]
    [SerializeField] private float bullseyeRadius = 0.006f;
    [SerializeField] private float bullRadius      = 0.016f;
    [SerializeField] private float trebleInner     = 0.095f;
    [SerializeField] private float trebleOuter     = 0.105f;
    [SerializeField] private float doubleInner     = 0.162f;
    [SerializeField] private float doubleOuter     = 0.172f;

    /// <summary>
    /// Calculate the score for a dart landing at <paramref name="hitPoint"/>.
    /// </summary>
    /// <param name="hitPoint">World-space position of dart impact.</param>
    /// <param name="boardCenter">World-space centre of the dartboard (bullseye).</param>
    /// <param name="isDouble">Out: true if the dart landed in a double ring.</param>
    /// <returns>Points scored (0 = miss).</returns>
    public int CalculateScore(Vector3 hitPoint, Vector3 boardCenter, out bool isDouble)
    {
        isDouble = false;

        // Project onto board plane (assume board faces +Z, ignore Z depth)
        Vector2 delta = new Vector2(hitPoint.x - boardCenter.x, hitPoint.y - boardCenter.y);
        float dist    = delta.magnitude;

        // Bullseye / Bull
        if (dist <= bullseyeRadius) return 50;
        if (dist <= bullRadius)     return 25;

        // Miss
        if (dist > doubleOuter) return 0;

        // Determine segment (0–19) from angle
        // Angle 0° = top of board (+Y), increases clockwise
        float angle = Mathf.Atan2(delta.x, delta.y) * Mathf.Rad2Deg; // [-180, 180]
        if (angle < 0) angle += 360f;                                 // [0, 360]

        // Each segment = 18°; offset by 9° so segment 0 (value 20) is centred at top
        int segIndex = Mathf.FloorToInt(((angle + 9f) % 360f) / 18f) % 20;
        int segValue = SegmentValues[segIndex];

        // Multiplier rings
        if (dist >= trebleInner && dist <= trebleOuter) return segValue * 3;
        if (dist >= doubleInner && dist <= doubleOuter) { isDouble = true; return segValue * 2; }

        return segValue; // single
    }
}
