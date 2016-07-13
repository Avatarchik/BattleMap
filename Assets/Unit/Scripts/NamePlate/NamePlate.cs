namespace Unit
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

		void Start()
		{
			text = GetComponent<TextMesh>();
			manager = GetComponentInParent<UnitManager>();
		}

		private float UnitsToFeet(float Input)
		{
			return Mathf.Ceil(Input * 2f);
		}

		void Update()
		{
			transform.rotation = Camera.main.transform.rotation;
			float distance = Vector3.Distance(transform.position, Camera.main.transform.position);
			text.characterSize = distance / 2000.0f;

			if (Target == null || manager.Motor.InControl)
			{
				text.text = identifier + "\n" +
					UnitsToFeet(manager.Motor.DistanceTraveled) + "ft. traveled this turn.";
				return;
			}
			Transform tTransform = Target.transform;
			distance = Vector3.Distance(new Vector3(transform.position.x, 0.0f, transform.position.z),
				new Vector3(tTransform.position.x, 0.0f, tTransform.position.z));

			// distance in feet
			distance = UnitsToFeet(distance);
			distance -= Target.Unit.Radius;
			distance -= manager.Unit.Radius;
			distance += 5.0f;

			text.text = identifier +  "\n" + 
				Mathf.Ceil(distance) + "ft. away\n" + 
				UnitsToFeet(manager.Motor.DistanceTraveled) + "ft. traveled this turn.";
		}
	}
}
