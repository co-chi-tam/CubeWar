using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CubeWar {
	public class CFadeGUI : MonoBehaviour {

		#region Properties

		[SerializeField]	private Color m_ScreenLoadingColor = Color.white;
		[Range(0.25f, 5f)]
		[SerializeField]	private float m_ScreenLoadingTime = 1f;

		private Texture2D m_LoadingScreenTexture;
		private Rect m_FullScreenRect;
		private bool m_Faded = false;
		private bool m_NeedDraw = false;

		#endregion

		#region Monobehaviour

		protected virtual void Awake ()
		{
			m_FullScreenRect = new Rect (0f, 0f, Screen.width, Screen.height);
			this.OnRepairTexture ();
		}

		protected virtual void Start ()
		{
			
		}

		protected virtual void OnGUI() {
			if (Event.current.type.Equals (EventType.Repaint) && m_NeedDraw) {
				GUI.DrawTexture (m_FullScreenRect, m_LoadingScreenTexture, ScaleMode.StretchToFill);
				if (m_Faded) {
					var currentColor = m_LoadingScreenTexture.GetPixels () [0];
					currentColor.a -= 1f / m_ScreenLoadingTime * Time.deltaTime;
					m_LoadingScreenTexture.SetPixels (new Color[] { currentColor });
					m_LoadingScreenTexture.Apply ();
					m_Faded = currentColor.a > 0f;
				}
			}
		}

		#endregion 

		#region Main methods

		private void OnRepairTexture() {
			m_LoadingScreenTexture = new Texture2D (1, 1);
			m_LoadingScreenTexture.SetPixels (new Color[] { m_ScreenLoadingColor });
			m_LoadingScreenTexture.Apply ();
		}

		public void OnFadeScreen() {
			OnRepairTexture ();
			m_Faded = true;
		}

		public void OnDraw(bool value) {
			m_NeedDraw = value;
		}

		#endregion
	
	}
}
