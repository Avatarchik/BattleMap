namespace Unit
{
	using UnityEngine;
	using UnityEngine.Networking;
	using Player;
	using System;

	[RequireComponent(typeof(IUnit))]
	public class UnitMotor : NetworkBehaviour, IMotor
	{
		[SerializeField]
		private Transform cameraPivot;

		private Vector3 velocity = Vector3.zero;
		private Vector3 rotation = Vector3.zero;
		private float curCRotationX = 0f;

		private Transform cam;

		[SerializeField]
		private float cRotationLimit = 85f;

		private bool inControl;

		[SyncVar]
		private string identifier = "";

		public void Move(Vector3 Velocity)
		{
			velocity = Velocity;
			velocity.y = 0f;
		}

		public void Rotate(Vector3 Rotation)
		{
			rotation = Rotation;
		}

		void FixedUpdate()
		{
			if (!inControl) { return; }
			PerformMovement();
			PerformRotation();
		}

		void OnDisable()
		{
			if (cam != null) { cam.transform.parent = null; }
		}

		private void PerformMovement()
		{
			if (velocity == Vector3.zero) { return; }
			transform.position = transform.position + velocity * Time.fixedDeltaTime;
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
				cam.transform.localPosition = new Vector3(0, 0, -5);
				cam.transform.localRotation = Quaternion.Euler(0, 0, 0);
			}
		}

		public void LooseControl()
		{
			cam.transform.parent = null;
			inControl = false;
		}

		public string GetName()
		{
			return identifier;
		}

		public void SetName(string Name)
		{
			identifier = Name;
		}

		public IUnit GetUnit()
		{
			return GetComponent<IUnit>();
		}

		public Transform GetTransform()
		{
			return transform;
		}

		public bool InControl()
		{
			return inControl;
		}
	}

}