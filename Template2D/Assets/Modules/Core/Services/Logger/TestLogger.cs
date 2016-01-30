/* --------------------------
 *
 * TestLogger.cs
 *
 * Description: Logger used for the creating of the logger system.  Will be deleted once no longer needed.
 *
 * Author: Jeremy Smellie
 *
 * Editors:
 *
 * 12/5/2015 - Starvoxel
 *
 * All rights reserved.
 *
 * -------------------------- */

#region Includes
#region System Includes
using System.Collections;
using System.Collections.Generic;
using StackTrace = System.Diagnostics.StackTrace;
using StackFrame = System.Diagnostics.StackFrame;
#endregion

#region Other Includes

#endregion
#endregion

 namespace Starvoxel.Core
{
	public class TestLogger : ILogger
	{
		#region Fields & Properties
		//const
	
		//public
	
		//protected
	
		//private
	
		//properties
		#endregion
	
		#region Constructor Methods
		#endregion

        #region Public Methods
        public void TestAllFunctions()
        {
            this.Log("Test normal log. {0}", true);
            this.LogWithCategory(LoggerConstants.CORE_CATEGORY, "Test Core category log. {0}", true);
            this.LogWarning("Test warning log. {0}", true);
            this.LogError("Test error log. {0}", true);

            this.LogVariable("TEST_ID", 2);
            this.LogVariable("TEST_ID", 3);
            this.LogVariable("TEST_ID", 1);
        }

        public void LogWithCategory(string category, string msg, params object[] args)
        {
            LogWithCategories(new List<string>(new string[] { category }), msg, args);
        }

        public void LogWithCategories(List<string> categories, string msg, params object[] args)
        {
            StackTrace stack = new StackTrace(1, true);

            string test = CreateInfoLine(stack) + CreateCategoryString(categories.ToArray()) + string.Format(msg, args) + FormatStackTrace(stack);

            // Do a proper Debug log based off the categories passed
            if (categories.Contains(LoggerConstants.ERROR_CATEGORY))
            {
                UnityEngine.Debug.LogError(test);
            }
            else if (categories.Contains(LoggerConstants.WARNING_CATEGORY))
            {
                UnityEngine.Debug.LogWarning(test);
            }
            else
            {
                UnityEngine.Debug.Log(test);
            }
        }

        public void Log(string msg, params object[] args)
        {
            LogWithCategory(LoggerConstants.GetDefaultCategory(), msg, args);
        }

        public void LogWarning(string msg, params object[] args)
        {
            LogWithCategory(LoggerConstants.WARNING_CATEGORY, msg, args);
        }

        public void LogError(string msg, params object[] args)
        {
            LogWithCategory(LoggerConstants.ERROR_CATEGORY, msg, args);
        }

        public void LogVariable(string id, object variable)
        {
            // This won't be done for a good long time but I definitely want it to happen!
        }
		#endregion
	
		#region Protected Methods
		#endregion

        #region Private Methods
        private string CreateCategoryString(string[] categories)
        {
            string categoryString = string.Empty;

            if (categories != null && categories.Length > 0)
            {
                categoryString = "{ ";

                for(int i = 0; i < categories.Length; ++i)
                {
                    if (i > 0)
                    {
                        categoryString += ", ";
                    }

                    categoryString += categories[i];
                }

                categoryString += " }";
            }

            return categoryString;
        }

        private string CreateInfoLine(StackTrace stack)
        {
            int frameIndex = GetIndexForFirstValidFrame(stack);
            if (frameIndex >= 0)
            {
                StackFrame frame = stack.GetFrame(GetIndexForFirstValidFrame(stack));
                return string.Format("\n[ {0}:{1} @ {2} ] ", frame.GetMethod().DeclaringType.Name, frame.GetMethod().Name, frame.GetFileLineNumber());
            }
            else
            {
                return string.Empty;
            }
        }

        private string FormatStackTrace(StackTrace stack)
        {
            string formattedStack = "\n\n--- STACK TRACE ---";
#if UNITY_EDITOR
            int startingIndex = GetIndexForFirstValidFrame(stack);

            if (startingIndex >= 0)
            {
                StackFrame[] frames = stack.GetFrames();
                StackFrame frame;

                for (int frameIndex = startingIndex; frameIndex < frames.Length; ++frameIndex)
                {
                    frame = frames[frameIndex];
                    System.Reflection.MethodBase method = frame.GetMethod();
                    formattedStack += "\n" + method.DeclaringType.FullName + ":" + method.Name + "(";
                    System.Reflection.ParameterInfo[] parameters = method.GetParameters();
                    if (parameters != null)
                    {
                        System.Reflection.ParameterInfo parameter;

                        for (int paramIndex = 0; paramIndex < parameters.Length; ++paramIndex)
                        {
                            parameter = parameters[paramIndex];

                            if (paramIndex != 0)
                            {
                                formattedStack += ", ";
                            }

                            formattedStack += parameter.ParameterType.Name;
                        }
                    }

                    string fileName = frame.GetFileName();
                    int assetIndex = fileName.LastIndexOf("Assets");

                    formattedStack += ") (at " + fileName.Substring(assetIndex) + ":" + frame.GetFileLineNumber() + ")";
                }

                formattedStack += "\n--- STACK TRACE END ---\n\n";
            }
#endif

            return formattedStack;
        }

        private int GetIndexForFirstValidFrame(StackTrace stack)
        {
            int counter = -1;
            StackFrame frame;

            do
            {
                counter += 1;
                frame = stack.GetFrame(counter);
            } while (counter < stack.GetFrames().Length && frame.GetMethod().DeclaringType == this.GetType());

            if (counter >= stack.GetFrames().Length)
            {
                return -1;
            }
            else
            {
                return counter;
            }
        }
		#endregion
    }
	
}
