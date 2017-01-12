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

		protected int m_ServerRandomSeed = -1;
		protected int m_ServerRandomTimes = -1;

		private CountdownTime m_RespawnCountdownTime;

		#endregion

		#region Monobehaviour

		public virtual void Init() {
			this.m_NetworkManager = CNetworkManager.GetInstance ();
			this.m_GameManager = CGameManager.GetInstance ();
			this.m_GameManager.GameMode = CEnum.EGameMode.MultiPlayer;
		}

		protected virtual void Awake() {
			
		}

		public override void OnStartServer ()
		{
			base.OnStartServer ();
			this.Init ();
			this.m_ServerRandomSeed = 66778899;
			this.m_GameManager.OnInitMap (m_ServerRandomSeed);

			CObjectManager.Instance.OnSetObject -= this.OnServerReturnPool;
			CObjectManager.Instance.OnSetObject += this.OnServerReturnPool;

			m_RespawnCountdownTime = new CountdownTime (this.m_GameManager.respawnTime, true);
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

		private void OnGUI() {
			GUI.Label (new Rect (0f, 0f, 150f, 30f), "randomTimes " + this.m_GameManager.randomTimes);
		}

		#endregion

		#region Server

		// On Server Update Base time
		[ServerCallback]
		public virtual void OnServerUpdateBaseTime(float dt) {
			if (this.m_GameManager.init == false)
				return;
			RpcRequestInit (m_ServerRandomSeed, this.m_GameManager.randomTimes);
			// Update random times
			if (m_ServerRandomTimes != this.m_GameManager.randomTimes) {
				m_ServerRandomTimes = this.m_GameManager.randomTimes;
				RpcRandomTimes (m_ServerRandomTimes);
			}
			// Spawn cube
			if (m_RespawnCountdownTime.UpdateTime (dt)) {
				this.m_GameManager.RespawnCube ();
				RpcRespawnCube ();
			}
		}

		// On Server FixedUpdate Base time
		[ServerCallback]
		public virtual void OnServerFixedUpdateBaseTime(float dt) {
			
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

		#endregion

		#region Command

		#endregion

		#region Rpc

		[ClientRpc]
		internal void RpcReturnPool(string name) {
			this.m_GameManager.OnObjectReturnPool (name, null);
		}

		[ClientRpc]
		internal void RpcRequestInit(int seed, int randomTimes) {
			if (m_ServerRandomSeed != seed && m_ServerRandomTimes != randomTimes) {
				this.m_GameManager.Rerandom (randomTimes);
				this.m_GameManager.OnInitMap (seed);
			}
			this.m_GameManager.randomTimes = randomTimes;
			m_ServerRandomSeed = seed;
			m_ServerRandomTimes = randomTimes;
		}

		[ClientRpc]
		internal void RpcRandomTimes(int random) {
			m_ServerRandomTimes = random;
			if (this.m_GameManager.init) {
				this.m_GameManager.Rerandom (random);
			}
		}

		[ClientRpc]
		internal void RpcRespawnCube() {
			if (this.m_GameManager.init) {
				this.m_GameManager.RespawnCube ();
			}
		}

		#endregion
	}
}
