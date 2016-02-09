﻿ 
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
using System.Collections.Generic;
#endregion

#region Other Includes
using Starvoxel.Core;
#endregion
#endregion

namespace Starvoxel.Core
{
	public sealed partial class StateMachine 
	{
		#region Classes
		public abstract class StateMachineState : BaseState 
		{
			#region Structs
            /// <summary>
            /// Data used to determine if a transition is valid and what state it would transition to
            /// </summary>
			public struct sTransitionData
			{
				public readonly eTransitionValidity TransitionValidity;
				public readonly StateMachine.eStates StateID;

				public sTransitionData(eTransitionValidity transitionValidity, StateMachine.eStates stateID)
				{
					TransitionValidity = transitionValidity;
					StateID = stateID;
				}
			}
			#endregion

			#region Fields & Properties
			protected readonly StateMachine m_Context = null; // Link to the context that owns tis instance.
			protected readonly Dictionary<StateMachine.eTransitions, sTransitionData> m_Transitions = null; // Map of all the transitions and to which states those transitions lead to
			
            /// <summary>
            /// State enum associated with this class.
            /// </summary>
			public abstract StateMachine.eStates StateID
			{
				get;
			}
			#endregion

			#region Constructors
			public StateMachineState(StateMachine context)
			{
				m_Context = context;
			}
			#endregion

			#region Public Methods
            /// <summary>
            /// Checks if the provided transition type is a valid transition for this state.
            /// </summary>
            /// <param name="transitionType">Transition type</param>
            /// <returns></returns>
			public bool IsValidTransition(StateMachine.eTransitions transitionType)
			{
				 return m_Transitions != null && m_Transitions.ContainsKey(transitionType) && m_Transitions[transitionType].TransitionValidity == eTransitionValidity.Valid;
			}
			#endregion
		}

		#region ---------- PLACEHOLDER STATES ----------
		public class AlphaState : StateMachineState
		{
			public StateMachine.eStates StateID
			{
				get { return StateMachine.eStates.ALPHA; }
			}

			public bool IsValidTransition(StateMachine.eTransitions transitionType)
			{
				return false;
			}

			public AlphaState(StateMachine context) : base(context) { }
		}
		
		public class BetaState : StateMachineState
		{
			public StateMachine.eStates StateID
			{
				get { return StateMachine.eStates.BETA; }
			}

			public bool IsValidTransition(StateMachine.eTransitions transitionType)
			{
				return false;
			}

			public BetaState(StateMachine context) : base(context) { }
		}
		
		public class GammaState : StateMachineState
		{
			public StateMachine.eStates StateID
			{
				get { return StateMachine.eStates.GAMMA; }
			}

			public bool IsValidTransition(StateMachine.eTransitions transitionType)
			{
				return false;
			}

			public GammaState(StateMachine context) : base(context) { }
		}
		#endregion
		#endregion

		#region Enums
		/// <summary>
        /// All states.  Also used as the index in the state array
        /// </summary>
		public enum eStates
		{
			ALPHA = 0,
			BETA = 1,
			GAMMA = 2,
		}
		
        /// <summary>
        /// All possible transition types
        /// </summary>
		public enum eTransitions
		{
			Next = 0,
			Previous = 1,
		}
		#endregion

		#region Fields & Properties
		//const

		//public

		//protected
		protected StateMachineState[] m_States; // Instances of all the states
		protected int m_CurrentStateIndex = 0; // Index of the currently active state
		//private

		//properties
		public StateMachineState CurrentState
		{
			get { return m_States[m_CurrentStateIndex]; }
		}
		#endregion

		#region Constructor Methods
		public StateMachine()
		{
			m_States = new StateMachineState[3];
			
			// I know this looks hardcoded, but because this is a generated file this will be auto-updated when re-generated.
			m_States[0] = new AlphaState(this);
			m_States[1] = new BetaState(this);
			m_States[2] = new GammaState(this);
		}
		#endregion

		#region Public Methods
        /// <summary>
        /// Called to process a transition and potentially transition to a new state
        /// </summary>
        /// <param name="transitionType">Transiton type to try and transition with</param>
		public void ProcessTransition(StateMachine.eTransitions transitionType)
		{
			if (CurrentState.IsValidTransition(transitionType))
			{
				//TODO jsmellie: Fetch the state enum associated with the transition type
			}
		}
		#endregion

		#region Protected Methods
		#endregion

		#region Private Methods
		#endregion
	}
}