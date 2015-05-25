/* --------------------------
 *
 * TestObject.cs
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
#region System Includes
using System;
using System.Collections;
#endregion
#endregion

namespace Utility
{
    public class TestObject : IPoolable
    {
        #region Fields & Properties
        //const

        //public
        public string Name;

        //protected

        //private
        private Action<IPoolable> m_DeactivateAction;

        //properties
        #endregion

        #region Constructors
        public TestObject()
        {
            Name = "Clone";
        }

        public TestObject(string name)
        {
            Name = name;
        }
        #endregion

        #region Public Methods
        public void Deactivate()
        {
            if (m_DeactivateAction != null)
            {
                m_DeactivateAction(this);
            }
        }
        #endregion

        #region IPoolable Implementation
        public Action<IPoolable> DeactivateAction
        {
            set { m_DeactivateAction = value; }
            get { return m_DeactivateAction; }
        }

        public IPoolable Clone()
        {
            return new TestObject();
        }

        public void OnRemoved()
        {
            //TODO: This is called when this object is no longer being pooled.  Handle clean up here
        }

        public void OnActivate()
        {
            //TODO: This is where you could do some checking for anything if you wanted something to happen as soon as it turned on
        }

        public void OnDeactivate()
        {
            //TODO: This is where you could notify things that you've been deactivated
        }


        public void Reset()
        {
            //TODO: This is where you'd do all your state and field resets
        }
        #endregion

    }
}
