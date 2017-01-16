using UnityEngine;
using System.Collections;
using FSM;

namespace CubeWar {
	public class FSMCubeMoveState : FSMBaseControllerState
	{

		public FSMCubeMoveState(IContext context) : base (context)
		{
			
		}

		public override void StartState()
		{
			base.StartState ();
			m_Controller.SetAnimation (CEnum.EAnimation.Move);
		}

		public override void UpdateState(float dt)
		{
			base.UpdateState (dt);
			m_Controller.UpdateTouchInput (dt);
			m_Controller.UpdateCollider (dt);
			m_Controller.MoveToTarget (m_Controller.GetMovePosition (), dt);
		}

		public override void ExitState()
		{
			base.ExitState ();
		}
	}
}