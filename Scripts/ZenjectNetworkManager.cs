using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Zenject;
/*
public class ZenjectNetworkManager : NetworkManager, IInitializable
{
	[Inject]
	DiContainer Container;

	// The player object's prefab.
	GameObject _playerPrefab;

	// Used by our 'Zenject' custom spawn instantiation.
	Dictionary<NetworkHash128, GameObject> assetIdToPrefab = new Dictionary<NetworkHash128, GameObject>();

	public void Initialize()
	{
		Debug.Log("Initialize()");

		// Preparation for registering our own spawn handlers.
		assetIdToPrefab[playerPrefab.GetComponent<NetworkIdentity>().assetId] = playerPrefab;
		foreach (GameObject prefab in spawnPrefabs)
		{
			assetIdToPrefab[prefab.GetComponent<NetworkIdentity>().assetId] = prefab;
		}

		// Do not let the NetworkManager register the playerPrefab on the client, as the prefab instantiation
		// has a higher priority compared to the custom spawn handler instantiation method.
		_playerPrefab = playerPrefab;
		playerPrefab = null;

		// The same, we don't want the NetworkManager to register those prefabs as it would hide our
		// custom spawn handler instantiation method.
		spawnPrefabs.Clear();
	}

	// On the server, we create the player object using Zenject's container.
	public override void OnServerAddPlayer(NetworkConnection connection, short playerControllerId)
	{
		Debug.Log("OnServerAddPlayer(" + playerControllerId + ")");
		GameObject player = Container.InstantiatePrefab(_playerPrefab);
		NetworkServer.AddPlayerForConnection(connection, player, playerControllerId);
	}

	public override void OnStartClient(NetworkClient client)
	{
		base.OnStartClient(client);
		RegisterCustomSpawners();
	}

	public override void OnStopClient()
	{
		base.OnStopClient();
		UnregisterCustomSpawners();
	}

	private void RegisterCustomSpawners()
	{
		foreach (NetworkHash128 assetId in assetIdToPrefab.Keys)
		{
			Debug.Log("Register assetId " + assetId.ToString());
			ClientScene.RegisterSpawnHandler(assetId, Spawn, UnSpawn);
		}
	}

	private void UnregisterCustomSpawners()
	{
		foreach (NetworkHash128 assetId in assetIdToPrefab.Keys)
		{
			Debug.Log("Unregister assetId " + assetId.ToString());
			ClientScene.UnregisterSpawnHandler(assetId);
		}
	}

	// On the client, instantiate the prefab using Zenject's container.
	private GameObject Spawn(Vector3 position, NetworkHash128 assetId)
	{
		Debug.Log("Spawn(" + assetId.ToString() + ")");
		return Container.InstantiatePrefab(assetIdToPrefab[assetId]);
	}

	// On the client, destroy the game object.
	private void UnSpawn(GameObject spawned)
	{
		Debug.Log("UnSpawn()");
		Destroy(spawned.gameObject);
	}

}
*/