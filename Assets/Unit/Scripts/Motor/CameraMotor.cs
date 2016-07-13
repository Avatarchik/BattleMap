namespace Unit
{
	using System;
	using Player;
	using UnityEngine;
	using UnityEngine.Networking;

	public class CameraMotor : NetworkBehaviour, IMotor
	{
		private Vector3 velocity = Vector3.zero;
		private Vector3 rotation = Vector3.zero;
		private float curCRotationX = 0f;

		[SerializeField]
		private float cRotationLimit = 85f;

		private bool inControl = false;

		public Vector3 Movement { set { velocity = value; } }
		public Vector3 Rotation { set { rotation = value; } }
		float IMotor.Zoom { set { } }
		bool IMotor.InControl { get { return inControl; } }
		public float DistanceTraveled { get { return 0; } }

		void FixedUpdate()
		{
			if (!isLocalPlayer) { return; }
			if (!inControl) { return; }
			PerformMovement();
			PerformRotation();
		}

		private void PerformMovement()
		{
			if (velocity != Vector3.zero)
			{
				transform.position = transform.position + velocity * Time.fixedDeltaTime;
			}
		}

		private void PerformRotation()
		{
			transform.rotation = transform.rotation * 
				Quaternion.Euler(new Vector3(0f, rotation.y, 0f));
			curCRotationX -= rotation.x;
			curCRotationX = Mathf.Clamp(curCRotationX, -cRotationLimit, cRotationLimit);

			transform.eulerAngles = new Vector3(curCRotationX, transform.eulerAngles.y, 0f);
		}

		public void TakeControl(Transform Caller) { inControl = true; }

		public void LooseControl() { inControl = false; }

		public void ResetTurn() { }

		public void NewTurn() { }
	}

}
