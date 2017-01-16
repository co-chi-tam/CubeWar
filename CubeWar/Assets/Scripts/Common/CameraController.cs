using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CubeWar {
	[RequireComponent (typeof (Camera))]
	// Place the script in the Camera-Control group in the component menu
	[AddComponentMenu("Camera-Control/Smooth Follow")]
	public class CameraController : CMonoSingleton<CameraController> {

	    public Transform target;	

		private Camera m_Camera;
		private Vector3 m_CurrentScale;
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

		public void SetTarget(Transform tar) {
			this.target = tar;
			m_CurrentScale = target.localScale;
			m_CurrentFar = m_Camera.farClipPlane; 
		}

		// The distance in the x-z plane to the target
		public float distance = 10.0f;
		// the height we want the camera to be above the target
		public float height = 5.0f;
		// How much we 
		public float heightDamping = 2.0f;
		public float rotationDamping = 3.0f;

		void LateUpdate () {
			// Early out if we don't have a target
			if (!target) return;
			m_Camera.farClipPlane = target.localScale.y * m_CurrentFar / m_CurrentScale.y;

			// Calculate the current rotation angles
			float wantedRotationAngle = target.eulerAngles.y;
			float wantedHeight = target.position.y + (target.localScale.y * height / m_CurrentScale.y);

			float currentRotationAngle = transform.eulerAngles.y;
			float currentHeight = transform.position.y;

			// Damp the rotation around the y-axis
			currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

			// Damp the height
			currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

			// Convert the angle into a rotation
			var currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

			// Set the position of the camera on the x-z plane to:
			// distance meters behind the target
			transform.position = target.position;
			transform.position -= currentRotation * Vector3.forward * distance;

			// Set the height of the camera
			transform.position = new Vector3(transform.position.x,currentHeight,transform.position.z);

			// Always look at the target
			transform.LookAt(target);
		}

	}
}