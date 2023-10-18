using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestLobbyUI : MonoBehaviour
{
    [SerializeField] private Button createLobbyButton, queryLobbyButton, joinLobbyButton, quickJoinLobbyButton, changeGameModeButton, printLobbyMessageButton;
	[SerializeField] private TMP_InputField lobbyCodeTextField;
	[SerializeField] private TMP_Dropdown lobbyModeDropdown;

	[SerializeField] private TestLobby testLobby;

	private void Awake()
	{
		createLobbyButton.onClick.AddListener(() =>
		{
			testLobby.CreateLobbyAsync();
		});

		queryLobbyButton.onClick.AddListener(() =>
		{
			testLobby.QueryLobbyAsync();
		});

		joinLobbyButton.onClick.AddListener(() =>
		{
			var lobbyCode = lobbyCodeTextField.text;
			if(lobbyCode.Length != 6)
			{
				Debug.Log("Invalid Lobby Code Syntax");
				return;
			}
			testLobby.JoinLobbyAsync(lobbyCode);
		});

		quickJoinLobbyButton.onClick.AddListener(() =>
		{
			testLobby.QuickJoinLobbyAsync();
		});

		changeGameModeButton.onClick.AddListener(() =>
		{
			testLobby.UpdateLobbyGameMode(lobbyModeDropdown.options[lobbyModeDropdown.value].text);

		});

		printLobbyMessageButton.onClick.AddListener(() =>
		{
			testLobby.PrintLobbyMessage();
		});
	}
}
