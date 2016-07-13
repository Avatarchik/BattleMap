namespace Unit
{
	using Player;
	using UnityEngine;
	using UnityEngine.Networking;
	using System.Collections.Generic;

	public class Unit : NetworkBehaviour, IUnit
	{
		private string identifier = "Untitled";
		private INamePlate namePlate;

		private Color colour;

		[SerializeField]
		private float radius;

		public string Name
		{
			get { return identifier; }
			set { CmdNameChanged(value); }
		}

		public Color Colour
		{
			get { return colour; }
			set { CmdColourChanged(value.r, value.g, value.b);}
		}

		public float Radius { get { return radius; } }

		void Start()
		{
			namePlate = GetComponentInChildren<INamePlate>();
			namePlate.Name = identifier;
		}

		public override void OnStartAuthority()
		{
			CmdColourChanged(UserInterface.UnitColour.r,
					UserInterface.UnitColour.g,
					UserInterface.UnitColour.b);
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
			namePlate.Name = Name;
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

		[ClientRpc]
		private void RpcDeleteSelf()
		{
			Destroy(gameObject);
		}
	}
}
