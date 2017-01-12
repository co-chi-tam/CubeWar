using UnityEngine;
using System.Collections;

namespace CubeWar {
	public interface IEntity {

		void Init ();
		void OnServerUpdateBaseTime (float dt);
		void OnServerFixedUpdateBaseTime (float dt);

		void OnClientUpdateBaseTime(float dt);
		void OnClientFixedUpdateBaseTime(float dt);

		void OnNetworkDestroy ();

	}
}
