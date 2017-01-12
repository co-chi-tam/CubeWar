using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using FSM;

namespace CubeWar {
	public partial class CCubeController {

		#region Main methods

		protected override void OnRegisterAnimation ()
		{
			base.OnRegisterAnimation ();
			m_AnimatorController.RegisterAnimation ("Inactive", OnCubeInactive);
		}

		protected virtual void OnCubeInactive(string animName) {
			base.DisableObject (animName);
		}

		#endregion
	
	}
}
