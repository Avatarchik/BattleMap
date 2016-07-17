namespace Player
{
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.Networking;
	using UnityEngine.Events;
	using Unit;
	using System.Collections.Generic;
	using uCPf;
	using Utilities;
	using Prop;

	public class UserInterface : NetworkBehaviour
	{
		private PlayerController controller;
		private int motorCount;
		private UnitManager controlling;

		[SerializeField]
		private GameObject button;
		private GameObject nameInput;
		private GameObject colourPicker;
		private InputField namerField;

		private GameObject spawning;

		[SerializeField]
		private List<GameObject> SpawnableUnits;

		[SerializeField]
		private List<GameObject> SpawnableProps;

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
			colourPicker = picker.gameObject;
			picker.OnChange.AddListener(SetColour);
			
			namerField = canvas.GetComponentInChildren<InputField>();
			nameInput = namerField.gameObject;
			namerField.text = "Free Roam";
			namerField.onValueChanged.AddListener(SetName);
			namerField.onEndEdit.AddListener(SetName);

			CreateDropDown(new Vector3(60, Screen.height - 35), 
				new Vector2(100, 25), SpawnableUnits);
			CreateDropDown(new Vector3(Screen.width - 60, Screen.height - 35), 
				new Vector2(100, 25), SpawnableProps);
		}

		void Update()
		{
			if (!isLocalPlayer) { return; }
			if (controlling == null) { TakeMotor(0); }
			colourPicker.transform.position = new Vector3(30, 90);
			nameInput.transform.position = new Vector3(Screen.width / 2 + 50, 40);

			if (Input.GetKeyDown(KeyCode.C))
			{
				SpawnRangeChecker();
			}

			if (spawning != null)
			{
				RaycastHit hit;
				if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), 
					out hit, 
					maxDistance: float.MaxValue, 
					layerMask: 1 << LayerMask.NameToLayer("Floor")))
				{
					spawning.transform.position = hit.point.Round(1f);
				}
				if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftAlt))
				{
					CmdSpawnObject(spawning.name, 
						spawning.transform.position.x, 
						spawning.transform.position.y, 
						spawning.transform.position.z);
				}
				if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
				{
					Destroy(spawning);
					spawning = null;
				}
				return;
			}

			if (Input.GetKey(KeyCode.LeftAlt))
			{
				if (Input.GetKeyDown(KeyCode.Delete))
				{
					UnitManager temp = controlling;
					TakeMotor(0);
					RemoveUnit(temp);
					temp.Prop.DeleteSelf();
				}
				if (Input.GetMouseButtonDown(0))
				{
					RaycastHit hit;
					if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
					{
						Prop prop = hit.transform.GetComponent<Prop>();
						if (prop != null)
						{
							prop.DeleteSelf();
						}
					}
				}
			}
		}

		private void SetName(string Name)
		{
			controlling.Unit.Name = Name;
		}

		public void SpawnRangeChecker()
		{
			CmdSpawnObject("RangeChecker", 0, 0, 0);
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

		private void CreateDropDown(Vector3 startPoint, Vector2 buttonSize, List<GameObject> objects)
		{
			float pos = startPoint.y;
			int i = 0;

			foreach (var item in objects)
			{
				var temp = item;
				dropDownButtons.Add(CreateButton(button, new Vector3(startPoint.x, pos), buttonSize,
					() => 
					{
						if (spawning != null)
						{
							Destroy(spawning);
						}
						SpawnPrefab(temp);
					}
					, item.name));
				pos -= buttonSize.y;
				i++;
			}
		}

		private void SpawnPrefab(GameObject Prefab)
		{
			spawning = (GameObject)Instantiate(Prefab, Vector3.zero, Quaternion.Euler(Vector3.zero));
		}

		[Command]
		private void CmdSpawnObject(string toSpawn, float x, float y, float z)
		{
			string final = toSpawn.Replace("(Clone)", string.Empty);
			GameObject instance = (GameObject)Instantiate(Resources.Load<GameObject>(final), 
				new Vector3(x, y, z), Quaternion.Euler(Vector3.zero));
			NetworkServer.SpawnWithClientAuthority(instance, connectionToClient);
		}
	}
}
