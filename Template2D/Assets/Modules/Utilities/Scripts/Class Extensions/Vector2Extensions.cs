/* --------------------------
 *
 * Vector2Extensions.cs
 *
 * Description: Extension methods for Vector2
 *
 * Author: Jeremy Smellie
 *
 * Editors:
 *
 * 8/29/2015 - Starvoxel
 *
 * All rights reserved.
 *
 * -------------------------- */

#region Includes
#region Unity Includes
using UnityEngine;
#endregion
#endregion

namespace Starvoxel.Utilities
{
    public static class Vector2Extension
    {
        public static Vector2 Rotate(this Vector2 v, float degrees)
        {
            float radians = degrees * Mathf.Deg2Rad;
            float sin = Mathf.Sin(radians);
            float cos = Mathf.Cos(radians);

            float tx = v.x;
            float ty = v.y;

            return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
        }
    }
}
