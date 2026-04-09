using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Lobby screen UI.
/// Shows connected players, ready states, room code, and a Ready button.
///
/// TODO:
///   1. Add a Canvas with this component in the Lobby scene.
///   2. Wire Inspector references (playerListPanel, readyButton, roomCodeText).
///   3. Populate playerListPanel with one row per connected player (name + ready tick).
///   4. Disable Start button until all players are ready (host only).
/// </summary>
public class LobbyUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform      playerListPanel;
    [SerializeField] private Button         readyButton;
    [SerializeField] private TMP_Text       roomCodeText;
    [SerializeField] private TMP_InputField nameInputField;

    private LobbyManager _lobbyManager;

    private void Start()
    {
        _lobbyManager = FindObjectOfType<LobbyManager>();
        readyButton?.onClick.AddListener(OnReadyClicked);
    }

    private void OnReadyClicked()
    {
        // Set local player name if changed
        if (nameInputField != null && !string.IsNullOrEmpty(nameInputField.text))
            PlayerPrefs.SetString("PlayerName", nameInputField.text);

        _lobbyManager?.RPC_PlayerReady();
        readyButton.interactable = false; // prevent double-click
    }

    /// <summary>Update the room code display (called by PhotonManager after room creation).</summary>
    public void SetRoomCode(string code)
    {
        if (roomCodeText != null) roomCodeText.text = $"Room: {code}";
    }

    // TODO: add a method to refresh the player list from NetworkedPlayer objects in the scene
}
