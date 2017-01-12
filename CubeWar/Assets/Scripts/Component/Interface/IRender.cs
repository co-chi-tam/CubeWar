using UnityEngine;
using System;
using System.Collections;

namespace CubeWar {
	public interface IObjectRender
	{

		void OnBecameVisible();
		void OnBecameInvisible();
		bool GetIsVisible ();
	
	}
}

