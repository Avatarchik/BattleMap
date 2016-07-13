namespace Player
{
	using UnityEngine;
	using UnityEngine.Networking;
	using Unit;
	using System;

	public class PlayerController : NetworkBehaviour
	{
		[SerializeField]
		private float speed = 5f;
		[SerializeField]
		private float lookSensitivity = 3f;

		[HideInInspector]
		public UnitManager Controlling;

		void Start()
		{
			if (!isLocalPlayer) { return; }

			UnitManager cam = GetComponent<UnitManager>();
			if (cam != null)
			{
				Controlling = cam;
				Controlling.Motor.TakeControl(transform);
			}
		}

		void Update()
		{
			if (!isLocalPlayer) { return; }
			GetInput();
			if (Input.GetKeyDown(KeyCode.R))
			{
				Controlling.Motor.ResetTurn();
			}
			else if (Input.GetKeyDown(KeyCode.N))
			{
				Controlling.Motor.NewTurn();
			}
		}

		public void TakeMotor(UnitManager Unit)
		{
			if (Controlling == null) return;
			Controlling.Motor.LooseControl();
			Controlling = Unit;
			Controlling.Motor.TakeControl(transform);

		}

		private void GetInput()
		{
			// calculate movement velocity
			float xMov = Input.GetAxisRaw("Horizontal");
			float zMov = Input.GetAxisRaw("Vertical");
			float upMov = Convert.ToSingle(Input.GetKey(KeyCode.Space));

			// check camera zoom
			float zoom = Input.GetAxisRaw("Mouse ScrollWheel");
			Controlling.Motor.Zoom = -zoom;

			Vector3 movHorizontal = transform.right * xMov;
			Vector3 movVertical = transform.forward * zMov;
			Vector3 movUp = new Vector3(0, upMov, 0);

			// final movememnt vector
			Vector3 velocity = (movHorizontal + movVertical + movUp).normalized * speed;

			// apply movement
			Controlling.Motor.Movement = velocity;

			if (Input.GetMouseButton(1))
			{
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;

				// calculate rotation as a 3d vector for camera
				float xRot = Input.GetAxisRaw("Mouse Y");

				// calculate rotation as a 3d vector for turning only
				float yRot = Input.GetAxisRaw("Mouse X");

				Vector3 rotation = new Vector3(xRot, yRot, 0f) * lookSensitivity;

				// apply rotation
				Controlling.Motor.Rotation = rotation;
			}
			else
			{
				Controlling.Motor.Rotation = new Vector3(0, 0, 0);
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
		}
	}

}
