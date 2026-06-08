using UnityEngine;

/// <summary>
/// Owns the quiz flow: loads questions, shuffles their order, and shows
/// them one-by-one through the QuestionView. Listens to the view's option
/// click events and logs whether the chosen option was correct.
/// </summary>
public class GameController : MonoBehaviour
{
    [Header("Wiring")]
    [SerializeField] private QuestionView view;

    [Header("Data")]
    [Tooltip("File name (without extension) inside any Resources folder.")]
    [SerializeField] private string resourceName = "low_questions";

    private QuestionList _list;
    private int[] _order;
    private int _current;

    private void Awake()
    {
        var repo = new QuestionRepository();
        _list = repo.Load(resourceName);

        if (_list.QuestionDatas == null || _list.QuestionDatas.Count == 0)
        {
            Debug.LogError("GameController: no questions loaded — nothing to show.");
            _order = new int[0];
            return;
        }

        _order = BuildShuffledOrder(_list.QuestionDatas.Count);
        _current = 0;

        Debug.Log($"GameController: shuffled order = [{string.Join(",", _order)}]");
    }

    private void Start()
    {
        if (view != null)
        {
            view.OnOptionSelected += HandleOptionSelected;
            view.OnHighlightComplete += HandleHighlightComplete;
            ShowCurrent();
        }
        else
        {
            Debug.LogError("GameController: QuestionView reference is not set in the inspector.");
        }
    }

    private void OnDestroy()
    {
        if (view != null)
        {
            view.OnOptionSelected -= HandleOptionSelected;
            view.OnHighlightComplete -= HandleHighlightComplete;
        }
    }

    private void HandleOptionSelected(int optionIndex)
    {
        if (_order == null || _order.Length == 0) return;
        if (_current < 0 || _current >= _order.Length) return;

        var q = _list.QuestionDatas[_order[_current]];
        bool isCorrect = optionIndex == q.correct;
        Debug.Log($"Selected option {optionIndex} (correct={q.correct}) → {(isCorrect ? "correct" : "incorrect")}.");

        view.HighlightAnswer(optionIndex, q.correct);
    }

    private void HandleHighlightComplete()
    {
        ShowNext();
    }

    private void ShowNext()
    {
        _current++;
        if (_current >= _order.Length)
        {
            Debug.Log("No more questions.");
            return;
        }
        ShowCurrent();
    }

    private void ShowCurrent()
    {
        if (_order == null || _order.Length == 0) return;
        if (_current < 0 || _current >= _order.Length) return;

        var q = _list.QuestionDatas[_order[_current]];
        view.Display(q);
        Debug.Log($"Showing question {_current + 1} of {_order.Length}.");
    }

    /// <summary>Returns [0..n-1] in a uniformly random order (Fisher-Yates).</summary>
    private static int[] BuildShuffledOrder(int n)
    {
        var arr = new int[n];
        for (int i = 0; i < n; i++) arr[i] = i;

        for (int i = n - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int tmp = arr[i];
            arr[i] = arr[j];
            arr[j] = tmp;
        }
        return arr;
    }
}
