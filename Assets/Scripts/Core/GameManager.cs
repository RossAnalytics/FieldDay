using System.Collections.Generic;
using Fusion;
using UnityEngine;

/// <summary>
/// Central singleton that owns high-level game flow.
/// Manages the state machine (Lobby → Playing → Scoreboard) and
/// drives the playlist of sport modes.
/// </summary>
public class GameManager : NetworkBehaviour
{
    // ─── Singleton ─────────────────────────────────────────────────────────────

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ─── Game State ────────────────────────────────────────────────────────────

    public enum GameState { Lobby, Playing, Scoreboard }

    [Networked] public GameState CurrentState { get; private set; }

    // ─── Mode Playlist ─────────────────────────────────────────────────────────

    [Header("Mode Playlist")]
    [Tooltip("Assign one prefab per sport mode (NetworkObject with GameModeBase component).")]
    [SerializeField] private List<GameObject> modesPrefabs;

    private GameModePlaylist _playlist;
    private GameModeBase _currentMode;

    // ─── References ────────────────────────────────────────────────────────────

    [Header("Managers")]
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private TurnManager  turnManager;
    [SerializeField] private TeamManager  teamManager;

    // ─── Fusion Lifecycle ──────────────────────────────────────────────────────

    public override void Spawned()
    {
        if (!Object.HasStateAuthority) return;

        _playlist = new GameModePlaylist(modesPrefabs);
        TransitionTo(GameState.Lobby);
    }

    // ─── State Machine ─────────────────────────────────────────────────────────

    private void TransitionTo(GameState next)
    {
        CurrentState = next;

        switch (next)
        {
            case GameState.Lobby:
                // TODO: load lobby scene, enable LobbyManager
                break;

            case GameState.Playing:
                LaunchNextMode();
                break;

            case GameState.Scoreboard:
                // TODO: show ScoreboardUI with final standings
                break;
        }
    }

    /// <summary>Called by LobbyManager once all players are ready.</summary>
    public void StartGame()
    {
        if (!Object.HasStateAuthority) return;
        scoreManager?.ResetAll();
        _playlist.Reset();
        TransitionTo(GameState.Playing);
    }

    /// <summary>Called by each GameModeBase when its win condition is met.</summary>
    public void OnModeComplete(GameModeBase completedMode)
    {
        if (!Object.HasStateAuthority) return;

        // Tally winner's score on the team leaderboard
        PlayerRef winner = completedMode.CalculateWinner();
        if (winner != PlayerRef.None)
            scoreManager?.AddPoint(winner);

        // Despawn the current mode object
        if (_currentMode != null)
            Runner.Despawn(_currentMode.Object);

        // Move to next mode or end the game
        if (_playlist.HasNext())
            LaunchNextMode();
        else
            TransitionTo(GameState.Scoreboard);
    }

    private void LaunchNextMode()
    {
        GameObject prefab = _playlist.Next();
        if (prefab == null) { TransitionTo(GameState.Scoreboard); return; }

        NetworkObject no = Runner.Spawn(prefab, Vector3.zero, Quaternion.identity);
        _currentMode = no.GetComponent<GameModeBase>();
        _currentMode?.StartMode();
    }
}
