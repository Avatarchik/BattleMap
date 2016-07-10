namespace Unit
{
	using System;
	using UnityEngine;

	public class CameraUnit : MonoBehaviour, IUnit
	{
		private string identifier = "Free Roam";

		public void DeleteSelf()
		{
		}

		public Color GetColour()
		{
			return Color.black;
		}

		public string GetName()
		{
			return identifier;
		}

		public float GetRadius()
		{
			return 0.0f;
		}

		public void SetColour(Color Colour)
		{
		}

		public void SetName(string Name)
		{
			identifier = Name;
		}
	}
}
