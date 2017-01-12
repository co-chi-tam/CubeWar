using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using FSM;

namespace CubeWar {
	public partial class CPlayerCubeController {

		#region FSM

		protected override void OnRegisterFSM() {
			base.OnRegisterFSM ();
			var attackState 	= new FSMCubeAttackState (this);
			var seekState 		= new FSMCubeSeekState (this);
			var autoAttackState = new FSMCubeAutoAttackState (this);
			var autoSeekState 	= new FSMCubeAutoSeekState (this);
			var autoAvoidanceState = new FSMCubeAutoAvoidanceState (this);

			m_FSMManager.RegisterState ("CubeAttackState", 			attackState);
			m_FSMManager.RegisterState ("CubeSeekState", 			seekState);
			m_FSMManager.RegisterState ("CubeAutoAttackState", 		autoAttackState);
			m_FSMManager.RegisterState ("CubeAutoSeekState",		autoSeekState);
			m_FSMManager.RegisterState ("CubeAutoAvoidanceState",	autoAvoidanceState);

			m_FSMManager.RegisterCondition ("DidMoveToPosition",	DidMoveToPosition);
			m_FSMManager.RegisterCondition ("DidMoveToTargetAttack", DidMoveToTargetAttack);
			m_FSMManager.RegisterCondition ("HaveTargetAttack",		HaveTargetAttack);
			m_FSMManager.RegisterCondition ("HaveTargetInRange",	HaveTargetInRange);
		}

		#endregion
	
	}
}
