using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CubeWar {
	[RequireComponent(typeof(Renderer))]
	public class CRenderController : CBaseMonoBehaviour {

		[SerializeField]	private GameObject m_ObjectCallback;
		private IObjectRender m_Root;

		protected override void Awake ()
		{
			base.Awake ();
			if (m_Root == null) {
				m_Root = this.transform.GetComponentInParent<IObjectRender> ();
			} else {
				m_Root = m_ObjectCallback.GetComponent<IObjectRender> ();
			}
		}

		protected override void Update ()
		{
			base.Update ();
		}

		public void OnBecameVisible() {
			m_Root.OnBecameVisible ();
		}

		public void OnBecameInvisible() {
			m_Root.OnBecameInvisible ();
		}

	}
}
