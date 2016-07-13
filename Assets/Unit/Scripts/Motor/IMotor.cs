namespace Unit
{
	using UnityEngine;

	public interface IMotor
	{
		Vector3 Movement { set; }

		Vector3 Rotation { set; }

		float Zoom { set; }

		void TakeControl(Transform Caller);

		void LooseControl();

		bool InControl { get; }

		float GetDistanceTraveled { get; }
	}

}
