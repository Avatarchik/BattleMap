namespace Unit
{
	using UnityEngine;

	[RequireComponent(typeof(TextMesh))]
	[RequireComponent(typeof(UnitManager))]
	public class UnitNamePlate : MonoBehaviour
	{
		public static UnitManager Target;
		private UnitManager manager;
		private TextMesh text;
		private string identifier;

		void Start()
		{
			text = GetComponent<TextMesh>();
			manager = GetComponentInParent<UnitManager>();
		}

		void Update()
		{
			transform.rotation = Camera.main.transform.rotation;
			float distance = Vector3.Distance(transform.position, Camera.main.transform.position);
			text.characterSize = distance / 2000.0f;

			if (Target == null) { return; }
			Transform tTransform = Target.transform;
			distance = Vector3.Distance(new Vector3(transform.position.x, 0.0f, transform.position.z),
				new Vector3(tTransform.position.x, 0.0f, tTransform.position.z));

			// distance in feet
			distance *= 2.0f;
			distance -= Target.Unit.GetRadius();
			distance -= manager.Unit.GetRadius();
			distance += 5.0f;

			text.text = identifier + ": " + Mathf.Ceil(distance) + "ft.";
		}

		public void SetName(string Name)
		{
			identifier = Name;
		}
	}
}
