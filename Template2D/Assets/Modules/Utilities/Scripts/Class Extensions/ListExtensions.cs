/* --------------------------
 *
 * ListExtensions.cs
 *
 * Description: 
 *
 * Author: Jeremy Smellie
 *
 * Editors:
 *
 * 5/25/2015 - Starvoxel
 *
 * All rights reserved.
 *
 * -------------------------- */

#region Includes
#region Unity Includes
using UnityEngine;
#endregion

#region System Includes
using System.Collections;
using System.Collections.Generic;
#endregion

using sRandom = System.Random;
#endregion

public static class ListExtensions
{
    #region Public Methods
    public static void Shuffle<T>(this IList<T> list)
    {
        sRandom rng = new sRandom();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            list.Swap(k, n);
        }
    }

    public static void Swap<T>(this IList<T> list, int indexA, int indexB)
    {
        T value = list[indexA];
        list[indexA] = list[indexB];
        list[indexB] = value;
    }

    public static void Move<T>(this IList<T> list, int startIndex, int endIndex)
    {
        if (startIndex > 0 && startIndex < list.Count && endIndex >= 0 && endIndex < list.Count)
        {
            T value = list[startIndex];
            list.RemoveAt(startIndex);

            if (endIndex > startIndex)
            {
                endIndex -= 1;
            }

            list.Insert(endIndex, value);
        }
    }
    #endregion
}
