using UnityEngine;

/// <summary>
/// Utility methods shared across all sport modes for common physics calculations.
/// All methods are static — no MonoBehaviour needed.
/// </summary>
public static class PhysicsHelper
{
    /// <summary>
    /// Calculate the launch velocity needed to hit a target position using projectile motion.
    /// Assumes no air resistance. Useful for basketball shots, golf chips, etc.
    /// </summary>
    /// <param name="origin">Launch position.</param>
    /// <param name="target">Target position.</param>
    /// <param name="angle">Launch angle above horizontal in degrees.</param>
    /// <returns>Launch velocity vector, or Vector3.zero if the shot is impossible.</returns>
    public static Vector3 CalculateLaunchVelocity(Vector3 origin, Vector3 target, float angle)
    {
        float   g     = Mathf.Abs(Physics.gravity.y);
        Vector3 dir   = target - origin;
        float   yDiff = dir.y;
        float   flat  = new Vector2(dir.x, dir.z).magnitude;

        float angleRad = angle * Mathf.Deg2Rad;
        float cosA     = Mathf.Cos(angleRad);
        float sinA     = Mathf.Sin(angleRad);

        float denom = 2f * cosA * cosA * (flat * Mathf.Tan(angleRad) - yDiff);
        if (denom <= 0f)
        {
            Debug.LogWarning("[PhysicsHelper] CalculateLaunchVelocity: impossible trajectory.");
            return Vector3.zero;
        }

        float speed = Mathf.Sqrt(g * flat * flat / denom);

        Vector3 flatDir = new Vector3(dir.x, 0f, dir.z).normalized;
        return flatDir * speed * cosA + Vector3.up * speed * sinA;
    }

    /// <summary>
    /// Returns true if the Rigidbody has effectively stopped moving.
    /// </summary>
    /// <param name="rb">The Rigidbody to check.</param>
    /// <param name="threshold">Speed threshold in m/s (default 0.05).</param>
    public static bool HasSettled(Rigidbody rb, float threshold = 0.05f)
        => rb != null && rb.linearVelocity.magnitude < threshold && rb.angularVelocity.magnitude < threshold;

    /// <summary>
    /// Apply an explosive radial force to all Rigidbodies within a radius.
    /// Useful for pins, billiards cluster breaks, etc.
    /// </summary>
    /// <param name="centre">World-space origin of the explosion.</param>
    /// <param name="force">Peak force in Newtons.</param>
    /// <param name="radius">Radius of effect in metres.</param>
    public static void RadialImpulse(Vector3 centre, float force, float radius)
    {
        Collider[] hits = Physics.OverlapSphere(centre, radius);
        foreach (var col in hits)
        {
            if (col.attachedRigidbody != null)
                col.attachedRigidbody.AddExplosionForce(force, centre, radius, 0.5f, ForceMode.Impulse);
        }
    }
}
