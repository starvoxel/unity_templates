/* --------------------------
 *
 * TestBehaviour.cs
 *
 * Description: 
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
#endregion
#endregion

namespace Starvoxel.Core
{
    public class TestBehaviour : MonoBehaviour
    {
        #region Fields & Properties
        //const

        //public

        //protected

        //private

        //properties
        #endregion

        #region Unity Methods
        [ContextMenu("Test")]
        void Awake()
        {
            Logger logger = Services.Logger;

            //This is just some test stuff
            Pool<TestObject> testPool = new Pool<TestObject>(new TestObject("Original"));

            TestObject firstInstance = testPool.NextInstance();
            firstInstance.Name = "1st Clone";

            logger.Log(testPool.ToString());

            TestObject secondInstance = testPool.NextInstance();
            secondInstance.Name = "2nd Clone";

            logger.Log(testPool.ToString());

            TestObject thirdInstance = testPool.NextInstance();
            thirdInstance.Name = "3rd Clone";

            logger.Log(testPool.ToString());

            firstInstance.Deactivate();

            logger.Log(testPool.ToString());

            TestObject fourthInstance = testPool.NextInstance();

            logger.Log(testPool.ToString());

            testPool.Deactivate(fourthInstance);

            logger.Log(testPool.ToString());
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
