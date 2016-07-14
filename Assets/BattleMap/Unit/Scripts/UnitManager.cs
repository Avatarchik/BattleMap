namespace Unit
{
	using UnityEngine.Networking;
	using Player;
	using System;
	using Prop;

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
					throw new Exception("Unit not found.");
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
					throw new Exception("Motor not found.");
				}
			}
		}
		public INamePlate NamePlate
		{
			get
			{
				if (namePlate != null) { return namePlate; }
				else
				{
					namePlate = GetComponentInChildren<INamePlate>();
					if (namePlate != null) { return namePlate; }
					throw new Exception("Name plate not found.");
				}
			}
		}

		public Prop Prop
		{
			get
			{
				if (prop != null) { return prop; }
				else
				{
					prop = GetComponentInChildren<Prop>();
					if (prop != null) { return prop; }
					throw new Exception("Prop not found.");
				}
			}
		}

		private IUnit unit;
		private IMotor motor;
		private INamePlate namePlate;
		private Prop prop;

		public override void OnStartAuthority()
		{
			var userInterface = FindObjectOfType<UserInterface>();
			Motor.NewTurn();
			if (userInterface != null) { userInterface.AddUnit(this); }
		}

		public override void OnStopAuthority()
		{
			var userInterface = FindObjectOfType<UserInterface>();
			if (userInterface != null) { userInterface.RemoveUnit(this); }
		}
	}
}
