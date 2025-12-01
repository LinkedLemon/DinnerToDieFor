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
    
    [Header("Reaction Particles")]
    [SerializeField] private GameObject goodParticle;
    [SerializeField] private GameObject mehParticle;
    [SerializeField] private GameObject badParticle;
    [SerializeField] private GameObject deadParticle;
    [SerializeField] private Transform particleSpawnPoint;

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
        GameObject prefabToSpawn = null;

        switch(reaction)
        {
            case CustomerReaction.dead:
                prefabToSpawn = deadParticle;
                break;
            case CustomerReaction.happy:
                prefabToSpawn = goodParticle;
                break;
            case CustomerReaction.dislike:
                prefabToSpawn = badParticle;
                break;
            case CustomerReaction.neutral:
                prefabToSpawn = mehParticle;
                break;
        }

        if (prefabToSpawn != null)
        {
            Vector3 spawnPos = particleSpawnPoint != null ? particleSpawnPoint.position : transform.position + Vector3.up * 2f;
            Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
        }
    }
}
