using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using FSM;

namespace CubeWar {
	public partial class CPlayerCubeController {

		protected override void OnRegisterAnimation ()
		{
			base.OnRegisterAnimation ();
			m_AnimatorController.RegisterAnimation ("Shot", OnCubeShot);
		}

		protected virtual void OnCubeShot(string animationName) {
			var forwardSeekPosition = m_Transform.forward.normalized * (this.GetSize() + 10f) + this.GetPosition();
			forwardSeekPosition.y = 0f;
			this.CreateSkillObject ("CubeShotSkill", this.GetPosition(), forwardSeekPosition, null);
			this.SetAnimation (CEnum.EAnimation.Idle);
			this.SetAction (CEnum.EAnimation.Idle);
			this.SetMovePosition (this.GetPosition ());
		}

		private void CreateSkillObject(string name, Vector3 position, Vector3 movePosition, params CObjectController[] targets) {
			var objectSkill = CObjectManager.Instance.GetObject(name) as CSkillController;
			objectSkill.Init ();
			objectSkill.SetActive (true);
			objectSkill.SetEnable (true);
			objectSkill.SetStartPosition (position);
			objectSkill.SetPosition (position);
			objectSkill.SetMovePosition (movePosition);
			if (targets != null && targets.Length > 0) {
				objectSkill.SetTargetInteract (targets [0]);
			}
			objectSkill.OnStartAction.RemoveAllListener ();
			objectSkill.OnStartAction.AddListener ((x) => {
				// TODO
			});
			objectSkill.OnAction.RemoveAllListener ();
			objectSkill.OnAction.AddListener ((x) => {
				// TODO
			});
			objectSkill.OnEndAction.RemoveAllListener ();
			objectSkill.OnEndAction.AddListener ((x) => {
				// TODO
			});
		}
	
	}
}
