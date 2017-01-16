using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CubeWar {

	[RequireComponent(typeof(NetworkIdentity))]
	public class CGameManagerEntity : NetworkBehaviour, IEntity {

		#region Properties

		protected CGameManager m_GameManager;
		protected CNetworkManager m_NetworkManager;

		// Sync
		protected int m_ServerRandomSeed = -1;
		[SerializeField]	protected int m_ServerUpdateCubeIndex = -1;
		// Sync time
		protected float m_FixedTimeSync = 0.1f;
		protected CountdownTime m_CountDownFixedTimeSync;

		#endregion

		#region Monobehaviour

		public virtual void Init() {
			this.m_NetworkManager = CNetworkManager.GetInstance ();
			this.m_GameManager = CGameManager.GetInstance ();
			this.m_GameManager.GameMode = CEnum.EGameMode.MultiPlayer;
		}

		protected virtual void Awake() {
			m_CountDownFixedTimeSync = new CountdownTime (m_FixedTimeSync, true);
		}

		public override void OnStartServer ()
		{
			base.OnStartServer ();
			this.Init ();
			this.m_ServerRandomSeed = 66778899;
			this.m_GameManager.OnInitMap (m_ServerRandomSeed, () => {
				this.m_GameManager.alreadyPlay = true;
			});

			CObjectManager.Instance.OnSetObject -= this.OnServerReturnPool;
			CObjectManager.Instance.OnSetObject += this.OnServerReturnPool;
		}

		public override void OnStartLocalPlayer ()
		{
			base.OnStartLocalPlayer ();
		}

		public override void OnStartClient ()
		{
			base.OnStartClient ();
			this.Init ();
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

		// On Clients Network Destroy
		public override void OnNetworkDestroy ()
		{
			base.OnNetworkDestroy ();
		}

		#endregion

		#region Server

		// On Server Update Base time
		[ServerCallback]
		public virtual void OnServerUpdateBaseTime(float dt) {
			if (this.m_GameManager.init == false)
				return;
			RpcRequestInit (m_ServerRandomSeed);
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
			if (m_GameManager.init == false)
				return;
			m_ServerUpdateCubeIndex = (m_ServerUpdateCubeIndex + 1) % m_GameManager.GetObjectCount ();
			var cubeCtrl = m_GameManager.GetObjectController (m_ServerUpdateCubeIndex);
			if (cubeCtrl != null) {
				RpcOnClientUpdateCubeController (m_ServerUpdateCubeIndex, cubeCtrl.GetPosition ());
			}
		}

		[ServerCallback]
		public virtual void OnServerReturnPool(string name, CBaseController controller) {
			this.m_GameManager.OnObjectReturnPool (name, controller);
			RpcReturnPool (name);
		}

		#endregion

		#region Client

		// On Client Update
		[ClientCallback]
		public virtual void OnClientUpdateBaseTime(float dt) {
			
		}

		// On Client Fixed Update
		[ClientCallback]
		public virtual void OnClientFixedUpdateBaseTime(float dt) {

		}


		[ClientCallback]
		public virtual void OnClientFixedUpdateSyncTime(float dt) {

		}

		#endregion

		#region Command

		#endregion

		#region Rpc

		[ClientRpc] 
		internal void RpcOnClientUpdateCubeController(int index, Vector3 position) {
			if (this.m_GameManager.init == false)
				return;
			m_ServerUpdateCubeIndex = index;
			var cubeCtrl = this.m_GameManager.GetObjectController (index);
			if (cubeCtrl != null) {
				cubeCtrl.SetPosition (position);
				cubeCtrl.SetActive (true);
			}
		}

		[ClientRpc]
		internal void RpcReturnPool(string name) {


		}

		[ClientRpc]
		internal void RpcRequestInit(int seed) {
			if (m_ServerRandomSeed != seed) {
				this.m_GameManager.OnInitMap (seed, () => {
					this.m_GameManager.alreadyPlay = true;
				});				
			}
			m_ServerRandomSeed = seed;
		}

		// RPC On Client Fixed Update Sync time
		[ClientRpc]
		internal virtual void RpcFixedUpdateClientSyncTime(float dt) {
			// Client Update  
			OnClientFixedUpdateSyncTime(dt);
		}

		#endregion
	}
}
