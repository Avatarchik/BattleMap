namespace Unit
{
	using UnityEngine;

	[RequireComponent(typeof(TextMesh))]
	public class UnitNamePlate : MonoBehaviour
	{
		private TextMesh text;
		private string identifier;

		void Start()
		{
			text = GetComponent<TextMesh>();
		}

		void Update()
		{
			transform.LookAt(Camera.main.transform);
			float distance = Vector3.Distance(transform.position, Camera.main.transform.position);
			text.characterSize = distance / 2000;
			text.text = identifier + " " + distance;
		}

		public void SetName(string Name)
		{
			identifier = Name;
		}
	}
}
