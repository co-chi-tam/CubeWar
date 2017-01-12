using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace CubeWar {
	public class CMsgRegisterPlayer : MessageBase {

		public CUserData userData;

		public CMsgRegisterPlayer () : base()
		{
			this.userData = new CUserData ();
		}

	}
}
