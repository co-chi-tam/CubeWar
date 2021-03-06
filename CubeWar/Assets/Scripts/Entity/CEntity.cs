﻿using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CubeWar {
	
	[RequireComponent(typeof(NetworkIdentity))]
	public class CEntity : NetworkBehaviour, IEntity {
	
		#region Properties

		// Sync Object
		protected IStatus m_ObjectSyn;

		// Network
		protected CNetworkManager m_NetworkManager;
		// Sync time
		protected float m_FixedTimeSync = 0.1f;
		protected CountdownTime m_CountDownFixedTimeSync;
		// Transform
		protected Vector3 m_MovePosition;
		protected Vector3 m_Position;
		protected Vector3 m_StartPosition;
		protected Vector3 m_Rotation;
		// Animation
		protected int m_Animation = 0;
		protected float m_AnimationTime = 0f;
		// Info
		public string uID;
		// Control data
		public CObjectData controlData;
		// Interactive
		protected string m_TargetInteractiveId = "-1";
		// Server
		protected float m_ServerObjectTime;
		// Client
		protected float m_ClientObjectTime;

		#endregion

		#region Implementation MonoBehaviour

		protected virtual void OnEnable() {
			
		}

		protected virtual void OnDisable() {

		}

		// Init to first spawn object
		public virtual void Init() {
			// Move Position
			m_ObjectSyn.SetMovePosition (m_StartPosition);
			// Transfrom
			m_ObjectSyn.SetPosition (m_Position);
			m_ObjectSyn.SetStartPosition (m_StartPosition);
			m_ObjectSyn.SetRotation (m_Rotation);
			// Animation
			m_ObjectSyn.SetAnimation ((CEnum.EAnimation) m_Animation);
			m_ObjectSyn.SetAnimationTime (m_AnimationTime);
			// Info
			m_ObjectSyn.SetID(uID);
		}

		// Awake GameObject
		protected virtual void Awake() {
			this.controlData = new CObjectData ();
			this.m_CountDownFixedTimeSync = new CountdownTime (m_FixedTimeSync, true);
			this.uID = string.Empty;
		}

		// Start GameObject
		protected virtual void Start () {
			m_NetworkManager = CNetworkManager.GetInstance ();

			this.OnCreateControlObject ();
		}

		// Active on Server
		public virtual void OnServerLoadedObject ()
		{
			m_ObjectSyn.SetLocalUpdate (false);
			m_ObjectSyn.SetOtherInteractive (true);
			m_ObjectSyn.SetDataUpdate (true);
			Init ();
		}

		// Active On local and is local player
		public virtual void OnLocalPlayerLoadedObject  ()
		{
			m_ObjectSyn.SetLocalUpdate (true);
			m_ObjectSyn.SetOtherInteractive (false);
			m_ObjectSyn.SetDataUpdate (false);
			m_NetworkManager.OnClientRegisterEntity (this);
			Init ();
		}

		// Active On local and is client
		public virtual void OnClientLoadedObject ()
		{
			m_ObjectSyn.SetLocalUpdate (false);
			m_ObjectSyn.SetOtherInteractive (false);
			m_ObjectSyn.SetDataUpdate (false);
			m_NetworkManager.OnClientRegisterEntity (this);
			Init ();
		}

		// Mono Update
		protected virtual void Update () {
			this.OnServerUpdateBaseTime (Time.deltaTime);
			this.OnClientUpdateBaseTime (Time.deltaTime);
		}

		// Mono FixedUpdate
		protected virtual void FixedUpdate() {
			this.OnServerFixedUpdateBaseTime (Time.fixedDeltaTime);
			this.OnClientFixedUpdateBaseTime (Time.fixedDeltaTime);
		}

		// Mono Destroy
		[ClientCallback]
		protected virtual void OnDestroy() {
			
		}

		// Mono Application Quit
		[ClientCallback]
		public virtual void OnApplicationQuit() {
			m_NetworkManager.StopClient ();
		}

		// Mono Application Focus
		[ClientCallback]
		public virtual void OnApplicationFocus(bool value) {
#if UNITY_ANDROID
			if (value == false) {
				m_NetworkManager.StopClient ();
			}
#endif
		}

		// Mono Application Pause
		[ClientCallback]
		public virtual void OnApplicationPause(bool value) {
#if UNITY_ANDROID
			if (value == true) {
				m_NetworkManager.StopClient ();
			}
#endif
		}

		#endregion

		#region Server

		// On Server Update Base time
		[ServerCallback]
		public virtual void OnServerUpdateBaseTime(float dt) {
			this.m_ServerObjectTime += dt;
		}

		// On Server FixedUpdate Base time
		[ServerCallback]
		public virtual void OnServerFixedUpdateBaseTime(float dt) {
			if (m_ObjectSyn == null)
				return;
			var onTime = 0f;
			if (m_CountDownFixedTimeSync.UpdateTime (dt, out onTime)) {
				// Update sync data
				m_MovePosition = m_ObjectSyn.GetMovePosition ();
				m_Position = m_ObjectSyn.GetPosition ();
				m_Rotation = m_ObjectSyn.GetRotation ();
				// Server sync data
				OnServerFixedUpdateSynData (dt);
				// Update client
				RpcFixedUpdateClientSyncTime (onTime);
			}
		}

		[ServerCallback]
		public virtual void OnServerFixedUpdateSynData(float dt) {
			// Update Info
			RpcUpdateInfo (m_ObjectSyn.GetID()); 
			// Update control Data
			RpcUpdateControlData (this.m_ObjectSyn.GetActive(), this.m_ObjectSyn.GetEnable(),
				this.controlData.modelPath, this.controlData.fsmPath,
				m_ObjectSyn.GetCurrentHealth(), m_ObjectSyn.GetMaxHealth(), 
				m_ObjectSyn.GetMoveSpeed(), m_ObjectSyn.GetSeekRadius(),
				(int)this.controlData.objectType,
				this.m_ObjectSyn.GetFSMStateName());
			// Update transform
			RpcUpdateTransform (m_ObjectSyn.GetMovePosition(), m_ObjectSyn.GetPosition (), m_ObjectSyn.GetRotation());
			// Update animation
			RpcUpdateAnimation ((int) m_ObjectSyn.GetAnimation (), m_ObjectSyn.GetAnimationTime ());
			// Interactive
			var targetAttack = m_ObjectSyn.GetTargetInteract ();
			if (targetAttack != null && targetAttack.GetActive ()) {
				this.m_TargetInteractiveId = targetAttack.GetID ();
				RpcUpdateTargetInteractive (this.m_TargetInteractiveId);
			} else {
				this.m_TargetInteractiveId = "-1";
				RpcUpdateTargetInteractive ("-1");
			}
		}

		// On Server Destroy Object 
		[ServerCallback]
		public virtual void OnServerDestroyObject() {
			m_ObjectSyn.SetActive (false);
			m_ObjectSyn.OnDestroyObject ();
		}

		#endregion

		#region Client

		// On Client Update
		[ClientCallback]
		public virtual void OnClientUpdateBaseTime(float dt) {
			m_ClientObjectTime += dt;
		}

		// On Client Fixed Update
		[ClientCallback]
		public virtual void OnClientFixedUpdateBaseTime(float dt) {
			
		}

		[ClientCallback]
		public virtual void OnClientFixedUpdateSyncTime(float dt) {
			if (m_NetworkManager == null || m_ObjectSyn == null)
				return;
			CObjectController targetInteractive = null;
			if (this.m_TargetInteractiveId.Equals ("-1") == false) {
				var targetEntity = m_NetworkManager.FindEntity (m_TargetInteractiveId);
				if (targetEntity != null) {
					var controller = targetEntity.GetController () as CObjectController;
					targetInteractive = controller;
				} 
			}
			m_ObjectSyn.SetTargetInteract (targetInteractive);
		}

		// On Clients Network Destroy
		public override void OnNetworkDestroy ()
		{
			base.OnNetworkDestroy ();
			m_ObjectSyn.OnDestroyObject ();
		}

		public virtual void OnClientUpdateTransform() {
			if (m_ObjectSyn == null)
				return;
			// Move Position
			m_ObjectSyn.SetMovePosition (m_MovePosition);
			// Transform
			if (m_Position != m_ObjectSyn.GetPosition()) {
				var lerpPosition = Vector3.Lerp (m_ObjectSyn.GetPosition (), m_Position, 0.5f);
				m_ObjectSyn.SetPosition (lerpPosition);
			}
			if (m_Rotation != m_ObjectSyn.GetRotation ()) {
				var lerpRotation = Vector3.Lerp (m_ObjectSyn.GetRotation (), m_Rotation, 0.5f);
				m_ObjectSyn.SetRotation (lerpRotation);
			}
		}

		public virtual void OnClientUpdateAnimation() {
			if (m_ObjectSyn == null)
				return;
			var animation = (CEnum.EAnimation)m_Animation;
			if (animation != m_ObjectSyn.GetAnimation ()) {
				m_ObjectSyn.SetAnimation (animation);
			}
			if (m_AnimationTime != m_ObjectSyn.GetAnimationTime ()) {
				var animLerpTime = Mathf.Lerp (m_ObjectSyn.GetAnimationTime (), m_AnimationTime, 0.5f);
				m_ObjectSyn.SetAnimationTime (animLerpTime);
			}
		}

		#endregion

		#region Main methods

		public virtual void OnCreateControlObject() {
			if (m_ObjectSyn == null) {
				CHandleEvent.Instance.AddEvent (HandleOnCreateControlObject(), null);
			}
		}

		private IEnumerator HandleOnCreateControlObject() {
			while (string.IsNullOrEmpty (this.controlData.modelPath) 
				&& string.IsNullOrEmpty (this.controlData.fsmPath)) {
				yield return WaitHelper.WaitFixedUpdate;
			}
			var goObj = Instantiate (CResourceManager.Instance.LoadResourceOrAsset <GameObject> (this.controlData.modelPath));
			this.LoadObjectSync (goObj);
			yield return goObj != null;
			if (this.isServer) {
				OnServerLoadedObject ();
			} else {
				if (this.isLocalPlayer) {
					OnLocalPlayerLoadedObject ();
				} else {
					OnClientLoadedObject ();
				}
			}
		}

		protected virtual void LoadObjectSync (GameObject value) {
			m_ObjectSyn = value.GetComponent<IStatus> ();
		}

		#endregion

		#region Command

		#endregion

		#region RPC

		// RPC Entity info
		[ClientRpc]
		internal virtual void RpcUpdateInfo(string id) {
			this.SetID (id);
		}

		// RPC Control Data
		[ClientRpc]
		internal virtual void RpcUpdateControlData(bool active, bool enable,
			string modelPath, string fsmPath,
			int currentHealth, int maxHealth, 
			float moveSpeed, float seekRadius,
			int objectType,
			string fsmStateName) {
			if (m_ObjectSyn != null) {
				m_ObjectSyn.SetActive (active);
				m_ObjectSyn.SetEnable (enable);
				if (fsmStateName != m_ObjectSyn.GetFSMStateName ()) {
					m_ObjectSyn.SetFSMStateName (fsmStateName);
				}
				m_ObjectSyn.SetCurrentHealth (currentHealth);
				m_ObjectSyn.SetMaxHealth (maxHealth);
				m_ObjectSyn.SetMoveSpeed (moveSpeed);
				m_ObjectSyn.SetSeekRadius (seekRadius);
				m_ObjectSyn.SetObjectType ((CEnum.EObjectType) objectType);
			}
			this.controlData.modelPath = modelPath;
			this.controlData.fsmPath = fsmPath;
			this.controlData.currentHealth = currentHealth;
			this.controlData.maxHealth = maxHealth;
		}

		// RPC Update transform
		[ClientRpc]
		internal virtual void RpcUpdateTransform(Vector3 movePosition, Vector3 position, Vector3 rotation) {
			this.m_MovePosition = movePosition;
			this.m_Position = position;
			this.m_Rotation = rotation;
			// Transform
			OnClientUpdateTransform();
		}

		// RPC Update Animation
		[ClientRpc]
		internal virtual void RpcUpdateAnimation(int anim, float animTime) {
			this.m_Animation = anim;
			this.m_AnimationTime = animTime;
			// Animation
			OnClientUpdateAnimation();
		}

		// RPC On Client Fixed Update Sync time
		[ClientRpc]
		internal virtual void RpcFixedUpdateClientSyncTime(float dt) {
			// Client Update  
			OnClientFixedUpdateSyncTime(dt);
		}

		[ClientRpc]
		internal virtual void RpcUpdateTargetInteractive(string id) {
			this.m_TargetInteractiveId = id;
		}

		#endregion

		#region Getter && Setter

		public virtual void SetObjectSync(IStatus value) {
			this.m_ObjectSyn = value;
		}

		public virtual IStatus GetObjectSync() {
			return this.m_ObjectSyn;
		}

		public virtual void SetPosition(Vector3 position) {
			this.m_Position = position;
			if (m_ObjectSyn == null)
				return;
			this.m_ObjectSyn.SetPosition (position);
		}

		public virtual void SetStartPosition(Vector3 position) {
			this.m_StartPosition = position;
			if (m_ObjectSyn == null)
				return;
			this.m_ObjectSyn.SetStartPosition (position);
		}

		public virtual string GetID() {
			return uID;
		}

		public virtual void SetID(string value) {
			this.uID = value;
			if (m_ObjectSyn == null)
				return;
			m_ObjectSyn.SetID (value);
		}

		public virtual object GetController() {
			if (m_ObjectSyn == null)
				return null;
			return m_ObjectSyn.GetController ();
		}

		#endregion

	}

}
