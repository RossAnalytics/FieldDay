using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// In-game HUD — shows current mode name, active player indicator,
/// turn count, timer, and per-player score summary.
///
/// TODO:
///   1. Create a Canvas (Screen Space — Overlay) with this component.
///   2. Wire Inspector references.
///   3. Subscribe to GameManager state changes and TurnManager events.
///   4. Animate turn changes (slide in active player name, etc.).
/// </summary>
public class HUDManager : MonoBehaviour
{
    [Header("HUD Elements")]
    [SerializeField] private TMP_Text modeNameText;
    [SerializeField] private TMP_Text activePlayerText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Slider   powerBar; // shown during aiming phases

    private TurnManager _turnManager;

    private void Start()
    {
        _turnManager = FindObjectOfType<TurnManager>();
    }

    private void Update()
    {
        if (_turnManager != null && activePlayerText != null)
            activePlayerText.text = $"Active: {_turnManager.ActivePlayer}";

        // TODO: poll GameManager for mode name and timer, update text fields
    }

    /// <summary>Show or hide the power bar (call from aiming scripts).</summary>
    public void SetPowerBarValue(float value)
    {
        if (powerBar != null) powerBar.value = Mathf.Clamp01(value);
    }
}
