/* --------------------------
 *
 * LoggerFlag.cs
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
#endregion

#region Other Includes

#endregion
#endregion

namespace Starvoxel
{
    public partial class LoggerSettings
    {
        public enum eLoggerFlags
        {
            DEFAULT = 1 << 0,
            Module = 1 << 1,
            Core = 1 << 2,
            NewScriptDialog = 1 << 3,
            JustOneMoreTest = 1 << 4,
            Pooling = 1 << 5,
            SomeTestThing = 1 << 6,
        }
    }
}