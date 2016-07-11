namespace Unit
{
	using UnityEngine;

	public interface IMotor
	{
		void Move(Vector3 Velocity);

		void Rotate(Vector3 Rotation);

		void TakeControl(Transform Caller);

		void LooseControl();

		bool InControl();
	}

}
