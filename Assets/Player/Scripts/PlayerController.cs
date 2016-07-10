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
		public IMotor Motor;

		void Start()
		{
			if (!isLocalPlayer)
			{
				GetComponent<Camera>().enabled = false;
				GetComponent<AudioListener>().enabled = false;
				GetComponent<FlareLayer>().enabled = false;
				GetComponent<GUILayer>().enabled = false;
				return;
			}

			IMotor cam = GetComponent<IMotor>();
			if (cam != null)
			{
				Motor = cam;
				Motor.TakeControl(transform);
			}
		}

		void Update()
		{
			if (!isLocalPlayer) { return; }
			GetInput();
		}

		public void TakeMotor(IMotor Motor)
		{
			if (Motor == null) return;
			this.Motor.LooseControl();
			this.Motor = Motor;
			this.Motor.TakeControl(transform);

		}

		private void GetInput()
		{
			// calculate movement velocity
			float xMov = Input.GetAxisRaw("Horizontal");
			float zMov = Input.GetAxisRaw("Vertical");
			float upMov = Convert.ToSingle(Input.GetKey(KeyCode.Space));

			Vector3 movHorizontal = transform.right * xMov;
			Vector3 movVertical = transform.forward * zMov;
			Vector3 movUp = new Vector3(0, upMov, 0);

			// final movememnt vector
			Vector3 velocity = (movHorizontal + movVertical + movUp).normalized * speed;

			// apply movement
			Motor.Move(velocity);

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
				Motor.Rotate(rotation);
			}
			else
			{
				Motor.Rotate(new Vector3(0, 0, 0));
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
		}
	}

}
