using UnityEngine;

namespace CustomNamespace.Extensions
{
    /// <summary>
    /// Provides extension methods for the <see cref="Quaternion"/> structure.
    /// </summary>
    public static class QuaternionExtensions
    {
        /// <summary>
        /// Decomposes a <see cref="Quaternion"/> into its swing and twist components around a specified twist axis.
        /// </summary>
        /// <param name="q">The <see cref="Quaternion"/> to decompose.</param>
        /// <param name="twistAxis">The axis around which the twist rotation occurs.</param>
        /// <param name="swing">When this method returns, contains the swing <see cref="Quaternion"/>.</param>
        /// <param name="twist">When this method returns, contains the twist <see cref="Quaternion"/>.</param>
        public static void SwingTwist(this Quaternion q, Vector3 twistAxis, out Quaternion swing, out Quaternion twist)
        {
            Vector3 r = new Vector3(q.x, q.y, q.z);

            // singularity: rotation by 180 degree
            if (r.sqrMagnitude < float.Epsilon)
            {
                Vector3 rotatedTwistAxis = q * twistAxis;
                Vector3 swingAxis = Vector3.Cross(twistAxis, rotatedTwistAxis);

                if (swingAxis.sqrMagnitude > float.Epsilon)
                {
                    float swingAngle =
                        Vector3.Angle(twistAxis, rotatedTwistAxis);
                    swing = Quaternion.AngleAxis(swingAngle, swingAxis);
                }
                else
                {
                    swing = Quaternion.identity; // no swing
                }

                // always twist 180 degree on singularity
                twist = Quaternion.AngleAxis(180.0f, twistAxis);
                return;
            }

            // meat of swing-twist decomposition
            Vector3 p = Vector3.Project(r, twistAxis);
            twist = new Quaternion(p.x, p.y, p.z, q.w);
            twist = Quaternion.Normalize(twist);
            swing = q * Quaternion.Inverse(twist);
        }
    }
}