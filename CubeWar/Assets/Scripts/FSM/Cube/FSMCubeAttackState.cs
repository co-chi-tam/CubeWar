﻿using UnityEngine;
using System.Collections;
using FSM;

namespace CubeWar {
	public class FSMCubeAttackState : FSMBaseControllerState
	{

		public FSMCubeAttackState(IContext context) : base (context)
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
			m_Controller.UpdateTouchInput (dt);
			var target = m_Controller.GetTargetInteract ();
			if (target != null) { 
				m_Controller.LookAtTarget (target.GetPosition ());
			}
		}

		public override void ExitState()
		{
			base.ExitState ();
			m_Controller.SetTargetInteract (null);
			m_Controller.SetMovePosition (m_Controller.GetPosition ());
		}
	}
}