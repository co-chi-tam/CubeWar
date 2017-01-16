using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CubeWar {
	public class CPlayableEntity : CEntity {

		#region Properties

		// User
		public CUserData userData;

		#endregion

		#region Implementation MonoBehaviour 

		public override void Init ()
		{
			base.Init ();
			m_ObjectSyn.SetName (userData.displayName);
			m_ObjectSyn.SetToken (userData.token);
		}

		protected override void Awake ()
		{
			base.Awake ();
			userData = new CUserData ();
		}

		// Active on Server
		public override void OnServerLoadedObject ()
		{
			base.OnServerLoadedObject ();
			m_ObjectSyn.SetUnderControl (false);
		}

		// Active On local and is local player
		public override void OnLocalPlayerLoadedObject ()
		{
			base.OnLocalPlayerLoadedObject ();
			m_ObjectSyn.SetUnderControl (true);
			m_ObjectSyn.AddEventListener ("TouchScreenInput", OnClientTouchScreenInput);
			m_ObjectSyn.AddEventListener ("MoveDirectionInput", OnClientMoveDirectionInput);
			m_ObjectSyn.AddEventListener ("ActionInput", OnClientActionInput);
			m_ObjectSyn.AddEventListener ("ChatInput", OnClientChatInput);
			m_ObjectSyn.AddEventListener ("EmotionInput", OnClientEmotionInput);
		}

		// Active On local and is client
		public override void OnClientLoadedObject ()
		{
			base.OnClientLoadedObject ();
			m_ObjectSyn.SetUnderControl (false);
		}

		#endregion

		#region Main methods

		#endregion

		#region Server

		[ServerCallback]
		public override void OnServerFixedUpdateSynData(float dt) {
			base.OnServerFixedUpdateSynData (dt);
			// Update Info
			RpcUpdateUserData (userData.displayName, userData.token);
		}

		#endregion

		#region Client

		[ClientCallback]
		public override void OnClientFixedUpdateBaseTime(float dt) {
			if (m_ObjectSyn == null)
				return;
			base.OnClientFixedUpdateBaseTime (dt);
		}

		[ClientCallback]
		public virtual void OnClientTouchScreenInput(object value) {
			var touchPoints = value as Vector3[];
			CmdUpdateSelectionObject (touchPoints[0], touchPoints[1]);
		}

		[ClientCallback]
		public virtual void OnClientMoveDirectionInput(object value) {
			var directionPoint = (Vector3)value;
			CmdUpdateMoveDirection (directionPoint);
		}

		[ClientCallback]
		public virtual void OnClientActionInput(object value) {
			var action = (int)value;
			CmdOnClientActionInput (action);
		}

		[ClientCallback]
		public virtual void OnClientChatInput(object value) {
			var chatInput = (string)value;
			CmdUpdateChat (chatInput);
		}

		[ClientCallback]
		public virtual void OnClientEmotionInput(object value) {
			var emotionInput = (string)value;
			CmdUpdateEmotion (emotionInput);
		}

		#endregion

		#region Command

		[Command]
		internal virtual void CmdUpdateChat(string chat) {
			m_ObjectSyn.SetChat (chat);
			RpcUpdateChat (chat);
		}

		[Command]
		internal virtual void CmdUpdateEmotion(string emotion) {
			m_ObjectSyn.SetEmotion (emotion);
			RpcUpdateEmotion (emotion);
		}

		[Command]
		internal virtual void CmdUpdateSelectionObject(Vector3 originPoint, Vector3 directionPoint) {
			m_ObjectSyn.UpdateSelectionObject (originPoint, directionPoint);
		}

		[Command]
		internal virtual void CmdUpdateMoveDirection(Vector3 directionPoint) {
			m_ObjectSyn.UpdateMoveDirection (directionPoint);
		}

		[Command]
		internal virtual void CmdOnClientActionInput(int value) {
			m_ObjectSyn.UpdateActionInput (value);
		}

		#endregion

		#region RPC

		[ClientRpc]
		internal virtual void RpcUpdateUserData(string name, string token) {
			userData.displayName = name;
			userData.token = token;
			if (m_ObjectSyn == null)
				return;
			m_ObjectSyn.SetName (name);
			m_ObjectSyn.SetToken (token);
		}

		[ClientRpc]
		internal virtual void RpcUpdateChat(string chat) {
			if (m_ObjectSyn == null)
				return;
			m_ObjectSyn.SetChat (chat);
		}

		[ClientRpc]
		internal virtual void RpcUpdateEmotion(string emotion) {
			if (m_ObjectSyn == null)
				return;
			m_ObjectSyn.SetEmotion (emotion);
		}

		#endregion

	}

}
