namespace Unit
{
	using Player;
	using UnityEngine;
	using UnityEngine.Networking;
	using System.Collections.Generic;

	public class Unit : NetworkBehaviour, IUnit
	{
		private string identifier = "Untitled";
		private UnitNamePlate namePlate;

		private Color colour;

		void Start()
		{
			namePlate = GetComponentInChildren<UnitNamePlate>();
		}

		public override void OnStartAuthority()
		{
			CmdColourChanged(UserInterface.UnitColour.r,
					UserInterface.UnitColour.g,
					UserInterface.UnitColour.b);
		}

		public Color GetColour()
		{
			return colour;
		}

		public string GetName()
		{
			return identifier;
		}

		public void SetColour(Color Colour)
		{
			CmdColourChanged(Colour.r, Colour.g, Colour.b);
		}

		public void SetName(string Name)
		{
			CmdNameChanged(Name);
		}

		[Command]
		private void CmdColourChanged(float r, float g, float b)
		{
			RpcColourChanged(r, g, b);
		}

		[Command]
		private void CmdNameChanged(string Name)
		{
			RpcNameChanged(Name);
		}

		[ClientRpc]
		private void RpcNameChanged(string Name)
		{
			identifier = Name;
			FindObjectOfType<UserInterface>().ArrangeButtons();
			namePlate.SetName(Name);
		}

		[ClientRpc]
		private void RpcColourChanged(float r, float g, float b)
		{
			colour = new Color(r, g, b);
			var colourables = GetColourableChildren(transform);
			foreach (var colourable in colourables)
			{
				Renderer renderer = colourable.GetComponent<Renderer>();
				Material mat = renderer.material;

				float emission = Mathf.PingPong(Time.time, 1.0f);
				Color baseColor = colour;

				Color finalColor = baseColor * Mathf.LinearToGammaSpace(emission);

				mat.color = colour;
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

		public void DeleteSelf()
		{
			CmdDeleteSelf();
		}

		[Command]
		private void CmdDeleteSelf()
		{
			RpcDeleteSelf();
		}

		private void RpcDeleteSelf()
		{
			Destroy(gameObject);
		}
	}
}
