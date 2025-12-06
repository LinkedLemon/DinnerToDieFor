using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public enum CustomerReaction
{
    happy,neutral,dislike,dead
}

public class CustomerAI : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;
    private float _stopDistance = 0f;
    private bool walking = false;
    [SerializeField] private TextMeshProUGUI _textMeshPro;
    [SerializeField] private GameObject modelVisuals;
    
    [Header("Reaction Visuals")]
    [SerializeField] private Renderer reactionPlaneRenderer;
    [SerializeField] private Texture happyTexture;
    [SerializeField] private Texture neutralTexture;
    [SerializeField] private Texture dislikeTexture;
    [SerializeField] private Texture deadTexture;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void WalkToSpot(Vector3 location)
    {
        _agent.destination = location;
    }

    public void SetName(string name)
    {
        _textMeshPro.text = name;
    }

    public void SetNameColor(Color color)
    {
        if (_textMeshPro != null) _textMeshPro.color = color;
    }

    public void HideModel()
    {
        if (modelVisuals != null)
        {
            modelVisuals.SetActive(false);
        }
        else
        {
            // Fallback: try to disable renderer on this object if no visual root assigned
            Renderer r = GetComponent<Renderer>();
            if (r != null) r.enabled = false;
        }
    }

    public void DoReaction(CustomerReaction reaction)
    {
        if (reactionPlaneRenderer == null) return;

        // Ensure the GameObject itself is active
        if (!reactionPlaneRenderer.gameObject.activeSelf)
        {
            reactionPlaneRenderer.gameObject.SetActive(true);
        }
        
        Texture textureToApply = neutralTexture;

        switch (reaction)
        {
            case CustomerReaction.dead:
                textureToApply = deadTexture;
                break;
            case CustomerReaction.happy:
                textureToApply = happyTexture;
                break;
            case CustomerReaction.dislike:
                textureToApply = dislikeTexture;
                break;
            case CustomerReaction.neutral:
                textureToApply = neutralTexture;
                break;
        }

        reactionPlaneRenderer.material.mainTexture = textureToApply;
        reactionPlaneRenderer.enabled = true;
    }
}
