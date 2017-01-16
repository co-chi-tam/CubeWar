using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using FSM;

namespace CubeWar {
	public partial class CPlayerCubeController : CCubeController {

		#region Properties

		[Header("NavMesh")]
		[SerializeField]	protected NavMeshAgent m_NavMeshAgent;
		[Header("Layer Mask")]
		[SerializeField]	protected LayerMask m_TerrainLayerMask;
		[SerializeField]	protected LayerMask m_TargetLayerMask;

		protected CMovableComponent m_MovableComponent;
		private bool m_TouchedUI;

		#endregion

		#region Monobehaviour

		protected override void Start ()
		{
			base.Start ();
			// FSM
			var fsmJson = CResourceManager.Instance.LoadResourceOrAsset <TextAsset> (m_Data.fsmPath);
			m_FSMManager.LoadFSM (fsmJson.text);

			// Random animation idle
			this.m_AnimatorController.RandomAnimationFrame();

			if (CGameManager.Instance.GameMode == CEnum.EGameMode.Offline) {
#if UNITY_EDITOR || UNITY_STANDALONE
				CUIControlManager.Instance.RegisterControl (false, this.ShowChat, this.ShowEmotion);	
#elif UNITY_ANDROID
				CUIControlManager.Instance.RegisterControl (true, this.ShowChat, this.ShowEmotion);			
#endif
			} else {
				CUIControlManager.Instance.RegisterUIInfo (this, true, true);
			}
		}

		public override void FixedUpdateBaseTime (float dt)
		{
			base.FixedUpdateBaseTime (dt);
			if (this.GetActive()) {
				UpdateFSM (dt);

				// Input
				if (Input.GetKeyDown (KeyCode.Q)) {
					UpdateActionInput ((int)CEnum.EAnimation.Action_1);
				}
			}
		}

		public override void OnBecameVisible ()
		{
			
		}

		public override void OnBecameInvisible ()
		{
			
		}

		protected override void OnDrawGizmos ()
		{
			base.OnDrawGizmos ();
			if (Application.isPlaying == false)
				return;
			Gizmos.color = Color.blue;
			Gizmos.DrawLine (this.GetPosition(), this.GetPosition () + (m_Transform.forward * 3f));
		}

		#endregion

		#region Main methods

		protected override void OnRegisterComponent() {
			base.OnRegisterComponent ();
			// Movable
			this.m_MovableComponent = new CMovableComponent (this, m_NavMeshAgent);
			this.m_MovableComponent.currentTransform = m_Transform;
			this.m_MovableComponent.targetPosition = m_MovePosition;
		}

		public override void MoveToTarget(Vector3 target, float dt) {
			base.MoveToTarget (target, dt);
			m_MovableComponent.targetPosition = target;
			m_MovableComponent.MoveForwardToTarget (dt);
		}

		public override void LookAtTarget(Vector3 target) {
			base.LookAtTarget (target);
			m_MovableComponent.LookForwardToTarget (target);
		}

		public override void UpdateTouchInput(float dt) {
			base.UpdateTouchInput (dt);
			if (this.GetUnderControl() == false)
				return;
#if UNITY_EDITOR || UNITY_STANDALONE
			if (Input.GetMouseButtonDown (0)) {
				if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) { return; }
				var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				UpdateSelectionObject(ray.origin, ray.direction);
			}
#elif UNITY_ANDROID
//			if (Input.touchCount == 1) {	
//				var touchPoint = Input.GetTouch (0);
//				m_TouchedUI = CUtil.IsPointerOverUIObject (touchPoint.position);
//				if (m_TouchedUI) { return; }
//				var ray = Camera.main.ScreenPointToRay (touchPoint.position);
//				UpdateSelectionObject(ray.origin, ray.direction);
//			}
			var direction = CUIControlManager.Instance.GetJoytickPoint();
			if (direction != Vector3.zero) {
				this.UpdateMoveDirection (direction);
			}
#endif
		}

		public override void UpdateSelectionObject(Vector3 originPoint, Vector3 directionPoint) {
			base.UpdateSelectionObject (originPoint, directionPoint);
			this.m_EventComponent.InvokeEventListener ("TouchScreenInput", new Vector3[] { originPoint, directionPoint });
			if (this.GetOtherInteractive() == false)
				return;
			RaycastHit hitInfo;
			if (Physics.Raycast (originPoint, directionPoint, out hitInfo, this.GetCurrentHealth(), m_TerrainLayerMask)) { // Object layermask
				this.SetMovePosition (hitInfo.point);
			}
		}

		public override void UpdateMoveDirection (Vector3 directionPoint)
		{
			base.UpdateMoveDirection (directionPoint);
			this.m_EventComponent.InvokeEventListener ("MoveDirectionInput", directionPoint);
			if (this.GetOtherInteractive() == false)
				return;
			this.SetMovePosition (directionPoint + this.GetPosition ());
		}

		public override void UpdateCollider (float dt)
		{
			base.UpdateCollider (dt);
			var colliders = Physics.OverlapSphere (this.GetPosition (), this.GetSize (), m_TargetLayerMask);
			if (colliders.Length > 0) {
				for (int i = 0; i < colliders.Length; i++) {
					var objController = colliders [i].GetComponent<CObjectController> ();
					if (objController != null
						&& objController.GetActive()
					    && objController != this) {
						if (objController.GetCurrentHealth () < this.GetCurrentHealth ()) {
							objController.ApplyDamage (this, objController.GetCurrentHealth (), CEnum.EElementType.Pure);
							if (objController.GetTargetInteract() == this) {
								if (this.GetOtherInteractive ()) {
									this.SetCurrentHealth (this.GetCurrentHealth () + objController.GetCurrentHealth ());
								}
							}
						}
					}
				}
			}
			var offsetSize = 1f / this.GetMaxHealth ();
			var currentSize = offsetSize * this.GetCurrentHealth ();
			this.SetSize (currentSize);
		}

		public override void UpdateActionInput (int value)
		{
			base.UpdateActionInput (value);
			this.m_EventComponent.InvokeEventListener ("ActionInput", value);
			if (this.GetOtherInteractive() == false)
				return;
			this.SetAction ((CEnum.EAnimation) value);
			if (this.GetAction () != CEnum.EAnimation.Idle) {
				this.SetAnimation (this.GetAction ());
			}
		}

		public override void ShowChat (string value)
		{
			base.ShowChat (value);
			this.SetChat (Time.time + ":=:" + value);
			this.m_EventComponent.InvokeEventListener ("ChatInput", this.GetChat());
		}

		public override void ShowEmotion (string value)
		{
			base.ShowEmotion (value);
			this.SetEmotion (Time.time + ":=:" + value);
			this.m_EventComponent.InvokeEventListener ("EmotionInput", this.GetEmotion());
		}

		public override void OnReturnObjectManager() {
			
		}

		#endregion

		#region FSM

		internal virtual bool HaveTargetInRange() {
			if (m_TargetInteract == null || m_TargetInteract.GetActive() == false)
				return false;
			var direction = m_TargetInteract.GetPosition () - this.GetPosition ();
			var distance = this.GetSeekRadius () * this.GetSeekRadius ();
			return direction.sqrMagnitude <= distance;
		}

		internal virtual bool DidMoveToTargetAttack() {
			if (m_TargetInteract == null)
				return false;
			m_MovableComponent.targetPosition = m_TargetInteract.GetPosition();
			return m_MovableComponent.DidMoveToTarget (m_TargetInteract.GetPosition());
		}

		internal virtual bool DidMoveToPosition() {
			m_MovableComponent.targetPosition = this.GetMovePosition ();
			return m_MovableComponent.DidMoveToTarget (this.GetMovePosition ());
		}

		internal virtual bool HaveTargetAttack() {
			if (m_TargetInteract == null)
				return false;
			return m_TargetInteract.GetActive();
		}

		#endregion

		#region Getter && Setter

		public override void SetUnderControl (bool value)
		{
			base.SetUnderControl (value);
			if (value && this.GetLocalUpdate()) {
				CameraController.Instance.SetTarget (this.transform);
#if UNITY_EDITOR || UNITY_STANDALONE
				CUIControlManager.Instance.RegisterControl (false, this.ShowChat, this.ShowEmotion);
#elif UNITY_ANDROID
				CUIControlManager.Instance.RegisterControl (true, this.ShowChat, this.ShowEmotion);	
#endif
			}
		}

		public override float GetDistanceToTarget ()
		{
			base.GetDistanceToTarget ();
			if (m_TargetInteract == null)
				return 0.2f;
			return this.GetSize () + m_TargetInteract.GetSize();
		}

		public override void SetMovePosition(Vector3 value) {
			base.SetMovePosition (value);
			if (m_MovableComponent == null)
				return;
			value.y = 0f;
			m_MovableComponent.targetPosition = value;
		}

		#endregion
	
	}
}
