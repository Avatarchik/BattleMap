namespace Unit
{
	using UnityEngine;

	public interface IUnit
	{
		void SetName(string Name);

		string GetName();

		void SetColour(Color Colour);

		Color GetColour();

		void DeleteSelf();

		float GetRadius();
	}
}