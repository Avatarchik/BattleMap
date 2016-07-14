namespace Unit
{
	using UnityEngine;

	public interface IUnit
	{
		string Name { get; set; }

		Color Colour { get; set; }

		float Circumference { get; }
	}
}