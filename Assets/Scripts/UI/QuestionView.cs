using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Renders a single question on screen: one text label for the question
/// and four buttons for the answer options. Handles color feedback
/// (green for correct, red for chosen) using DOTween.
/// </summary>
public class QuestionView : MonoBehaviour
{
    private static readonly Color DefaultColor = new Color(0.78f, 0.78f, 0.78f, 1f);
    private static readonly Color CorrectColor = new Color(0.20f, 0.65f, 0.20f, 1f); // green
    private static readonly Color WrongColor   = new Color(0.78f, 0.18f, 0.18f, 1f); // red
    private const float HighlightDuration = 0.25f; // DOTween tween length
    private const float HoldDuration      = 3.0f;  // how long the highlighted state stays on screen

    [Header("UI References")]
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private Button[] optionButtons = new Button[4];
    [SerializeField] private TMP_Text[] optionLabels = new TMP_Text[4];
    [SerializeField] private Image[] optionButtonImages = new Image[4];

    /// <summary>Fires when the user clicks one of the four option buttons. Argument is the index 0..3.</summary>
    public event Action<int> OnOptionSelected;

    /// <summary>Fires after the highlight + hold time elapses and the next question can be shown.</summary>
    public event Action OnHighlightComplete;

    private bool _isAnswerLocked;

    private void Awake()
    {
        for (int i = 0; i < optionButtons.Length; i++)
        {
            int index = i; // capture for the closure
            if (optionButtons[i] != null)
            {
                optionButtons[i].onClick.AddListener(() =>
                {
                    if (_isAnswerLocked) return;
                    OnOptionSelected?.Invoke(index);
                });
            }
        }
    }

    /// <summary>Fills the UI with the given question. Assumes data.options has 4 elements.</summary>
    public void Display(QuestionData data)
    {
        _isAnswerLocked = false;

        if (questionText != null)
        {
            questionText.text = data.question;
        }

        if (data.options == null)
        {
            Debug.LogError("QuestionView.Display: data.options is null.");
            return;
        }

        int count = Mathf.Min(data.options.Count, optionButtons.Length);
        for (int i = 0; i < count; i++)
        {
            if (optionLabels[i] != null)
            {
                optionLabels[i].text = data.options[i];
            }
            if (optionButtonImages[i] != null)
            {
                optionButtonImages[i].color = DefaultColor;
            }
            if (optionButtons[i] != null)
            {
                optionButtons[i].interactable = true;
            }
        }
    }

    /// <summary>
    /// Plays the answer highlight: the chosen option fades to red, the correct option (if different)
    /// fades to green. After HoldDuration seconds the OnHighlightComplete event fires.
    /// </summary>
    public void HighlightAnswer(int chosenIndex, int correctIndex)
    {
        _isAnswerLocked = true;
        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (optionButtons[i] != null) optionButtons[i].interactable = false;
        }

        Color chosenColor   = (chosenIndex == correctIndex) ? CorrectColor : WrongColor;
        Sequence seq = DOTween.Sequence();

        if (optionButtonImages[chosenIndex] != null)
        {
            seq.Join(optionButtonImages[chosenIndex].DOColor(chosenColor, HighlightDuration));
        }
        if (chosenIndex != correctIndex && optionButtonImages[correctIndex] != null)
        {
            seq.Join(optionButtonImages[correctIndex].DOColor(CorrectColor, HighlightDuration));
        }

        seq.AppendInterval(HoldDuration);
        seq.OnComplete(() => OnHighlightComplete?.Invoke());
        seq.Play();
    }
}
