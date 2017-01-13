using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CubeWar {
	[RequireComponent (typeof (Camera))]
	public class CameraController : CMonoSingleton<CameraController> {

	    public Transform target;	
		[SerializeField]	private Vector3 m_OffsetPosition;
		[SerializeField]	private Vector3 m_OffsetRotation;

		private Camera m_Camera;
		private Vector3 m_CurrentScale;
		private Vector3 m_CurrentOffsetPosition;
		private float m_CurrentFar;

		protected override void Awake ()
		{
			base.Awake ();
			m_Camera = this.GetComponent<Camera> ();
			if (target != null) {
				m_CurrentScale = target.localScale;
				m_CurrentFar = m_Camera.farClipPlane;
			}
		}

		protected override void Update() {
			if (target != null) {
				m_CurrentOffsetPosition.x = target.localScale.x * m_OffsetPosition.x / m_CurrentScale.x * 0.95f;
				m_CurrentOffsetPosition.y = target.localScale.y * m_OffsetPosition.y / m_CurrentScale.y * 0.95f;
				m_CurrentOffsetPosition.z = target.localScale.z * m_OffsetPosition.z / m_CurrentScale.z * 0.95f;
				m_Transform.position = Vector3.Lerp (m_Transform.position, target.transform.position + m_CurrentOffsetPosition, 0.1f);
				m_Transform.rotation = Quaternion.Euler (m_OffsetRotation);
				m_Camera.farClipPlane = target.localScale.x * m_CurrentFar / m_CurrentScale.x;
			}
		}

		public void SetTarget(Transform tar) {
			this.target = tar;
			m_CurrentScale = target.localScale;
			m_CurrentFar = m_Camera.farClipPlane; 
		}
	}
}