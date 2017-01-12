using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using FSM;

namespace CubeWar {
	public class CSmallCubeController : CCubeController {

		protected override void Start ()
		{
			base.Start ();
			// FSM
			var fsmJson = CResourceManager.Instance.LoadResourceOrAsset <TextAsset> (m_Data.fsmPath);
			m_FSMManager.LoadFSM (fsmJson.text);

			// Random animation idle
			this.m_AnimatorController.RandomAnimationFrame();

			// Random color
			//			this.m_CubeMesh.material.shader = Shader.Find("Standard");
			//			this.m_CubeMesh.material.color = UnityEngine.Random.ColorHSV ();
		}

		public override void FixedUpdateBaseTime (float dt)
		{
			base.FixedUpdateBaseTime (dt);
			if (this.GetActive()) {
				UpdateFSM (dt);
			}
		}
	
	}
}
