namespace Unit
{
	using Player;
	using UnityEngine;
	using UnityEngine.Networking;
	using System.Collections.Generic;
	using Utilities;

	public class Unit : NetworkBehaviour, IUnit
	{
		[SyncVar(hook = "OnNameChanged")]
		private string identifier = "Untitled";
		private INamePlate namePlate;

		[SyncVar(hook = "OnColourChanged")]
		private Color colour = Color.cyan;

		public string Name
		{
			get { return identifier; }
			set { CmdSetName(value); }
		}

		public Color Colour
		{
			get { return colour; }
			set { CmdSetColour(value); }
		}

		public float Circumference { get { return transform.localScale.XZAverage(); } }
		
		void Start()
		{
			namePlate = GetComponentInChildren<INamePlate>();
			OnNameChanged(identifier);
			OnColourChanged(colour);
		}

		public override void OnStartAuthority()
		{
			colour = UserInterface.UnitColour;
		}

		private void OnNameChanged(string NewName)
		{
			FindObjectOfType<UserInterface>().ArrangeButtons();
			namePlate.Name = NewName;
		}

		[Command]
		private void CmdSetColour(Color newColor)
		{
			colour = newColor;
		}

		[Command]
		private void CmdSetName(string newName)
		{
			identifier = newName;
		}

		private void OnColourChanged(Color newColor)
		{
			var colourables = GetColourableChildren(transform);
			namePlate.Colour = newColor;
			foreach (var colourable in colourables)
			{
				Renderer renderer = colourable.GetComponent<Renderer>();
				Material mat = renderer.material;

				float emission = Mathf.PingPong(Time.time, 1.0f);
				Color baseColor = newColor;

				Color finalColor = baseColor * Mathf.LinearToGammaSpace(emission);

				mat.color = newColor;
				mat.SetColor("_EmissionColor", finalColor);
			}
		}

		private List<Transform> GetColourableChildren(Transform parent)
		{
			var output = new List<Transform>();
			foreach (Transform child in parent)
			{
				if (child.childCount > 0)
				{
					output.AddRange(GetColourableChildren(child));
				}

				if (child.CompareTag("Colourable"))
				{
					output.Add(child);
				}
			}

			return output;
		}
	}
}
