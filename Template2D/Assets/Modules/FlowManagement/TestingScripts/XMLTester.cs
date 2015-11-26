/* --------------------------
 *
 * XMLTester.cs
 *
 * Description: 
 *
 * Author: Jeremy Smellie
 *
 * Editors:
 *
 * 11/24/2015 - Starvoxel
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
using System.IO;
using System.Xml;
#endregion

#region Other Includes

#endregion
#endregion

 namespace Starvoxel
{
	public class XMLTester : MonoBehaviour
	{
		#region Fields & Properties
		//const
	
		//public
	
		//protected
	
		//private
	
		//properties
		#endregion
	
		#region Unity Methods
        private void Awake()
        {
            TextAsset asset = Resources.Load<TextAsset>("XML/TestFlow_1");
            string xmlString = asset.text;

            // Create an XmlReader
            using (XmlReader reader = XmlReader.Create(new StringReader(xmlString)))
            {
                while (reader.Read())
                {
                    // We don't care about white space or the declaration at the top
                    if (IsInvalidNodeType(reader))
                    {
                        continue;
                    }

                    /*string msg = string.Format("Node Data - type: {0} | name: {1} | value: {2}", reader.NodeType, reader.Name, reader.Value);

                    if (reader.HasAttributes)
                    {
                        msg += " | attributes: ";

                        reader.MoveToFirstAttribute();

                        bool addDivider = false;

                        do
                        {
                            if (addDivider)
                            {
                                msg += ", ";
                            }
                            else
                            {
                                addDivider = true;
                            }
                            msg += reader.Name + ";" + reader.Value;
                        } while (reader.MoveToNextAttribute());
                    }
                    Debug.Log(msg);*/
                }
            }
        }
		#endregion
	
		#region Public Methods
		#endregion
	
		#region Protected Methods
		#endregion
	
		#region Private Methods
        private bool IsInvalidNodeType(XmlReader reader)
        {
            return reader.NodeType == XmlNodeType.Whitespace || reader.NodeType == XmlNodeType.SignificantWhitespace || reader.NodeType == XmlNodeType.XmlDeclaration;
        }
		#endregion
	}
}