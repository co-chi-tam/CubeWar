using UnityEngine;
using System.Collections;
using FSM;

namespace CubeWar {
	public class FSMSkillActiveState : FSMBaseControllerState
	{
		public FSMSkillActiveState(IContext context) : base (context)
		{

		}

		public override void StartState()
		{
			base.StartState ();
			m_Controller.OnStartAction.Invoke (null);
			m_Controller.ResetPerAction ();
		}

		public override void UpdateState(float dt)
		{
			base.UpdateState (dt);
			m_Controller.OnAction.Invoke (null);
		}

		public override void ExitState()
		{
			base.ExitState ();
		}
	}
}