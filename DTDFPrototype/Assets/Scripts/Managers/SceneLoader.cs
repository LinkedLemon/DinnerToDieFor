using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [SerializeField] private GameObject _loadingScreenPanel;
    [SerializeField] private TextMeshProUGUI _loadingText;

    [SerializeField] private float extraWaitTime = 6f;
    [SerializeField] private float minimumLoadTime = 3f;
    private float fakeTimer = 0f;
    private float fakeProgress = 0f;




    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        if (_loadingScreenPanel != null)
        {
            _loadingScreenPanel.SetActive(false);
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        if (_loadingScreenPanel != null)
        {
            _loadingScreenPanel.SetActive(true);
            if (_loadingText != null) _loadingText.text = "Loading...";
        }

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        fakeTimer = 0f;
        fakeProgress = 0f;

        while (!operation.isDone)
        {
            fakeTimer += Time.deltaTime;
            float realProgress = Mathf.Clamp01(operation.progress / 0.9f);

            float targetFake = Mathf.Clamp01(fakeTimer / minimumLoadTime);

            fakeProgress = Mathf.Lerp(fakeProgress, Mathf.Max(realProgress, targetFake), 0.1f);

            if (_loadingText != null)
                _loadingText.text = $"Loading: {fakeProgress * 100:F0}%";

            if (realProgress >= 0.9f && fakeProgress >= 0.99f)
            {
                yield return new WaitForSeconds(extraWaitTime);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        if (_loadingScreenPanel != null)
        {
            _loadingScreenPanel.SetActive(false);
        }

        InputManager.Instance.EnablePlayerInput();
    }

}
