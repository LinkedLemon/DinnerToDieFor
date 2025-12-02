using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private GameObject _pauseMenuPanel;
    [SerializeField] private GameObject _loadingScreenPanel;

    private Stack<GameObject> _activePanels = new Stack<GameObject>();

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
    }

    private void Start()
    {
        // Initially hide all panels
        HideAllPanels();
        // Example: Show main menu on start
        ShowPanel(_mainMenuPanel);
    }

    public void ShowPanel(GameObject panel)
    {
        if (panel == null) return;

        // Hide the current top panel if any
        if (_activePanels.Count > 0)
        {
            _activePanels.Peek().SetActive(false);
        }

        panel.SetActive(true);
        _activePanels.Push(panel);
        InputManager.Instance.EnableUIInput(); // Enable UI input when a panel is shown
    }

    public void HideCurrentPanel()
    {
        if (_activePanels.Count > 0)
        {
            GameObject currentPanel = _activePanels.Pop();
            currentPanel.SetActive(false);
        }

        // Show the previous panel if any
        if (_activePanels.Count > 0)
        {
            _activePanels.Peek().SetActive(true);
        }
        else
        {
            InputManager.Instance.EnablePlayerInput(); // Enable player input if no UI panels are active
        }
    }

    public void HideAllPanels()
    {
        while (_activePanels.Count > 0)
        {
            _activePanels.Pop().SetActive(false);
        }
        if (_mainMenuPanel != null) _mainMenuPanel.SetActive(false);
        if (_pauseMenuPanel != null) _pauseMenuPanel.SetActive(false);
        if (_loadingScreenPanel != null) _loadingScreenPanel.SetActive(false);
        InputManager.Instance.EnablePlayerInput(); // Ensure player input is enabled after hiding all panels
    }

    public void ShowMainMenu()
    {
        HideAllPanels();
        ShowPanel(_mainMenuPanel);
    }

    public void ShowPauseMenu()
    {
        ShowPanel(_pauseMenuPanel);
    }

    public void ShowLoadingScreen()
    {
        HideAllPanels();
        ShowPanel(_loadingScreenPanel);
    }

    public void HideLoadingScreen()
    {
        HideCurrentPanel(); // This assumes loading screen is the top-most panel
    }
}
