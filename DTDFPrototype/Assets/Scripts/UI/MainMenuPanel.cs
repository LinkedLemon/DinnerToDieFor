using UnityEngine;
using UnityEngine.UI;
using TMPro; // Required for TextMeshPro

public class MainMenuPanel : MonoBehaviour
{
    [SerializeField] private Button _startGameButton;
    [SerializeField] private TextMeshProUGUI _titleText; // Assuming you're using TextMeshPro

    private void Awake()
    {
        if (_startGameButton != null)
        {
            _startGameButton.onClick.AddListener(StartGame);
        }
    }

    public void StartGame()
    {
        Debug.Log("Start Game button clicked!");
        UIManager.Instance.HideAllPanels(); 
        SceneLoader.Instance.LoadScene("ResturauntTesting"); // Example scene name
    }

    // You can add other methods here for other buttons or UI interactions
}
