namespace RangeChecker
{
	using UnityEngine;
	using UnityEngine.Networking;

	public class RangeChecker : NetworkBehaviour
	{
		TextMesh text;

		[SyncVar]
		float radius = 1;

		void Start()
		{
			text = GetComponentInChildren<TextMesh>();
		}

		private float UnitsToFeet(float Input)
		{
			return Input * 5f;
		}

		void Update()
		{
			transform.localScale = new Vector3(radius * 2, 0.1f, radius * 2);
			text.text = Mathf.RoundToInt(UnitsToFeet(radius)) + "ft.";
			if (!hasAuthority) { return; }
			radius += Input.GetAxisRaw("Mouse ScrollWheel") * 2;

			RaycastHit hit;
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),
					out hit,
					maxDistance: float.MaxValue,
					layerMask: 1 << LayerMask.NameToLayer("Floor")))
			{
				transform.position = hit.point;
			}

			if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
			{
				CmdDeleteSelf();
			}
		}

		[Command]
		void CmdDeleteSelf()
		{
			NetworkServer.Destroy(gameObject);
		}
	}

}