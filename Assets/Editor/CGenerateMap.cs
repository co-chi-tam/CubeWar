using UnityEngine;
using UnityEditor;
using System.Collections;

namespace CubeWar {
	[CustomEditor(typeof (CGameManager))]
	public class CGenerateMap : Editor {

		private GameObject m_MapRoot;
		private int m_CubeCount = 100;

		private CGameManager m_GameManager;

		public override void OnInspectorGUI() {
			DrawDefaultInspector ();
			m_GameManager = target as CGameManager;
			GUILayout.Space (10f);
			m_MapRoot = EditorGUILayout.ObjectField ("Map root", m_MapRoot, typeof(GameObject), true) as GameObject;
			m_CubeCount = EditorGUILayout.IntField ("Cube count", m_CubeCount);
			if (GUILayout.Button ("Generate Map")) {
				var cubePrefab = (GameObject) AssetDatabase.LoadAssetAtPath ("Assets/Resources/Prefabs/Cube/SmallCube.prefab", typeof(GameObject));
				if (cubePrefab != null) {
					for (int i = 0; i < m_CubeCount; i++) {
						var cubeObj = PrefabUtility.InstantiatePrefab (cubePrefab) as GameObject;
						if (m_MapRoot != null) {
							cubeObj.transform.SetParent (m_MapRoot.transform);
						}
						cubeObj.transform.position = m_GameManager.GetMapRandomPosition ();
					}
				} else {
					Debug.Log ("NULL PREFAB");
				}
			}
		}

	}
}
