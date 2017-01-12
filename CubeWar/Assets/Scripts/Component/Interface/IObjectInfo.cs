using UnityEngine;
using System;
using System.Collections;

namespace CubeWar {
	public interface IObjectInfo {

		string GetID();
		void SetID(string value);

		void SetObjectType (CEnum.EObjectType value);
		CEnum.EObjectType GetObjectType ();

		void SetClassType (CEnum.EClassType value);
		CEnum.EClassType GetClassType ();

		void SetName(string value);
		string GetName();

		void SetAvatar(string value);
		string GetAvatar();

		float GetSize();
		void SetSize (float value);
		float GetHeight();

		void SetCurrentHealth(int value);
		int GetCurrentHealth();
		void SetMaxHealth(int value);
		int GetMaxHealth();

		float GetMoveSpeed();
		void SetMoveSpeed(float value);

		float GetSeekRadius();
		void SetSeekRadius(float value);

		object GetController ();

	}
}

