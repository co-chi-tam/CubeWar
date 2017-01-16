using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UICustomize;
using ObjectPool;

namespace CubeWar {
	public class CUIControlManager : CMonoSingleton<CUIControlManager> {

		[Header("Control")]
		[SerializeField]	private GameObject m_UIControlPanel;
		[Header("Joytick")]
		[SerializeField]	protected UIJoytick m_Joytick;
		[Header("Info")]
		[SerializeField]	private GameObject m_UIInfoRootPanel;
		[SerializeField]	private CUIObjectInfo m_UIObjectInfoPrefab;

		public Action<string> OnEventChatInput;
		public Action<string> OnEventEmotionInput;

		private ObjectPool<CUIObjectInfo> m_UIObjInfoPool;

		protected override void Awake ()
		{
			base.Awake ();
			this.m_UIObjInfoPool = new ObjectPool<CUIObjectInfo> ();
		} 

		public void RegisterControl(bool joytick, 
			Action<string> eventChat, 
			Action<string> eventEmotion) {
			m_Joytick.SetEnable (joytick);
			this.m_UIControlPanel.SetActive (true);
			// Reset
			this.OnEventChatInput = null;
			this.OnEventEmotionInput = null;
			// Register
			this.OnEventChatInput = eventChat;
			this.OnEventEmotionInput = eventEmotion;
		}

		public void RegisterUIInfo(IStatus value, bool showName, bool showStatus) {
			this.m_UIInfoRootPanel.SetActive (true);
			var waitingToCreate = 2;
			while (waitingToCreate > 0) {
				var uiInfoPool = this.m_UIObjInfoPool.Get ();
				if (uiInfoPool != null) {
					var uiInfoRect = uiInfoPool.transform as RectTransform;
					uiInfoPool.owner = value;
					uiInfoPool.ShowName (showName);
					uiInfoPool.ShowStatus (showStatus);
					uiInfoRect.SetParent (m_UIInfoRootPanel.transform);
					// Reset position
					uiInfoRect.anchoredPosition = Vector2.zero;
					uiInfoRect.localScale = Vector3.one;
					uiInfoRect.sizeDelta = Vector2.one;
					break;
				} else {
					var uiInfoPrefab = Instantiate<CUIObjectInfo> (m_UIObjectInfoPrefab);
					this.m_UIObjInfoPool.Set (uiInfoPrefab);
					uiInfoPrefab.owner = value;
				}
				waitingToCreate--;
			}
		}

		public void UnregisterUIInfo(CUIObjectInfo value) {
			this.m_UIObjInfoPool.Set (value);
		}

		public void PressEmotionItem(Image emotionImage) {
			if (this.OnEventEmotionInput != null) {
				this.OnEventEmotionInput (emotionImage.sprite.name);
			}
		}

		public void SubmitChat(InputField inputTalk) {
			var submitText = inputTalk.text;
			if (string.IsNullOrEmpty (submitText))
				return;
			if (this.OnEventChatInput != null) {
				this.OnEventChatInput (submitText);
			}
			inputTalk.text = string.Empty;
		}

		public Vector3 GetJoytickPoint() {
			return m_Joytick.InputDirection;
		}
	
	}
}
