/* --------------------------
 *
 * Alpha.cs
 *
 * Description: One of the states used in the TestMachine state machine.
 * WARNING: THIS CLASS IS GENERATED.  CHANGES COULD BE LOST.
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
	public partial class TestMachine
	{
		public sealed partial class AlphaState : TestMachineState
		{
			public TestMachine.eStates StateID
			{
				get { return TestMachine.eStates.ALPHA; }
			}

			public AlphaState(TestMachine context) : base(context) { }

			protected override void PopulateTransitionDictionary()
			{
				m_Transitions = new Dictionary<TestMachine.eTransitions, sTransitionData>();

				sTransitionData transitionData;

				// Valid Transitions
				transitionData = new sTransitionData(eTransitionValidity.Valid, TestMachine.eStates.BETA);
				m_Transitions.Add(TestMachine.eTransitions.NEXT, transitionData);

				//Ignored Transitions
				transitionData = new sTransitionData(eTransitionValidity.Ignore, (TestMachine.eStates)0);
				m_Transitions.Add(TestMachine.eTransitions.PREVIOUS, transitionData);
			}
		}
	}
}