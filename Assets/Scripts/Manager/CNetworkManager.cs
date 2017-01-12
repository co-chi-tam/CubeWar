﻿using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System;
using System.Collections;
using System.Collections.Generic;
using FSM;

namespace CubeWar {
	public class CNetworkManager : NetworkManager {

		#region Singleton

		protected static CNetworkManager m_Instance;
		private static object m_SingletonObject = new object();
		public static CNetworkManager Instance {
			get { 
				lock (m_SingletonObject) {
					if (m_Instance == null) {
						var go = new GameObject ();
						m_Instance = go.AddComponent<CNetworkManager> ();
						go.SetActive (true);
						go.name = m_Instance.GetType().Name;
					}
					return m_Instance;
				}
			}
		}

		public static CNetworkManager GetInstance() {
			return Instance;
		}

		#endregion

		#region Properties

		private Dictionary<string, CEntity> m_RegisterEntities;
		private Dictionary<NetworkConnection, CEntity> m_EntityConnecteds;
		private int m_PlayerCount;

		protected NetworkClient m_CurrentClient;

		public static string SERVER_IP = "192.168.0.129";
		public static int SERVER_PORT = 7766;

		public enum EMsgType : short
		{
			RegisterPlayer = 1001
		}

		public enum EEntityType : short
		{
			PlayableEntity = 0,
			GameManagerEntity = 1
		}

		#endregion

		#region MonoBehaviour

		protected virtual void Awake() {
			m_Instance = this;
			m_RegisterEntities = new Dictionary<string, CEntity> ();
			m_EntityConnecteds = new Dictionary<NetworkConnection, CEntity> ();
		}

		protected virtual void Start() {
			this.networkAddress = SERVER_IP;
			this.networkPort = SERVER_PORT;
			this.StartServer ();
		}

		#endregion

		#region Main Methods

		public virtual CEntity FindEntity(string id) {
			if (m_RegisterEntities.ContainsKey (id)) {
				return m_RegisterEntities [id];
			}
			return null;
		}

		public virtual bool OnServerRegisterEntity(CEntity entity, NetworkConnection conn) {
			if (m_RegisterEntities.ContainsKey (entity.GetID ()) == true)
				return false;
			entity.SetID(Guid.NewGuid().ToString());
			m_RegisterEntities.Add (entity.GetID (), entity);
			if (conn != null) {
				if (m_EntityConnecteds.ContainsKey (conn) == true)
					return false;
				m_EntityConnecteds.Add (conn, entity);
			}
			return true;
		}

		public virtual bool OnClientRegisterEntity(CEntity entity) {
			if (m_RegisterEntities.ContainsKey (entity.GetID ()) == true)
				return false;
			m_RegisterEntities.Add (entity.GetID(), entity);
			return true;
		}

		#endregion

		#region Server

		public override void OnStartServer ()
		{
			base.OnStartServer ();
		}

		public override void OnServerSceneChanged (string sceneName)
		{
			base.OnServerSceneChanged (sceneName);
			OnServerAddMapObject ("WorldMap0001");
			NetworkServer.RegisterHandler ((short) EMsgType.RegisterPlayer, OnServerRegisterPlayer);
		}

		public virtual void OnServerAddMapObject(string mapPath) {
			// Game entity manager
			var gameManagerGO = (GameObject)GameObject.Instantiate (spawnPrefabs [(int)EEntityType.GameManagerEntity], Vector3.zero, Quaternion.identity);
			gameManagerGO.name = "Network-" + EEntityType.GameManagerEntity.ToString();
			NetworkServer.Spawn (gameManagerGO);
		}

		public override void OnServerAddPlayer (NetworkConnection conn, short playerControllerId)
		{
//			base.OnServerAddPlayer (conn, playerControllerId);
			m_PlayerCount ++;
			var player = (GameObject)GameObject.Instantiate(spawnPrefabs[(int)EEntityType.PlayableEntity], 
				Vector3.zero, 
				Quaternion.identity);
			if (NetworkServer.AddPlayerForConnection (conn, player, playerControllerId)) {
				var entity = player.GetComponent<CPlayableEntity> ();
				OnServerRegisterEntity (entity, conn);
			}
		}

		public override void OnServerDisconnect (NetworkConnection conn)
		{
			base.OnServerDisconnect (conn);
			if (m_EntityConnecteds.ContainsKey (conn)) {
				var player = m_EntityConnecteds [conn];
				player.OnServerDestroyObject ();
				m_EntityConnecteds.Remove (conn);
			}
			NetworkServer.DestroyPlayersForConnection (conn);
		}

		public override void OnServerError (NetworkConnection conn, int errorCode)
		{
			base.OnServerError (conn, errorCode);
		}

		public virtual void OnServerRegisterPlayer(NetworkMessage netMsg) {
			
		}

		#endregion

		#region Client

		public virtual void OnClientLoginComplete(CUserData player) {
			this.networkAddress = SERVER_IP;
			this.networkPort = SERVER_PORT;
			m_CurrentClient = this.StartClient ();
		}

		public override void OnStartClient (NetworkClient client)
		{
			base.OnStartClient (client);
		}

		public override void OnClientSceneChanged (NetworkConnection conn)
		{
			base.OnClientSceneChanged (conn);
//			this.OnClientRegisterPlayer (m_CurrentClient, CUserManager.Instance.CurrentUser);
		}

		public virtual void OnClientRegisterPlayer(NetworkClient client, CUserData playerData) {
			var msgRegister = new CMsgRegisterPlayer ();
			msgRegister.userData = playerData;
			client.Send ((short) EMsgType.RegisterPlayer, msgRegister);
		}

		public override void OnClientConnect (NetworkConnection conn)
		{
			base.OnClientConnect (conn);
		}

		public override void OnClientDisconnect (NetworkConnection conn)
		{
			base.OnClientDisconnect (conn);
			this.StopClient ();
		}

		#endregion

	}
}