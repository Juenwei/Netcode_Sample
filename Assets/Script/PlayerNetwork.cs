using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
	[SerializeField] private MeshRenderer meshRenderer;
	[SerializeField] private Transform spawnObjectPrefab;
	private Transform spawnObjectTransfrom;
	public struct MyCustomData : INetworkSerializable
	{
		public int _int;
		public bool _bool;
		public FixedString128Bytes _message;

		public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
		{
			serializer.SerializeValue(ref _int);
			serializer.SerializeValue(ref _bool);
			serializer.SerializeValue(ref _message);
		}
	}

	private NetworkVariable<MyCustomData> randomNumber = new NetworkVariable<MyCustomData>(new MyCustomData
	{
		_int = 1,
		_bool = false,
	}, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

	public override void OnNetworkSpawn()
	{
		randomNumber.OnValueChanged += (MyCustomData previousValue, MyCustomData newValue) => 
		{
			Debug.Log(OwnerClientId + ", Number switch to " + newValue._int + " from " + previousValue._int + newValue._message);
		};
	}

	private void Update()
	{

		if (!IsOwner) return;
		var moveDir = new Vector3(0, 0, 0);

		if(Input.GetKeyDown(KeyCode.T))
		{
			//randomNumber.Value = new MyCustomData
			//{
			//	_int = Random.Range(1, 100),
			//	_bool = true,
			//	_message = " Hello Unity, here is Netcode"
			//};
			//TestServerRpc(new ServerRpcParams());
			GameObject spawnGameObjec = Instantiate(spawnObjectPrefab).gameObject;
			gameObject.GetComponent<NetworkObject>().Spawn(true);

		}

		if (Input.GetKeyDown(KeyCode.R))
		{
			//TestClientRpc(new ClientRpcParams()
			//{
			//	Send = new ClientRpcSendParams()
			//	{
			//		TargetClientIds = new List<ulong> { 1 }
			//	}
			//});
			if(spawnObjectTransfrom != null)
				Destroy(spawnObjectTransfrom.gameObject);
		}

		//if (Input.GetKey(KeyCode.A)) moveDir.x -= 1;
		//if (Input.GetKey(KeyCode.D)) moveDir.x += 1;
		//if (Input.GetKey(KeyCode.W)) moveDir.z += 1;
		//if (Input.GetKey(KeyCode.S)) moveDir.z -= 1;

		//transform.position += moveDir * playerMoveSpeed * Time.deltaTime;

	}

	[ServerRpc]
	public void TestServerRpc(ServerRpcParams serverRpcParams)
	{
		Debug.Log("Test Server RPCs from ID : " + serverRpcParams.Receive.SenderClientId);
	}

	[ClientRpc]
	public void TestClientRpc(ClientRpcParams clientRpcParams)
	{
		Debug.Log("Test Client RPC from ID : " + clientRpcParams.Send.TargetClientIds);
		meshRenderer.material.color = Color.red;
	}
}
