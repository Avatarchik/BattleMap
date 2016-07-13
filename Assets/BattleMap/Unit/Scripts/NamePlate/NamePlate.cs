﻿namespace Unit
{
	using UnityEngine;

	[RequireComponent(typeof(TextMesh))]
	[RequireComponent(typeof(UnitManager))]
	public class NamePlate : MonoBehaviour, INamePlate
	{
		public static UnitManager Target;
		private UnitManager manager;
		private TextMesh text;
		private string identifier;

		public string Name { set { identifier = value; } }

		public Color Colour { set { if (text != null) { text.color = value; } } }

		void Start()
		{
			text = GetComponent<TextMesh>();
			manager = GetComponentInParent<UnitManager>();
		}

		private float UnitsToFeet(float Input)
		{
			return Input * 5f;
		}

		void Update()
		{
			transform.rotation = Camera.main.transform.rotation;
			float distance = Vector3.Distance(transform.position, Camera.main.transform.position);
			distance /= 15f;
			transform.localScale = new Vector3(
				distance * (1f / transform.parent.localScale.x),
				distance * (1f / transform.parent.localScale.y),
				distance * (1f / transform.parent.localScale.z));

			if (Target == null || manager.Motor.InControl)
			{
				text.text = "Name: " + identifier + "\n Traveled: " +
					Mathf.RoundToInt(UnitsToFeet(manager.Motor.DistanceTraveled)) + "ft.";
				return;
			}
			Transform tTransform = Target.transform;
			distance = Vector3.Distance(new Vector3(transform.position.x, 0.0f, transform.position.z),
				new Vector3(tTransform.position.x, 0.0f, tTransform.position.z));

			if (distance > 24)
			{
				foreach (Renderer r in GetComponentsInChildren<Renderer>())
					r.enabled = false;
			}
			else
			{
				foreach (Renderer r in GetComponentsInChildren<Renderer>())
					r.enabled = true;
			}

			// distance in feet
			distance = UnitsToFeet(distance);
			distance -= UnitsToFeet(Target.Unit.Circumference / 2);
			distance -= UnitsToFeet(manager.Unit.Circumference / 2);
			distance += 5.0f;

			text.text = "Name: " + identifier +  "\n Distance: " + 
				Mathf.RoundToInt(distance) + "ft.\nTraveled: " + 
				Mathf.RoundToInt(UnitsToFeet(manager.Motor.DistanceTraveled)) + "ft.";
		}
	}
}
