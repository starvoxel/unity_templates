/* --------------------------
 *
 * IPoolable.cs
 *
 * Description: 
 * Interface for all poolable objects
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
#endregion
#endregion

namespace Starvoxel.Core
{
    public interface IPoolable
    {
        #region Properties
        Action<IPoolable> DeactivateAction { set; get; }
        #endregion

        #region Public Methods
        IPoolable Clone();

        void OnRemoved();

        void OnActivate();
        void OnDeactivate();
        void Reset();
        #endregion
    }
}
