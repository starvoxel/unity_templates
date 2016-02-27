/* --------------------------
 *
 * Pool.cs
 *
 * Description: 
 * Basic pool that can be used in most circumstances
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
#endregion

namespace Starvoxel.Core
{
    public class Pool<T> where T : IPoolable
    {
        #region Fields & Properties
        //const

        //public

        //protected
        protected List<T> m_Pool;
        protected T m_Original;

        protected int m_ActiveIndex;
        //private

        //properties
        public int Size
        {
            get { return m_Pool.Count; }
        }

        public int ActiveCount
        {
            get { return m_ActiveIndex + 1; }
        }

        public int DeactiveCount
        {
            get { return this.Size - this.ActiveCount; }
        }
        #endregion

        #region Constructors
        public Pool(T original)
        {
            m_Original = original;
            m_Pool = new List<T>();
            m_ActiveIndex = -1;
        }

        public Pool(T original, int startingCount)
        {
            m_Original = original;
            m_Pool = new List<T>(startingCount);
            m_ActiveIndex = -1;

            for (int curElement = 0; curElement < startingCount; ++curElement)
            {
                CreateInstance();
            }
        }
        #endregion

        #region Deconstructors
        ~Pool()
        {
            for (int index = 0; index < m_Pool.Count; ++index)
            {
                m_Pool[index].DeactivateAction -= OnInstanceDeactivate;
                m_Pool[index].OnRemoved();
            }

            m_Pool.Clear();
        }
        #endregion

        #region Public Methods
        public T NextInstance()
        {
            return ActivateInstance();
        }

        public void Deactivate(T instance)
        {
            if (m_Pool.Contains(instance))
            {
                int index = m_Pool.IndexOf(instance);

                if (index >= m_Pool.Count)
                {
                    Services.Logger.LogError("Object at invalid index trying to be deactivated.\nIndex: " + index);
                }
                else if (index > m_ActiveIndex)
                {
                    Services.Logger.LogWarning("Trying to de-activate an object that is already in-active.");
                }
                else
                {
                    instance.OnDeactivate();
                    instance.Reset();
                    m_ActiveIndex -= 1;
                    m_Pool.Move(index, m_Pool.Count - 1);
                }
            }
        }
        #endregion

        #region Protected Methods
        protected T ActivateInstance()
        {
            T activeInstance;

            if (m_ActiveIndex == m_Pool.Count - 1)
            {
                CreateInstance();
            }

            m_ActiveIndex += 1;

            activeInstance = m_Pool[m_ActiveIndex];

            activeInstance.OnActivate();

            return activeInstance;
        }

        protected void CreateInstance()
        {
            IPoolable newInstance = m_Original.Clone();

            if (newInstance.GetType() == typeof(T))
            {
                T castedInstance = (T)newInstance;

                castedInstance.DeactivateAction += OnInstanceDeactivate;
                castedInstance.Reset();

                m_Pool.Add(castedInstance);
            }
        }

        protected void OnInstanceDeactivate(IPoolable instance)
        {
            if (instance.GetType() == typeof(T))
            {
                T castedInstance = (T)instance;

                Deactivate(castedInstance);
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Basic Overrides
        public override string ToString()
        {
            return "{S: " + this.Size + "| A: " + this.ActiveCount + "| D: " + this.DeactiveCount + "}";
        }
        #endregion
    }
}
