using UnityEngine;
using TMPro;

public class SpyStreakUI : MonoBehaviour
{
    [Tooltip("The TextMeshProUGUI component to display the streak.")]
    [SerializeField] private TextMeshProUGUI streakText;
    
    [Tooltip("Text to display before the number.")]
    [SerializeField] private string prefix = "Spies Caught: ";

    private void Start()
    {
        if (RoundManager.Instance != null)
        {
            RoundManager.Instance.OnSpyStreakChanged.AddListener(UpdateStreak);
            // Initialize with current value
            UpdateStreak(RoundManager.Instance.SpyCatchStreak);
        }
    }

    private void UpdateStreak(int count)
    {
        if (streakText != null)
        {
            streakText.text = prefix + count;
        }
    }
}
