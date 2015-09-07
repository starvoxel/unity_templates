/* --------------------------
 *
 * LoggerFlagsBase.cs
 *
 * Description: Default implementation for all the logger flags
 *
 * Author: Jeremy Smellie
 *
 * Editors:
 *
 * 8/30/2015 - Starvoxel
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

#region Other Includes

#endregion
#endregion

namespace Starvoxel
{
    public partial class eLoggerFlags
    {
        #region Fields & Properties
        public int Value;

        public static readonly eLoggerFlags DEFAULT = new eLoggerFlags(1 << 0);
        public static readonly eLoggerFlags MODULE = new eLoggerFlags(1 << 1);
        public static readonly eLoggerFlags GAME = new eLoggerFlags(1 << 2);

        public static readonly eLoggerFlags[] INCLUDED_FLAGS = new eLoggerFlags[] { DEFAULT, MODULE, GAME };
        #endregion

        #region Constructors
        public eLoggerFlags(int value)
        {
            this.Value = value;
        }
        #endregion

        #region Methods
        public static eLoggerFlags GetFlagForName(string name)
        {
            string[] names = GetNames();

            eLoggerFlags[] flags = GetFlags();

            for(int i = 0; i < names.Length; ++i)
            {
                if(string.Compare(names[i], name) == 0)
                {
                    return flags[i];
                }
            }

            return null;
        }

        public static string GeNameForFlag(eLoggerFlags flag)
        {
            string[] names = GetNames();

            eLoggerFlags[] flags = GetFlags();

            for (int i = 0; i < names.Length; ++i)
            {
                if (flags[i] == flag)
                {
                    return names[i];
                }
            }

            return null;
        }

        public static eLoggerFlags[] GetFlags()
        {
            eLoggerFlags[] allFlags = INCLUDED_FLAGS;
            GetPartialEnumValues(ref allFlags);
            return allFlags;
        }

        public static string[] GetIncludedNames()
        {
            return new string[] { "DEFAULT", "MODULE", "GAME" };
        }

        public static string[] GetCustomNames()
        {
            List<string> names = new List<string>(GetNames());

            string[] includedNames = GetIncludedNames();

            for (int i = 0; i < includedNames.Length; ++i)
            {
                names.Remove(includedNames[i]);
            }

            return names.ToArray();
        }

        public static string[] GetNames()
        {
            List<string> names = new List<string>();

            System.Reflection.FieldInfo[] fieldInfo = typeof(eLoggerFlags).GetFields();

            for (int i = 0; i < fieldInfo.Length; ++i)
            {
                if (fieldInfo[i].FieldType == typeof(eLoggerFlags))
                {
                    names.Add(fieldInfo[i].Name);
                }
            }

            return names.ToArray();
        }

        static partial void GetPartialEnumValues(ref eLoggerFlags[] flags);

        #endregion
    }
}