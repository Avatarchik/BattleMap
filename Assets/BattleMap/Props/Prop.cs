namespace Prop
{
	using UnityEngine;
	using UnityEngine.Networking;

	public class Prop : NetworkBehaviour
	{
		public void DeleteSelf()
		{
			if (!hasAuthority) { return; }
			CmdDeleteSelf();
		}

		[Command]
		private void CmdDeleteSelf()
		{
			NetworkServer.Destroy(gameObject);
		}
	}
}
