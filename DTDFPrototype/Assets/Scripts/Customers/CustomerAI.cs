using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public enum CustomerReaction
{
    ecstatic,happy,neutral,dislike,hatred,dead
}

public class CustomerAI : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;
    private float _stopDistance = 0f;
    private bool walking = false;
    [SerializeField] private TextMeshProUGUI _textMeshPro;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void WalkToSpot(Vector3 location)
    {
        _agent.destination = location;
    }

    public void SetName(string name)
    {
        _textMeshPro.text = name;
    }

    public void DoReaction(CustomerReaction reaction)
    {
        switch(reaction)
        {
            case CustomerReaction.dead:
                break;
            case CustomerReaction.ecstatic:
                break;
            case CustomerReaction.happy:
                break;
            case CustomerReaction.dislike:
                break;
            case CustomerReaction.hatred:
                break;
            default:
                break;

        }
    }
}
