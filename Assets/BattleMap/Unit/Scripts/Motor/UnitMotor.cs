namespace Unit
{
	using System;
	using UnityEngine;
	using UnityEngine.Networking;

	[RequireComponent(typeof(IUnit))]
	public class UnitMotor : NetworkBehaviour, IMotor
	{
		[SerializeField]
		private Transform cameraPivot;

		private Vector3 velocity = Vector3.zero;
		private Vector3 rotation = Vector3.zero;
		private float curCRotationX = 0f;
		[SerializeField]
		private float zoom = 5f;

		private Transform cam;

		[SyncVar]
		private float distance = 0f;

		private Vector3 previousPosition;
		private Vector3 startTurnPosition;

		[SerializeField]
		private float speed = 4f;
		private const float cRotationLimit = 85f;

		private bool inControl;

		public Vector3 Movement { set { velocity = value; velocity.y = 0f; } }
		public Vector3 Rotation { set { rotation = value; } }
		float IMotor.Zoom { set { zoom += value; } }
		bool IMotor.InControl { get { return inControl; } }
		public float DistanceTraveled { get { return distance; } }

		void Start()
		{
			startTurnPosition = transform.position;
			previousPosition = transform.position;
		}

		void FixedUpdate()
		{
			if (transform.position != previousPosition)
			{
				distance += Vector3.Distance(previousPosition, transform.position);
				previousPosition = transform.position;
			}
			if (!inControl) { return; }
			PerformMovement();
			PerformRotation();
			cam.transform.localPosition = new Vector3(0, 0, -zoom);
		}

		void OnDisable()
		{
			if (cam != null) { cam.transform.parent = null; }
		}

		private void PerformMovement()
		{
			if (velocity == Vector3.zero) { return; }
			transform.position = transform.position + velocity.normalized * Time.fixedDeltaTime * speed;
		}

		private void PerformRotation()
		{
			if (rotation == Vector3.zero) { return; }
			transform.rotation = transform.rotation * 
				Quaternion.Euler(new Vector3(0f, rotation.y, 0f));
			if (cameraPivot != null)
			{
				curCRotationX -= rotation.x;
				curCRotationX = Mathf.Clamp(curCRotationX, -cRotationLimit, cRotationLimit);

				cameraPivot.localEulerAngles = new Vector3(curCRotationX, 0f, 0f);
			}
		}

		public void TakeControl(Transform Caller)
		{
			cam = Caller;
			if (cameraPivot != null)
			{
				inControl = true;
				cam.transform.parent = cameraPivot;
				cam.transform.localPosition = new Vector3(0, 0, -zoom);
				cam.transform.localRotation = Quaternion.Euler(0, 0, 0);
			}
		}

		public void LooseControl()
		{
			cam.transform.parent = null;
			inControl = false;
		}

		public void ResetTurn()
		{
			transform.position = startTurnPosition;
			previousPosition = startTurnPosition;
			distance = 0f;
		}

		public void NewTurn()
		{
			startTurnPosition = transform.position;
			previousPosition = transform.position;
			distance = 0f;
		}
	}

}