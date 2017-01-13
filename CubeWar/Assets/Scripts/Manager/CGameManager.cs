using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CubeWar {
	public class CGameManager : CMonoSingleton<CGameManager>, ITask {

		[SerializeField]	private Terrain m_MapTerrain;
		[SerializeField]	private Vector2 m_MapSize;
		[SerializeField]	private int m_ObjectCount = 500;
		[SerializeField]	private float m_MapRadiusBound = 10f;

		public CEnum.EGameMode GameMode = CEnum.EGameMode.Offline;
		public bool init;
		public bool alreadyPlay;

		private CObjectManager m_ObjectManager;
		private List<CObjectController> m_ActiveCubes;

		protected override void Awake ()
		{
			base.Awake ();
#if UNITY_ANDROID
			Application.targetFrameRate = 30;
#endif
			m_ActiveCubes = new List<CObjectController> ();
			m_MapTerrain = GameObject.Find ("WorldMap/Terrain").GetComponent<Terrain>();
			if (m_MapTerrain != null) {
				m_MapSize = new Vector2 (m_MapTerrain.terrainData.size.x - m_MapRadiusBound
					, m_MapTerrain.terrainData.size.z - m_MapRadiusBound);
			}
		}

		protected override void Start ()
		{
			base.Start ();
			m_ObjectManager = CObjectManager.GetInstance ();


			m_ObjectManager.OnGetObject -= OnObjectPeekPool;
			m_ObjectManager.OnGetObject += OnObjectPeekPool;

			if (GameMode == CEnum.EGameMode.Offline) {
				m_ObjectManager.OnSetObject -= OnObjectReturnPool;
				m_ObjectManager.OnSetObject += OnObjectReturnPool;
			}

			CSceneManager.Instance.OnRegisterTask (this);
			if (GameMode == CEnum.EGameMode.Offline) {
				this.OnInitMap ((int)DateTime.Now.Ticks, () =>  {
					alreadyPlay = true;
				});
			} 
		}

		public override void UpdateBaseTime (float dt)
		{
			base.UpdateBaseTime (dt);
		}

		public void OnInitMap(int seed, Action complete = null) {
			UnityEngine.Random.InitState (seed);
			OnInitMap (complete);
		}

		public void OnInitMap(Action complete = null) {
			init = false;
			CHandleEvent.Instance.AddEvent (this.HandleOnInitMap(() => {
				init = true;
				if (complete != null) {
					complete ();
				}
			}), null);
		}

		public void OnObjectPeekPool(string name, CBaseController controller) {
			var objCtrl = controller as CObjectController;
			if (m_ActiveCubes.Contains (objCtrl) == false) {
				m_ActiveCubes.Add (objCtrl);
			}
		}

		public void OnObjectReturnPool(string name, CBaseController controller) {
			var randomPos = this.GetMapRandomPosition();
			var cubeInPool = m_ObjectManager.GetObject ("SmallCube");
			cubeInPool.SetPosition(randomPos);
			cubeInPool.SetActive (true);
		}

		private IEnumerator HandleOnInitMap(Action complete) {
			for (int i = 0; i < m_ObjectCount; i++) {
				CCubeController cubeCtrl = null;
				m_ObjectManager.GetObjectModified ("SmallCube", (x) => {
					cubeCtrl = x as CCubeController;
					var randomPos = this.GetMapRandomPosition();
					cubeCtrl.SetPosition(randomPos);
					cubeCtrl.SetActive (true);
					return cubeCtrl;
				});
				yield return cubeCtrl != null;
			}
			if (complete != null) {
				complete ();
			}
		}

		public void RespawnCube() {
			for (int i = 0; i < m_ActiveCubes.Count; i++) {
				var randomPos = this.GetMapRandomPosition();
				var cubeCtrl = m_ActiveCubes [i];
				if (cubeCtrl.GetActive () == false) {
					var cubeInPool = m_ObjectManager.GetObject ("SmallCube");
					cubeInPool.SetPosition(randomPos);
					cubeInPool.SetActive (true);
				} else {
					cubeCtrl.SetPosition(randomPos);
					cubeCtrl.SetActive (true);
				}
			}
		}

		public override bool OnTask ()
		{
			return init && alreadyPlay;
		}

		public Vector3 GetMapRandomPosition() {
			return this.GetMapRandomPosition(m_MapSize);
		}

		public Vector3 GetMapRandomPosition(Vector2 size) {
			var randomCircle = UnityEngine.Random.insideUnitCircle;
			var x = randomCircle.x * size.x;
			var y = this.transform.position.y;
			var z = randomCircle.y * size.y;
			var targetPosition = new Vector3 (x, y, z);
			var path = new NavMeshPath ();
			if (NavMesh.CalculatePath (transform.position, targetPosition, NavMesh.AllAreas, path)) {
				if (path.status == NavMeshPathStatus.PathComplete) {
					return targetPosition;
				} else {
					return GetMapRandomPosition(size);
				}
			} else {
				return GetMapRandomPosition(size);
			}
			return Vector3.zero;
		}

		public int GetObjectCount() {
			return m_ObjectCount;
		}

		public int GetObjectActiveCount() {
			return m_ActiveCubes.Count;
		}

		public CObjectController GetObjectController(int index) {
			if (index > m_ActiveCubes.Count - 1)
				return null;
			return m_ActiveCubes [index];
		}

	}
}
