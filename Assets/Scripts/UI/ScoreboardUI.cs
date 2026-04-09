using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// End-of-session scoreboard — shows final standings across all modes.
///
/// TODO:
///   1. Triggered by GameManager transitioning to the Scoreboard state.
///   2. Populate rows from ScoreManager.GetScoreboard().
///   3. Highlight the winning team.
///   4. Add a "Play Again" button that resets and returns to the lobby.
/// </summary>
public class ScoreboardUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform  rowContainer; // parent for score row prefabs
    [SerializeField] private GameObject rowPrefab;    // TMP_Text x 2 (name + score)
    [SerializeField] private TMP_Text   winnerBanner;

    private ScoreManager _scoreManager;

    private void Start()
    {
        _scoreManager = FindObjectOfType<ScoreManager>();
    }

    /// <summary>Populate the scoreboard. Call when transitioning to Scoreboard state.</summary>
    public void Populate(Dictionary<int, int> scores, string winnerName)
    {
        if (rowContainer == null || rowPrefab == null) return;

        // Clear old rows
        foreach (Transform child in rowContainer)
            Destroy(child.gameObject);

        // Create a row for each player
        foreach (var kvp in scores)
        {
            GameObject row = Instantiate(rowPrefab, rowContainer);
            var texts      = row.GetComponentsInChildren<TMP_Text>();
            if (texts.Length >= 2)
            {
                texts[0].text = $"Player {kvp.Key}";
                texts[1].text = $"{kvp.Value} wins";
            }
        }

        if (winnerBanner != null)
            winnerBanner.text = $"Trophy {winnerName} wins!";
    }
}
