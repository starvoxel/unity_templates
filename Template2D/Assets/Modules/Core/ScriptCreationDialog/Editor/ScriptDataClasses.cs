/* --------------------------
 *
 * ScriptDataClasses.cs
 *
 * Description: All data describing the new script
 *
 * Author: Jeremy Smellie
 *
 * Editors:
 *
 * 3/5/2016 - Starvoxel
 *
 * All rights reserved.
 *
 * -------------------------- */

#region Includes
#region System Includes
using System.Collections;
using System.Collections.Generic;
#endregion

#region Other Includes

#endregion
#endregion

namespace Starvoxel.Core
{
    public enum Language
    {
        CSharp = 0,
        JavaScript = 1,
    }

    public struct FunctionData
    {
        public string name;
        public string returnType;
        public string returnDefault;
        public bool isVirtual;
        public ParameterData[] parameters;
        public string comment;
        public bool include;

        public FunctionData(string headerName)
        {
            comment = headerName;
            name = null;
            returnType = null;
            returnDefault = null;
            isVirtual = false;
            parameters = null;
            include = false;
        }
    }

    public struct ParameterData
    {
        public string name;
        public string type;

        public ParameterData(string name, string type)
        {
            this.name = name;
            this.type = type;
        }
    }

    [System.Serializable]
	public class ScriptData
    {
        #region Fields & Properties
        //enums
	
		//public
        public string ClassName = string.Empty;
        public string Namespace = string.Empty;
        public Language Lang;
        public string Template;
        public FunctionData[] Functions;
        public Dictionary<string, string> StringReplacements = new Dictionary<string, string>();
	
		//protected
	
		//private
	
		//properties
		#endregion
	
		#region Constructor Methods
		public ScriptData()
		{
			
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