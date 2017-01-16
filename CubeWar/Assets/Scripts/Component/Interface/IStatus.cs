using UnityEngine;
using System;
using System.Collections;

namespace CubeWar {
	public interface IStatus : IObjectInfo, ICommunicate, IObjectRender {

		#region Animation

		void SetAnimation (CEnum.EAnimation value);
		CEnum.EAnimation GetAnimation();

		void SetAnimationTime (float value);
		float GetAnimationTime();

		void SetAction (CEnum.EAnimation value);
		CEnum.EAnimation GetAction();

		#endregion

		bool GetActive();
		void SetActive(bool value);

		bool GetEnable();
		void SetEnable(bool value);

		bool GetUnderControl();
		void SetUnderControl(bool value);

		bool GetLocalUpdate();
		void SetLocalUpdate(bool value);

		bool GetDataUpdate();
		void SetDataUpdate(bool value);

		bool GetOtherInteractive();
		void SetOtherInteractive(bool value);

		string GetFSMStateName();
		void SetFSMStateName(string value);
		string GetFSMName();

		float GetWaitingTime ();
		void SetWaitingTime (float value);

		#region Transform

		Vector3 GetPosition ();
		void SetPosition(Vector3 position);

		Vector3 GetRotation ();
		void SetRotation(Vector3 rotation);

		Vector3 GetMovePosition ();
		void SetMovePosition(Vector3 position);

		Vector3 GetStartPosition ();
		void SetStartPosition(Vector3 position);

		#endregion

		void SetTargetInteract(CObjectController value);
		CObjectController GetTargetInteract();

		void UpdateFSM (float dt);
		void UpdateTouchInput (float dt);
		void UpdateSelectionObject (Vector3 originPoint, Vector3 directionPoint);
		void UpdateMoveDirection (Vector3 directionPoint);
		void UpdateActionInput (int value);
		void UpdateCollider (float dt);

		void ExecuteInventoryItem (object value);

		void OnDestroyObject ();

		string GetToken ();
		void SetToken(string value);

		void AddEventListener (string name, Action<object> onEvent);
		void RemoveEventListener (string name, Action<object> onEvent);
	}
}
