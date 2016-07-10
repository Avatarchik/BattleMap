namespace Player
{
	using UnityEngine;
	using UnityEngine.Networking;

	[RequireComponent(typeof(PlayerController))]
	public class ServerControls : NetworkBehaviour
	{
		[SerializeField]
		private GameObject player;

		void Update()
		{
			if (!isLocalPlayer) { return; }
			if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftShift))
			{
				RaycastHit hit;
				if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
				{
					CmdSpawnPlayer(hit.point.x, hit.point.y, hit.point.z);
				}
			}
		}

		[Command]
		private void CmdSpawnPlayer(float x, float y, float z)
		{
			GameObject spawned = (GameObject)Instantiate(player,
				new Vector3(x, y, z), Quaternion.Euler(Vector3.zero));
			NetworkServer.SpawnWithClientAuthority(spawned, connectionToClient);
		}
	}
}
