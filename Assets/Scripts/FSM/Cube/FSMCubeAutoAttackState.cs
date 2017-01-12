using UnityEngine;
using System.Collections;
using FSM;

namespace CubeWar {
	public class FSMCubeAutoAttackState : FSMBaseControllerState
	{
		public FSMCubeAutoAttackState(IContext context) : base (context)
		{

		}

		public override void StartState()
		{
			base.StartState ();
			if (m_Controller.GetOtherInteractive ()) {
				m_Controller.InteractAnObject ();
			}
		}

		public override void UpdateState(float dt)
		{
			base.UpdateState (dt);
			var target = m_Controller.GetTargetInteract ();
			if (target != null) { 
				m_Controller.LookAtTarget (target.GetPosition ());
			}
		}

		public override void ExitState()
		{
			base.ExitState ();
		}
	}
}