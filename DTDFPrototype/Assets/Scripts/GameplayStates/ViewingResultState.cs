using System.Collections;
using TMPro;
using UnityEngine;

public class ViewingResultState : GameState
{
    private readonly GameObject _scoreScreen;
    private readonly GameObject _winScreen;
    private readonly GameObject _loseScreen;
    private readonly TextMeshProUGUI _scoreText;
    
    private readonly float _slideDuration;
    private readonly float _scoreCountDuration;
    private readonly float _bounceDuration;
    private readonly float _postAnimationDelay;

    private readonly RectTransform _scoreScreenRect;
    private readonly Vector2 _scoreScreenOnscreenPos = Vector2.zero;
    private readonly Vector2 _scoreScreenOffscreenPos = new Vector2(0, -1000);

    public ViewingResultState(CoreGameplayManager manager, GameObject scoreScreen, GameObject winScreen, GameObject loseScreen, TextMeshProUGUI scoreText, float slideDuration, float scoreCountDuration, float bounceDuration, float postAnimationDelay) : base(manager)
    {
        _scoreScreen = scoreScreen;
        _winScreen = winScreen;
        _loseScreen = loseScreen;
        _scoreText = scoreText;
        _slideDuration = slideDuration;
        _scoreCountDuration = scoreCountDuration;
        _bounceDuration = bounceDuration;
        _postAnimationDelay = postAnimationDelay;

        if (_scoreScreen != null)
        {
            _scoreScreenRect = _scoreScreen.GetComponent<RectTransform>();
        }
    }

    public override void Enter()
    {
        manager.StartCoroutine(AnimateResults());
    }

    private IEnumerator AnimateResults()
    {
        Debug.Log("Entering ViewingResultState");
        
        // Initial setup
        _scoreScreen.SetActive(true);
        _winScreen.SetActive(false);
        _loseScreen.SetActive(false);
        _scoreScreenRect.anchoredPosition = _scoreScreenOffscreenPos;

        ScoreResult result = ScoreManager.Instance.CalculateOrderScore();
        Debug.Log($"Score: {result.TotalScore} / {result.TargetScore}. Win: {result.Win}");

        // Slide in score screen
        float elapsedTime = 0f;
        while (elapsedTime < _slideDuration)
        {
            _scoreScreenRect.anchoredPosition = Vector2.Lerp(_scoreScreenOffscreenPos, _scoreScreenOnscreenPos, elapsedTime / _slideDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _scoreScreenRect.anchoredPosition = _scoreScreenOnscreenPos;

        // Count up score
        elapsedTime = 0f;
        float startScore = 0;
        while (elapsedTime < _scoreCountDuration)
        {
            float currentScore = Mathf.Lerp(startScore, result.TotalScore, elapsedTime / _scoreCountDuration);
            _scoreText.text = $"Score: {currentScore:F0} / {result.TargetScore}";
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _scoreText.text = $"Score: {result.TotalScore} / {result.TargetScore}";

        // Show win/lose screen with bounce
        GameObject resultScreen = result.Win ? _winScreen : _loseScreen;
        if (resultScreen != null)
        {
            resultScreen.SetActive(true);
            RectTransform resultRect = resultScreen.GetComponent<RectTransform>();
            resultRect.localScale = Vector3.zero;

            elapsedTime = 0f;
            while (elapsedTime < _bounceDuration)
            {
                float scale = Bounce(elapsedTime / _bounceDuration);
                resultRect.localScale = new Vector3(scale, scale, scale);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            resultRect.localScale = Vector3.one;
        }
        
        if (result.Win)
        {
            OrderManager.Instance.ProcessNextOrder();
        }
        else
        {
            Debug.Log("Order failed. Resetting for next order.");
            OrderManager.Instance.ProcessNextOrder();
        }

        yield return new WaitForSeconds(_postAnimationDelay);

        manager.TransitionToState(manager.AwaitingOrderState);
    }
    
    private float Bounce(float t)
    {
        if (t < 0.5f)
        {
            return 2 * t * t;
        }
        else
        {
            t -= 0.75f;
            return 1 - (4 * t * t);
        }
    }

    public override void Update()
    {
        // Update logic is now handled by the coroutine
    }

    public override void Exit()
    {
        Debug.Log("Exiting ViewingResultState");
        if (_scoreScreen != null)
        {
            _scoreScreen.SetActive(false);
        }
        if (_winScreen != null)
        {
            _winScreen.SetActive(false);
        }
        if (_loseScreen != null)
        {
            _loseScreen.SetActive(false);
        }
    }
}