/* --------------------------
 *
 * ServiceTester.cs
 *
 * Description: Testing class for the service pattern.
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
#region Unity Includes
using UnityEngine;
#endregion

#region System Includes
using System.Collections;
#endregion

#region Other Includes

#endregion
#endregion

 namespace Starvoxel.Core
{
	public class ServiceTester : CustomMono
	{
		#region Fields & Properties
		//const
	
		//public
	
		//protected
	
		//private
	
		//properties
		#endregion
	
		#region Unity Methods
        protected void Awake()
        {
            Services.InitializeLogger(new TextFileLogger(true));
            Logger logger = Services.Logger;
            
            //Testing the category stuff
            logger.ClearActiveCategories();
            logger.AddCategories(LoggerConstants.CORE_CATEGORY, LoggerConstants.GAME_CATEGORY, LoggerConstants.INPUT_CATEGORY);
            logger.LogWithCategory(LoggerConstants.CORE_CATEGORY, LogType.Log, "Core Category test");
            logger.LogWithCategory(LoggerConstants.FLOW_CATEGORY, LogType.Log, "Flow Category test");
            logger.LogWithCategory(LoggerConstants.GAME_CATEGORY, LogType.Log, "Game Category test");
            logger.LogWithCategory(LoggerConstants.INPUT_CATEGORY, LogType.Log, " Input Category test");
            System.Collections.Generic.List<string> categories = new System.Collections.Generic.List<string>();
            categories.Add(LoggerConstants.INPUT_CATEGORY);
            categories.Add(LoggerConstants.FLOW_CATEGORY);
            logger.LogWithCategories(categories, LogType.Log, "Input & Flow Category test");
        }
		#endregion
	
		#region Public Methods
		#endregion
	
		#region Protected Methods
		#endregion
	
		#region Private Methods
		#endregion
	}
	
}
