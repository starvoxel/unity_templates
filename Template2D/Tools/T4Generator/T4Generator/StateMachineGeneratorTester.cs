
/* --------------------------
 *
 * StateMachine.cs
 *
 * Description: This is a auto-generated state machine.
 *
 * -------------------------- */

#region Includes
#region System Includes
using System.Collections;
#endregion

#region Other Includes
using Starvoxel.Core;
#endregion
#endregion

namespace Starvoxel.Test
{
	public class StateMachine 
	{
		#region Fields & Properties
		//const

		// classes
		public abstract class StateMachineState : BaseState 
		{
			public struct sTransitionData
			{
				public readonly StateMachine.Transitions m_TransitionType;
				public readonly TransitionValidity m_TransitionValidity;
				public readonly StateMachine.States m_StateID;

				public sTransitionData(StateMachine.Transitions transitionType, TransitionValidity transitionValidity, StateMachine.States stateID)
				{
					m_TransitionType = transitionType;
					m_TransitionValidity = transitionValidity;
					m_StateID = stateID;
				}
			}

			public abstract StateMachine.States StateID
			{
				get;
			}

			protected abstract bool IsValidTransition(StateMachine.Transitions transitionType);
		}

		// enums
		public enum States
		{
			Alpha = 0,
			Beta = 1,
			Gamma = 2,
		}
		
		public enum Transitions
		{
			Next = 0,
			Previous = 1,
		}
		//protected
		protected StateMachineState m_CurrentState;
		
		//private

		//properties
		public StateMachineState CurrentState
		{
			get { return m_CurrentState; }
		}
		#endregion

		#region Constructor Methods
		public StateMachine() { }
		#endregion

		#region Public Methods
		#endregion

		#region Protected Methods
		#endregion

		#region Private Methods
		#endregion
	}
}