/* --------------------------
 *
 * BaseState.cs
 *
 * Description: Base state for all states in a state machine
 *
 * Author: Jeremy Smellie
 *
 * Editors:
 *
 * 2/7/2016 - Starvoxel
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
	public abstract class BaseState
	{	
		#region Public Methods
        public virtual void Start() { }
        public virtual void Update() { }
        public virtual void End() { }
		#endregion
	}	
}