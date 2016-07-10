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

		private List<GameObject> buttons = new List<GameObject>();

		private Canvas canvas;

		private List<UnitManager> ownedUnits = new List<UnitManager>();

		[HideInInspector]
		public static Color UnitColour = Color.cyan;

		void Start()
		{
			if (!isLocalPlayer) { return; }
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
			if (Input.GetKeyDown(KeyCode.Delete) && 
				Input.GetKey(KeyCode.LeftShift) && 
				Input.GetKey(KeyCode.LeftAlt))
			{
				UnitManager temp = controlling;
				TakeMotor(0);
				RemoveUnit(temp);
				temp.Unit.DeleteSelf();
			}
		}

		private void SetName(string Name)
		{
			controlling.Unit.SetName(Name);
		}

		public void SetColour(Color Colour)
		{
			if (!isLocalPlayer) { return; }
			UnitColour = Colour;
			foreach (var unit in ownedUnits)
			{
				unit.Unit.SetColour(Colour);
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
			foreach (GameObject button in buttons)
			{
				Destroy(button);
			}
			buttons.Clear();
			int pos = 25;
			int i = 0;
			foreach (var unit in ownedUnits)
			{
				int temp = i;
				CreateButton(button, new Vector3(Screen.width - 110, pos),
					new Vector2(200, 30),
					delegate { TakeMotor(temp); },
					unit.Unit.GetName());
				pos += 30;
				i++;
			}
		}

		private void TakeMotor(int Index)
		{
			if (ownedUnits.Count <= Index) { return; }
			controller.TakeMotor(ownedUnits[Index]);
			controlling = ownedUnits[Index];
			namerField.text = controlling.Unit.GetName();
			UnitNamePlate.Target = controlling;
		}

		private void CreateButton(GameObject prefab, Vector3 position, Vector2 size, UnityAction method, string Name)
		{
			if (canvas == null)
			{
				canvas = FindObjectOfType<Canvas>();
			}
			GameObject button = Instantiate(prefab);
			button.transform.SetParent(canvas.transform, false);
			button.transform.position = position;
			button.GetComponent<RectTransform>().sizeDelta = size;
			button.GetComponent<Button>().onClick.AddListener(method);
			button.GetComponentInChildren<Text>().text = Name;
			buttons.Add(button);
		}
	}
}
