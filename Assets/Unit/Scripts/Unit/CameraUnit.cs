namespace Unit
{
	using UnityEngine;

	public class CameraUnit : MonoBehaviour, IUnit
	{
		private string identifier = "Free Roam";

		public Color Colour { get { return Color.black; } set { } }

		public string Name { get { return identifier; } set { identifier = value; } }

		public float Radius { get { return 0.0f; } }

		public void DeleteSelf() { }
	}
}
