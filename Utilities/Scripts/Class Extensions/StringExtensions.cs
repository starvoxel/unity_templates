/* --------------------------
 *
 * StringExtensions.cs
 *
 * Description: Extension methods for string
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
#region System Includes
using System;
using System.Linq;
using System.Collections;
#endregion

#region Other Includes

#endregion
#endregion

 namespace Starvoxel
{
	public static class StringExtensions
    {
        #region Public Methods
        public static string RemoveWhitespace(this string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }
        #endregion
    }
}