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

		protected int m_ServerRandomSeed;
		protected int m_ServerRandomTimes;

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
		}

		public override void OnStartLocalPlayer ()
		{
			base.OnStartLocalPlayer ();
			this.Init ();
			CmdRequestInit ();
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
			if (m_ServerRandomTimes != this.m_GameManager.randomTimes) {
				m_ServerRandomTimes = this.m_GameManager.randomTimes;
				RpcRandomTimes (m_ServerRandomTimes);
			}
		}

		// On Server FixedUpdate Base time
		[ServerCallback]
		public virtual void OnServerFixedUpdateBaseTime(float dt) {
			
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

		[Command]
		internal void CmdRequestInit() {
			RpcRequestInit (m_ServerRandomSeed);
		}

		#endregion

		#region Rpc

		[ClientRpc]
		internal void RpcRequestInit(int seed) {
			m_ServerRandomSeed = seed;
			this.m_GameManager.OnInitMap (m_ServerRandomSeed);
		}

		internal void RpcRandomTimes(int random) {
			m_ServerRandomTimes = random;
			this.m_GameManager.Rerandom (random);
		}

		#endregion
	}
}
