using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using FSM;

namespace CubeWar {
	public partial class CCubeController {

		#region Main methods

		protected override void OnRegisterFSM() {
			base.OnRegisterFSM ();
			var idleState 		= new FSMCubeIdleState (this);
			var moveState 		= new FSMCubeMoveState (this);
			var inactiveState	= new FSMCubeInactiveState (this);
			var waitingState	= new FSMCubeWaitingState (this);

			m_FSMManager.RegisterState ("CubeIdleState", 			idleState);
			m_FSMManager.RegisterState ("CubeMoveState", 			moveState);
			m_FSMManager.RegisterState ("CubeInactiveState",		inactiveState);
			m_FSMManager.RegisterState ("CubeWaitingState",			waitingState);
		}

		#endregion

		#region FSM

		internal override bool DidEndWaiting() {
			m_WaitingPerAction -= Time.fixedDeltaTime;
			return m_WaitingPerAction <= 0f;
		}

		#endregion

		#region Getter && Setter 

		public override string GetFSMStateName ()
		{
			base.GetFSMStateName ();
			return m_FSMManager.currentStateName;
		}

		public override void SetFSMStateName (string value)
		{
			base.SetFSMStateName (value);
			m_FSMManager.SetState (value);
		}

		public override string GetFSMName ()
		{
			if (m_Data != null) 
				return m_Data.fsmPath;
			return base.GetFSMName ();
		}

		public override void SetFSMName (string value)
		{
			base.SetFSMName (value);
			m_Data.fsmPath = value;
		}

		#endregion
	
	}
}
