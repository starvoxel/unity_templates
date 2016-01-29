/* -------------------------- * * ActionRunner.cs * * Description: Testing script used to fire actions * * Author: Jeremy Smellie * * Editors: * * 1/28/2016 - Starvoxel * * All rights reserved. * * -------------------------- */using UnityEngine;using System.Collections; namespace Starvoxel.FlowManagement{	public class ActionRunner : CustomMono	{		//const			//public			//protected        [SerializeField] private string m_ActionID;			//private			//properties        [ContextMenu("Run Action")]        private void ContextRunAction()
        {
            Debug.Log("Running Action: " + m_ActionID);
            FlowManager.Instance.TriggerAction(m_ActionID);
        }	}	}