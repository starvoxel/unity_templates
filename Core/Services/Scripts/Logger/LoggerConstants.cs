/* --------------------------
 *
 * LoggerConstants.cs
 *
 * Description: All constants that are needed by the various logger classes.
 * This is a partial class so implement it how ever you'd like in the game to add additional categories and such.
 *
 * Author: Jeremy Smellie
 *
 * Editors:
 *
 * 12/5/2015 - Starvoxel
 *
 * All rights reserved.
 *
 * -------------------------- */

#region Includes
#region System Includes
using System.Collections;
#endregion

#region Other Includes

#endregion
#endregion

 namespace Starvoxel.Core
{
	public static partial class LoggerConstants
    {
        #region Categories
        // Module related
        public const string CORE_CATEGORY = "CORE";
        public const string FLOW_CATEGORY = "FLOW";
        public const string POOLING_CATEGORY = "POOLING";
        public const string UTILITY_CATEGORY = "UTILITY";
        public const string EDITOR_UTILITY_CATEGORY = "EDITOR_UTILITY";
        public const string TELEMETERY_CATEGORY = "TELEMETRY";

        // Game related
        public const string GAME_CATEGORY = "GAME";
        public const string INPUT_CATEGORY = "INPUT";

        // Debug related
        public const string TESTING_CATEGORY = "TESTING";

        public static string GetDefaultCategory()
        {
            string defaultCategory = TESTING_CATEGORY;
            GetPartialDefaultCategory(ref defaultCategory);
            return defaultCategory;
        }

        static partial void GetPartialDefaultCategory(ref string defaultCategory);
        #endregion

        #region IO
        public const string BASE_FOLDER_STRUCTUR = "/Logs/";
        #endregion
    }
	
}
