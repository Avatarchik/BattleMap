namespace Unit
{
	using Player;
	using UnityEngine;
	using UnityEngine.Networking;

	[RequireComponent(typeof(IUnit))]
	[RequireComponent(typeof(UserInterface))]
	public class CameraMotor : NetworkBehaviour, IMotor
	{
		private Vector3 velocity = Vector3.zero;
		private Vector3 rotation = Vector3.zero;
		private float curCRotationX = 0f;

		[SerializeField]
		private float cRotationLimit = 85f;

		private bool inControl = false;

		public void Move(Vector3 Velocity)
		{
			velocity = Velocity;
		}

		public void Rotate(Vector3 Rotation)
		{
			rotation = Rotation;
		}

		void Start()
		{
			if (!isLocalPlayer) { return; }
			GetComponent<UserInterface>().AddMotor(this);
		}

		void FixedUpdate()
		{
			if (!isLocalPlayer) { return; }
			if (!inControl) { return; }
			PerformMovement();
			PerformRotation();
		}

		void OnDisable()
		{
			if (!isLocalPlayer) { return; }
			GetComponent<UserInterface>().RemoveMotor(this);
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

		public void TakeControl(Transform Caller)
		{
			inControl = true;
		}

		public void LooseControl()
		{
			inControl = false;
		}

		public IUnit GetUnit()
		{
			return GetComponent<IUnit>();
		}
	}

}
