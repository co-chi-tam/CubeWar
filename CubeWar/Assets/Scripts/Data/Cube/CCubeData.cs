using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CubeWar {
	[Serializable]
	public class CCubeData : CObjectData {

		public CEnum.EClassType classType;
		public float moveSpeed;
		public float seekRadius;
		public string token;

		public CCubeData () : base ()
		{
			this.classType = CEnum.EClassType.None;
			this.moveSpeed = 0f;
			this.seekRadius = 0f;
			this.token = string.Empty;
		}
	
	}
}
