using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro; // For TextMeshProUGUI

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [SerializeField] private GameObject _loadingScreenPanel; // Assign in Inspector
    [SerializeField] private TextMeshProUGUI _loadingText; // Assign in Inspector, if using TextMeshPro
  //  [SerializeField] private Slider _progressBar; // Assign in Inspector, if using a progress bar

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
         //   if (_progressBar != null) _progressBar.value = 0;
        }

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false; // Prevent scene from activating immediately

        while (!operation.isDone)
        {
            // Update progress bar/text
            float progress = Mathf.Clamp01(operation.progress / 0.9f); // operation.progress goes from 0 to 0.9
          //  if (_progressBar != null) _progressBar.value = progress;
            if (_loadingText != null) _loadingText.text = $"Loading: {progress * 100:F0}%";

            // When scene is almost loaded, allow activation
            if (operation.progress >= 0.9f)
            {
                // Optional: Wait for a short delay or user input before activating
                // For now, just activate
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        // Scene is loaded and activated
        if (_loadingScreenPanel != null)
        {
            _loadingScreenPanel.SetActive(false);
        }
        // Potentially re-enable player input or UI input based on the loaded scene
        InputManager.Instance.EnablePlayerInput(); // Assuming most scenes will need player input
    }
}
