using UnityEngine;
using UnityEngine.UI;

public class MainMenuPanel : MonoBehaviour
{
    [SerializeField] private Button _startGameButton;

    private void Awake()
    {
        if (_startGameButton != null)
        {
            _startGameButton.onClick.AddListener(StartGame);
        }
    }

    public void StartGame()
    {
       // Debug.Log("Start Game button clicked!");
        UIManager.Instance.HideAllPanels(); 
        SceneLoader.Instance.LoadScene("ResturauntTesting"); 
    }


}
