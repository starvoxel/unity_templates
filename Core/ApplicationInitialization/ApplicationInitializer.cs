/* --------------------------
 *
 * ApplicationInitializer.cs
 *
 * Description: Does all the initialization for the game.  Partial class so the game can put what ever it wants as well
 *
 * Author: Jeremy Smellie
 *
 * Editors:
 *
 * 1/31/2016 - DefaultCompany
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
using Starvoxel.FlowManagement;
#endregion
#endregion

 namespace Starvoxel.Core
{
	public partial class ApplicationInitializer : CustomMono
	{
		#region Fields & Properties
		//const
        private const string DEFAULT_FLOW_PATH = "XML/Flow";
        private const string INNITIALIZATION_SCENE_NAME = "ApplicationInitializer";
	
		//public
	
		//protected
	
		//private
        private static bool m_IsInitialized = false;
	
		//properties
        public static bool IsInitialized
        {
            get { return m_IsInitialized; }
        }
		#endregion
	
		#region Unity Methods
        private void Awake()
        {
#if !FLOW_MANAGEMENT
            string flowFilePath = DEFAULT_FLOW_PATH;
            FetchFlowXMLPath(ref flowFilePath);
            FlowManager.Instance.LaunchWithFile(flowFilePath);
#endif
            //TODO jsmellie:  Everything that needs to be init at the begining of the application should go here.
            Services.InitializeLogger(new TextFileLogger(true));

            m_IsInitialized = true;
        }
		#endregion
	
		#region Public Methods
        public static void Initialize()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(INNITIALIZATION_SCENE_NAME, UnityEngine.SceneManagement.LoadSceneMode.Additive);
        }
		#endregion
	
		#region Protected Methods
		#endregion
	
		#region Private Methods
		#endregion

        #region Partial Methods
        partial void PartialApplicationInitialization();
        partial void FetchFlowXMLPath(ref string xmlPath);
        #endregion
    }
	
}