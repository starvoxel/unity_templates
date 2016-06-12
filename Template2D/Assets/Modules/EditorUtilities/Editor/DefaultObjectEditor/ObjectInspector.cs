/* --------------------------
 *
 * ObjectInspector.cs
 *
 * Description: Abstract base to every Object inspector that we might want.
 *
 * Author: Jeremy Smellie
 *
 * Editors:
 *
 * 9/5/2015 - Starvoxel
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

namespace Starvoxel.EditorUtilities
{
    public abstract class ObjectInspector
	{
		#region Fields & Properties
		//const
	
		//public
	
		//protected
        protected ObjectEditor m_Editor;
	
		//private
	
		//properties
        public abstract GUIContent TabContent
        {
            get;
        }
		#endregion
	
		#region Public Methods
        public abstract bool IsValid(string path);

        public virtual void Initialize(ObjectEditor editor)
        {
            m_Editor = editor;
        }

        public abstract void OnInspector();
		#endregion
	
		#region Protected Methods
		#endregion
	
		#region Private Methods
		#endregion
	}
}