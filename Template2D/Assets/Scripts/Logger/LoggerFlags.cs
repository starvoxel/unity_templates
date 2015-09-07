/* --------------------------
 *
 * LoggerFlags.cs
 *
 * Description: Generated class for flags used in the Logger class.
 *
 * Starvoxel
 *
 * All rights reserved.
 *
 * WARNING: THIS IS A GENERATED CLASS.  ALL MANUAL CHANGES IN HERE WILL BE OVERWRITTEN WHEN RE-GENERATED.
 * TO ADD FLAGS TO THIS FILE, PLEASE CREATE A PROPER .lfi FILE WITH THE FLAG NAME THAT YOU WOULD LIKE.
 *
 * -------------------------- */

#region Includes
#region System Includes
using System;
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
        public static readonly eLoggerFlags CORE = new eLoggerFlags(1 << 3);
        public static readonly eLoggerFlags NEW_SCRIPT_DIALOG = new eLoggerFlags(1 << 4);

        public static readonly eLoggerFlags[] CUSTOM_FLAGS = new eLoggerFlags[] { CORE, NEW_SCRIPT_DIALOG};
		
        static partial void GetPartialEnumValues(ref eLoggerFlags[] flags)
        {
            List<eLoggerFlags> flagList = new List<eLoggerFlags>(flags);

            flagList.AddRange(CUSTOM_FLAGS);

            flags = flagList.ToArray();
        }
    }
}