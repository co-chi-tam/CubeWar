using UnityEngine;
using System.Collections;
using FSM;

namespace CubeWar {
	public class FSMCubeWaitingState : FSMBaseControllerState
	{
		public FSMCubeWaitingState(IContext context) : base (context)
		{

		}

		public override void StartState()
		{
			base.StartState ();
			m_Controller.SetAnimation (CEnum.EAnimation.Idle);
		}

		public override void UpdateState(float dt)
		{
			base.UpdateState (dt);
			m_Controller.UpdateTouchInput (dt);
		}

		public override void ExitState()
		{
			base.ExitState ();
		}
	}
}