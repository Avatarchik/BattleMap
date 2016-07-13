namespace Player
{
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.Networking;
	using UnityEngine.Events;
	using Unit;
	using System.Collections.Generic;
	using uCPf;

	public class UserInterface : NetworkBehaviour
	{
		private PlayerController controller;
		private int motorCount;
		private UnitManager controlling;

		[SerializeField]
		private GameObject button;
		private GameObject nameInput;
		private InputField namerField;

		[SerializeField]
		private List<GameObject> SpawnableUnits;

		private List<GameObject> buttons = new List<GameObject>();

		private List<GameObject> dropDownButtons = new List<GameObject>();

		private Canvas canvas;

		private List<UnitManager> ownedUnits = new List<UnitManager>();

		[HideInInspector]
		public static Color UnitColour = Color.cyan;

		void Start()
		{
			if (!isLocalPlayer)
			{
				gameObject.SetActive(false);
				return;
			}
			controller = GetComponent<PlayerController>();
			canvas = FindObjectOfType<Canvas>();

			ColorPicker picker = canvas.GetComponentInChildren<ColorPicker>();
			picker.OnChange.AddListener(SetColour);
			
			namerField = canvas.GetComponentInChildren<InputField>();
			namerField.text = "Free Roam";
			namerField.onValueChanged.AddListener(SetName);
			namerField.onEndEdit.AddListener(SetName);
		}

		void Update()
		{
			if (!isLocalPlayer) { return; }
			if (controlling == null) { TakeMotor(0); }
			if (Input.GetKey(KeyCode.LeftShift) &&
				Input.GetKey(KeyCode.LeftAlt))
			{
				if (Input.GetKeyDown(KeyCode.Delete))
				{
					UnitManager temp = controlling;
					TakeMotor(0);
					RemoveUnit(temp);
					temp.Unit.DeleteSelf();
				}
				if (Input.GetMouseButtonDown(0))
				{
					CreateDropDown(Input.mousePosition, new Vector2(100, 25));
				}
			}
		}

		private void SetName(string Name)
		{
			controlling.Unit.Name = Name;
		}

		public void SetColour(Color Colour)
		{
			if (!isLocalPlayer) { return; }
			UnitColour = Colour;
			foreach (var unit in ownedUnits)
			{
				unit.Unit.Colour = Colour;
			}
		}

		public void AddUnit(UnitManager Unit)
		{
			if (!hasAuthority) { return; }
			ownedUnits.Add(Unit);
			ArrangeButtons();
		}

		public void RemoveUnit(UnitManager Unit)
		{
			if (!hasAuthority) { return; }
			ownedUnits.Remove(Unit);
			ArrangeButtons();
		}

		public void ArrangeButtons()
		{
			foreach (GameObject b in buttons)
			{
				Destroy(b);
			}
			buttons.Clear();
			int pos = 25;
			int i = 0;
			foreach (var unit in ownedUnits)
			{
				int temp = i;
				buttons.Add(CreateButton(button, new Vector3(Screen.width - 110, pos),
					new Vector2(200, 30),
					delegate { TakeMotor(temp); },
					unit.Unit.Name));
				pos += 30;
				i++;
			}
		}

		private void TakeMotor(int Index)
		{
			if (ownedUnits.Count <= Index) { return; }
			controller.TakeMotor(ownedUnits[Index]);
			controlling = ownedUnits[Index];
			namerField.text = controlling.Unit.Name;
			NamePlate.Target = controlling;
		}

		private GameObject CreateButton(GameObject prefab, Vector3 position, Vector2 size, UnityAction method, string Name)
		{
			if (canvas == null)
			{
				canvas = FindObjectOfType<Canvas>();
			}
			GameObject b = Instantiate(prefab);
			b.transform.SetParent(canvas.transform, false);
			b.transform.position = position;
			b.GetComponent<RectTransform>().sizeDelta = size;
			b.GetComponent<Button>().onClick.AddListener(method);
			b.GetComponentInChildren<Text>().text = Name;
			return b;
		}

		private void CreateDropDown(Vector3 startPoint, Vector2 buttonSize)
		{
			float pos = startPoint.y;
			int i = 0;
			Vector3 worldPoint;
			RaycastHit hit;
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
			{
				worldPoint = hit.point;
			}
			else
			{
				return;
			}

			foreach (var item in SpawnableUnits)
			{
				int temp = i;
				dropDownButtons.Add(CreateButton(button, new Vector3(startPoint.x, pos), buttonSize,
					() => 
					{
						foreach (GameObject b in dropDownButtons)
						{
							Destroy(b);
						}
						dropDownButtons.Clear();
						CmdSpawnPlayer(worldPoint.x, 0.0f, worldPoint.z, temp);
					}
					, item.name));
				pos += buttonSize.y;
				i++;
			}
		}

		[Command]
		private void CmdSpawnPlayer(float x, float y, float z, int index)
		{
			GameObject spawned = (GameObject)Instantiate(SpawnableUnits[index],
				new Vector3(x, y, z), Quaternion.Euler(Vector3.zero));
			NetworkServer.SpawnWithClientAuthority(spawned, connectionToClient);
		}
	}
}
