/* --------------------------
 *
 * EventMessenger.cs
 *
 * Description: Simple type-safe event messenger.
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
using System;
using System.Collections.Generic;
#endregion

#region Other Includes

#endregion
#endregion

namespace Starvoxel.Core
{
    /// <summary>
    /// Base class of all events.  Simple inherite from it and put in any information you want.
    /// </summary>
    public class GameEvent { }

    public class EventMessenger
    {
        #region Fields & Properties
        //public
        public delegate void EventDelegate<T>(T ev) where T : GameEvent;

        //protected

        //private
        private static EventMessenger m_Instance;

        private readonly Dictionary<Type, Delegate> m_Delegates = new Dictionary<Type, Delegate>();
        //properties
        public static EventMessenger Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new EventMessenger();
                }

                return m_Instance;
            }
        }
        #endregion

        #region Public Methods
        public void AddListener<T>(EventDelegate<T> listener) where T : GameEvent
        {
            Delegate del;
            if (m_Delegates.TryGetValue(typeof(T), out del))
            {
                m_Delegates[typeof(T)] = Delegate.Combine(del, listener);
            }
            else
            {
                m_Delegates[typeof(T)] = listener;
            }
        }

        public void RemoveListener<T>(EventDelegate<T> listener) where T : GameEvent
        {
            Delegate del;
            if (m_Delegates.TryGetValue(typeof(T), out del))
            {
                Delegate currentDel = Delegate.Remove(del, listener);

                if (currentDel == null)
                {
                    m_Delegates.Remove(typeof(T));
                }
                else
                {
                    m_Delegates[typeof(T)] = currentDel;
                }
            }
        }

        public void Raise<T>(T ev) where T : GameEvent
        {
            if (ev == null)
            {
                throw new ArgumentNullException("ev");
            }

            Delegate del;
            if (m_Delegates.TryGetValue(typeof(T), out del))
            {
                EventDelegate<T> callback = del as EventDelegate<T>;
                if (callback != null)
                {
                    callback(ev);
                }
            }
        }
        #endregion
    }

}