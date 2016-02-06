/* --------------------------
 *
 * FlowEvents.cs
 *
 * Description: All events related to the app flow
 *
 * Author: Jeremy Smellie
 *
 * Editors:
 *
 * 2/6/2016 - Starvoxel
 *
 * All rights reserved.
 *
 * -------------------------- */

#region Includes
#region System Includes
using System.Collections.Generic;
#endregion

#region Other Includes
using Starvoxel.Core;
#endregion
#endregion

 namespace Starvoxel.FlowManagement
{
	public class FlowEvent : GameEvent
	{
		#region Fields & Properties
		//const
	
		//public
	    public readonly string ActionID;

        public readonly Dictionary<string, object> Parameters;
		#endregion
	
		#region Constructor Methods
		public FlowEvent(string actionID, Dictionary<string, object> parameters = null)
		{
            ActionID = actionID;
            Parameters = parameters;
		}
		#endregion
	}
	
}