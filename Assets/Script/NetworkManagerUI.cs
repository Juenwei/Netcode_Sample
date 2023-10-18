using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button hostButton, serverButton, clientButton;


	private void Awake()
	{
		hostButton.onClick.AddListener(() =>
		{
			NetworkManager.Singleton.StartHost();
		});
		serverButton.onClick.AddListener(() =>
		{
			NetworkManager.Singleton.StartServer();
		});
		clientButton.onClick.AddListener(() =>
		{
			NetworkManager.Singleton.StartClient();
		});
	}
}
