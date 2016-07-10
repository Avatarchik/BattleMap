namespace Unit
{
	using UnityEngine;
	using UnityEngine.Networking;
	using Player;
	using System;

	public class UnitManager : NetworkBehaviour
	{
		public IUnit Unit
		{
			get
			{
				if (unit != null) { return unit; }
				else
				{
					unit = GetComponentInChildren<IUnit>();
					if (unit != null) { return unit; }
					else
					{
						throw new Exception("Cannot find a unit.");
					}
				}
			}
		}
		public IMotor Motor
		{
			get
			{
				if (motor != null) { return motor; }
				else
				{
					motor = GetComponentInChildren<IMotor>();
					if (motor != null) { return motor; }
					else
					{
						throw new Exception("Cannot find a motor.");
					}
				}
			}
		}
		public UnitNamePlate NamePlate
		{
			get
			{
				if (namePlate != null) { return namePlate; }
				else
				{
					namePlate = GetComponentInChildren<UnitNamePlate>();
					if (namePlate != null) { return namePlate; }
					else
					{
						throw new Exception("Cannot find a name plate.");
					}
				}
			}
		}

		private IUnit unit;
		private IMotor motor;
		private UnitNamePlate namePlate;

		public override void OnStartAuthority()
		{
			var userInterface = FindObjectOfType<UserInterface>();
			if (userInterface != null) { userInterface.AddUnit(this); }
		}

		public override void OnStopAuthority()
		{
			var userInterface = FindObjectOfType<UserInterface>();
			if (userInterface != null) { userInterface.RemoveUnit(this); }
		}
	}
}
