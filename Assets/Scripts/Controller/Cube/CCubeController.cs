using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using FSM;

namespace CubeWar {
	public partial class CCubeController : CObjectController {

		#region Properties
		[Header("Mesh")]
		[SerializeField]	protected Renderer m_CubeMesh;
		[Header("Data text")]
		[SerializeField]	protected TextAsset m_DataText;
		[Header("Interactive")]
		[SerializeField]	protected CObjectController m_TargetInteract;

		protected CCubeData m_Data;

		#endregion

		#region Monobehavour

		protected override void Awake ()
		{
			base.Awake ();
		}

		protected override void Start ()
		{
			base.Start ();
			this.SetActive (true);
			this.SetEnable (true);
			this.SetMovePosition (this.GetPosition());
			this.SetStartPosition (this.GetPosition ());
		}

		public override void OnBecameVisible ()
		{
			if (this.GetOtherInteractive () == false)
				return;
			base.OnBecameVisible ();
			this.m_CapsuleCollider.enabled = true;
			this.m_AnimatorController.SetEnable (true);
		}

		public override void OnBecameInvisible ()
		{
			if (this.GetOtherInteractive () == false)
				return;
			base.OnBecameInvisible ();
			this.m_CapsuleCollider.enabled = false;
			this.m_AnimatorController.SetEnable (false);
		}

		protected override void OnLoadData ()
		{
			base.OnLoadData ();
			if (m_DataText != null) {
				m_Data = TinyJSON.JSON.Load (m_DataText.text).Make<CCubeData>();
			}
		}

		#endregion

		#region Main methods

		public override void UpdateFSM(float dt) {
			base.UpdateFSM (dt);
			m_FSMManager.UpdateState (dt);
		}

		public override void ResetAll ()
		{
			this.SetTargetInteract (null);
			if (this.GetActive () == false)
				return;
			base.ResetAll ();
			this.SetAnimation (CEnum.EAnimation.Idle);
		} 

		public override void ResetPerAction() {
			base.ResetPerAction ();
		}

		public override void DisableObject (string animationName)
		{
//			base.DisableObject (animationName);
		}

		#endregion

		#region Getter && Setter

		public override string GetID ()
		{
			base.GetID ();
			if (m_Data == null)
				return base.GetID ();
			return m_Data.id;
		}

		public override void SetID (string value)
		{
			base.SetID (value);
			m_Data.id = value;
		}

		public override void SetData (CObjectData value)
		{
			base.SetData (value);
			m_Data = value as CCubeData;
		}

		public override CObjectData GetData ()
		{
			base.GetData ();
			return m_Data;
		}

		public override void SetName (string value)
		{
			base.SetName (value);
			m_Data.name = value;
		}

		public override string GetName ()
		{
			if (m_Data != null)
				return m_Data.name;
			return base.GetName ();
		}

		public override void SetAvatar (string value)
		{
			base.SetAvatar (value);
			m_Data.avatarPath = value;
		}

		public override string GetAvatar ()
		{
			base.GetAvatar ();
			return m_Data.avatarPath;
		}

		public override void SetCurrentHealth (int value)
		{
			base.SetCurrentHealth (value);
			m_Data.currentHealth = value;
			m_EventComponent.InvokeEventListener("HealthChange", (float)value / this.GetMaxHealth ());
		}

		public override int GetCurrentHealth ()
		{
			base.GetCurrentHealth ();
			return m_Data.currentHealth;
		}

		public override void SetMaxHealth (int value)
		{
			base.SetCurrentHealth (value);
			m_Data.maxHealth = value;
		}

		public override int GetMaxHealth ()
		{
			base.GetMaxHealth ();
			return m_Data.maxHealth;
		}


		public override void SetAnimationTime (float value)
		{
			base.SetAnimationTime (value);
			m_AnimatorController.SetTime (value);
		}

		public override float GetAnimationTime ()
		{
			base.GetAnimationTime ();
			return m_AnimatorController.GetTime ();
		}

		public override float GetMoveSpeed ()
		{
			base.GetMoveSpeed ();
			return m_Data.moveSpeed;
		}

		public override void SetMoveSpeed (float value)
		{
			base.SetMoveSpeed (value);
			m_Data.moveSpeed = value;
		}

		public override float GetSeekRadius ()
		{
			base.GetSeekRadius ();
			return m_Data.seekRadius + this.GetSize();
		}

		public override void SetSeekRadius (float value)
		{
			base.SetSeekRadius (value);
			m_Data.seekRadius = value;
		}

		public override CEnum.EObjectType GetObjectType ()
		{
			return m_Data.objectType;
		}

		public override void SetObjectType (CEnum.EObjectType objectType)
		{
			base.SetObjectType (objectType);
			m_Data.objectType = objectType;
		}

		public override CEnum.EClassType GetClassType ()
		{
			base.GetClassType ();
			return m_Data.classType;
		}

		public override void SetClassType (CEnum.EClassType value)
		{
			base.SetClassType (value);
			m_Data.classType = value;
		}

		public override string GetToken() {
			base.GetToken ();
			return m_Data.token;
		}

		public override void SetToken(string value) {
			base.SetToken (value);
			m_Data.token = value;
		}

		public override void SetTargetInteract(CObjectController value) {
			base.SetTargetInteract (value);
			m_TargetInteract = value;
		}

		public override CObjectController GetTargetInteract() {
			base.GetTargetInteract ();
			if (m_TargetInteract == null || m_TargetInteract.GetActive () == false)
				return null;
			return m_TargetInteract;
		}

		public override void SetActive (bool value)
		{
//			base.SetActive (value);
			m_Active = value;
		}

		#endregion
	
	}
}
