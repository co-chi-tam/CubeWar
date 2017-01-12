using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CubeWar {
	[Serializable]
	public class CUserData : CObjectData {

		public string userName;
		public string displayName;
		public string avartar;
		public int gold;
		public int diamond;
		public bool isLogin;
		public string token;
		public string cubeColor;

		public CUserData () : base ()
		{
			this.userName = string.Empty;
			this.displayName = string.Empty;
			this.avartar = string.Empty;
			this.gold = 0;
			this.diamond = 0;
			this.isLogin = false;
			this.token = string.Empty;
			this.cubeColor = string.Empty;
		}

	}
}
