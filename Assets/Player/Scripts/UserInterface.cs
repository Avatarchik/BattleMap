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
		private IMotor controlling;

		[SerializeField]
		private GameObject button;
		private GameObject nameInput;
		private InputField namerField;

		private List<GameObject> buttons = new List<GameObject>();

		private Canvas canvas;

		private List<IMotor> ownedMotors = new List<IMotor>();

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
			if (Input.GetKeyDown(KeyCode.Delete) && 
				Input.GetKey(KeyCode.LeftShift) && 
				Input.GetKey(KeyCode.LeftAlt))
			{
				IMotor temp = controlling;
				TakeMotor(0);
				RemoveMotor(temp);
				temp.GetUnit().DeleteSelf();
			}
		}

		private void SetName(string Name)
		{
			controlling.GetUnit().SetName(Name);
		}

		public void SetColour(Color Colour)
		{
			if (!isLocalPlayer) { return; }
			UnitColour = Colour;
			foreach (var motor in ownedMotors)
			{
				motor.GetUnit().SetColour(Colour);
			}
		}

		public void AddMotor(IMotor Motor)
		{
			if (!isLocalPlayer) { return; }
			ownedMotors.Add(Motor);
			ArrangeButtons();
		}

		public void RemoveMotor(IMotor Motor)
		{
			if (!isLocalPlayer) { return; }
			ownedMotors.Remove(Motor);
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
			foreach (IMotor motor in ownedMotors)
			{
				int temp = i;
				CreateButton(button, new Vector3(Screen.width - 110, pos),
					new Vector2(200, 30),
					delegate { TakeMotor(temp); },
					motor.GetUnit().GetName());
				pos += 30;
				i++;
			}
		}

		private void TakeMotor(int Index)
		{
			controller.TakeMotor(ownedMotors[Index]);
			controlling = ownedMotors[Index];
			namerField.text = controlling.GetUnit().GetName();
		}

		private void CreateButton(GameObject prefab, Vector3 position, Vector2 size, UnityAction method, string Name)
		{
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
