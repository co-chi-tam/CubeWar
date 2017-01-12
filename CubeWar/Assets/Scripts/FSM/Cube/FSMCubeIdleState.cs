using UnityEngine;
using System.Collections;
using FSM;

namespace CubeWar {
	public class FSMCubeIdleState : FSMBaseControllerState
	{
		public FSMCubeIdleState(IContext context) : base (context)
		{

		}

		public override void StartState()
		{
			base.StartState ();
			m_Controller.SetAnimation (CEnum.EAnimation.Idle);
			m_Controller.ResetPerAction ();
		}

		public override void UpdateState(float dt)
		{
			base.UpdateState (dt);
			m_Controller.UpdateTouchInput (dt);
			m_Controller.UpdateAction (dt);
		}

		public override void ExitState()
		{
			base.ExitState ();
		}
	}
}