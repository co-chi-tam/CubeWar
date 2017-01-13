using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CubeWar {
	[RequireComponent(typeof(NetworkIdentity))]
	public class CSyncEntity : NetworkBehaviour, IEntity {


		#region Properties

		// Sync time
		protected float m_FixedTimeSync = 0.1f;
		protected CountdownTime m_CountDownFixedTimeSync;
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
			
		}

		// Awake GameObject
		protected virtual void Awake() {
			this.m_CountDownFixedTimeSync = new CountdownTime (m_FixedTimeSync, true);
		}

		// Start GameObject
		protected virtual void Start () {

		}

		// Active on Server
		public virtual void OnServerLoadedObject ()
		{
			
		}

		// Active On local and is local player
		public virtual void OnLocalPlayerLoadedObject  ()
		{
			
		}

		// Active On local and is client
		public virtual void OnClientLoadedObject ()
		{
			
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
			
		}

		// Mono Application Focus
		[ClientCallback]
		public virtual void OnApplicationFocus(bool value) {

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
			var onTime = 0f;
			if (m_CountDownFixedTimeSync.UpdateTime (dt, out onTime)) {
				// Server sync data
				OnServerFixedUpdateSynData (dt);
				// Update client
				RpcFixedUpdateClientSyncTime (onTime);
			}
		}

		[ServerCallback]
		public virtual void OnServerFixedUpdateSynData(float dt) {
			
		}

		// On Server Destroy Object 
		[ServerCallback]
		public virtual void OnServerDestroyObject() {
			
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
			
		}

		// On Clients Network Destroy
		public override void OnNetworkDestroy ()
		{
			base.OnNetworkDestroy ();
		}

		#endregion

		#region Main methods

		#endregion

		#region Command

		#endregion

		#region RPC

		// RPC On Client Fixed Update Sync time
		[ClientRpc]
		internal virtual void RpcFixedUpdateClientSyncTime(float dt) {
			// Client Update  
			OnClientFixedUpdateSyncTime(dt);
		}

		#endregion

		#region Getter && Setter

		#endregion
	
	}
}
