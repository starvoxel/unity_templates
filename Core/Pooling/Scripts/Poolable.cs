/* --------------------------
 *
 * Poolable.cs
 *
 * Description: 
 * Base class for all monobehaviors that can be pooled
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
using System;
#endregion
#endregion

namespace Starvoxel.Core
{
	public abstract class Poolable : CustomMono, IPoolable
	{
		#region Fields & Properties
		//const

		//public

        //protected
        protected Action<IPoolable> m_DeactivateAction;

		//private

		//properties
		#endregion

		#region Public Methods
        public virtual void Deactivate()
        {
            if (m_DeactivateAction != null)
            {
                m_DeactivateAction(this);
            }
        }
		#endregion

        #region IPoolable Implementation
        public virtual Action<IPoolable> DeactivateAction
        {
            set { m_DeactivateAction = value; }
            get { return m_DeactivateAction; }
        }

        public abstract IPoolable Clone();

        public virtual void OnRemoved()
        {
            Destroy(this.gameObject);
        }

        public virtual void OnActivate(){}
        public virtual void OnDeactivate(){}
        public virtual void Reset(){}
        #endregion
	}
}