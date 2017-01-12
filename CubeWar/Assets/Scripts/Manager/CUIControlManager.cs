using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UICustomize;

namespace CubeWar {
	public class CUIControlManager : CMonoSingleton<CUIControlManager> {

		[Header("Joytick")]
		[SerializeField]	protected UIJoytick m_Joytick;

		protected override void Awake ()
		{
			base.Awake ();
		} 

		public void RegisterControl(bool joytick) {
			m_Joytick.SetEnable (joytick);
		}

		public Vector3 GetJoytickPoint() {
			return m_Joytick.InputDirection;
		}
	
	}
}
