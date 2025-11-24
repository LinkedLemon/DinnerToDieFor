using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component should be placed on a child GameObject of a dish prefab.
/// The child GameObject must have a Collider set to "Is Trigger".
/// It detects garnishes entering its zone and attempts to add them to the dish.
/// </summary>
[RequireComponent(typeof(Collider))]
public class DishTrigger : MonoBehaviour
{
    private ActiveDish _activeDish;
    
    [Tooltip("The points where garnishes will be visually attached. Should be 3.")]
    public  List<Transform> GarnishAttachmentPoints;

    public AudioClip successSound;
    public AudioClip badSound;
    public AudioClip mehSound;

    /// <summary>
    /// Initializes the trigger with a reference to its controlling ActiveDish instance.
    /// </summary>
    public void Initialize(ActiveDish activeDish)
    {
        _activeDish = activeDish;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_activeDish == null) return;

        // Check if the colliding object is a garnish
        if (other.TryGetComponent<GarnishProvider>(out var garnishProvider))
        {
            GarnishGameObject garnishSO = garnishProvider.GarnishData;
            if (garnishSO == null) return;

            // Attempt to add the garnish to the dish
            GarnishAddResult result = _activeDish.TryAddGarnish(garnishSO);

            switch (result)
            {
                case GarnishAddResult.Success_Preferred:
                    Debug.Log("Correct garnish added. Spawning Sparkle.");
                    //TODO: hook up new sound for prefered garnish
                    ParticleManager.instance.SpawnParticleWithEmit(Particle.Good, gameObject.transform.position, 1.0f, 1);
                    if (successSound != null)
                    {
                        AudioSource.PlayClipAtPoint(successSound, gameObject.transform.position);
                    }
                    
                    // Visually attach the garnish
                    if (garnishSO.GarnishPrefab != null)
                    {
                        int garnishIndex = _activeDish.AppliedGarnishes.Count - 1;
                        if (garnishIndex < GarnishAttachmentPoints.Count)
                        {
                            Transform attachmentPoint = GarnishAttachmentPoints[garnishIndex];
                            Instantiate(garnishSO.GarnishPrefab, attachmentPoint.position, attachmentPoint.rotation, attachmentPoint);
                        }
                        else
                        {
                            Debug.LogWarning("Added more garnishes than there are attachment points!");
                        }
                    }
                    
                    break;
                case GarnishAddResult.Failure_BadGarnish:
                    Debug.Log("Wrong garnish added. Spawning Fire.");
                    //TODO: hook up new sound for bad garnish
                    ParticleManager.instance.SpawnParticleWithEmit(Particle.Bad, gameObject.transform.position, 1.0f, 1);
                    if (badSound != null)
                    {
                        AudioSource.PlayClipAtPoint(badSound, gameObject.transform.position);
                    }
                    
                    // Visually attach the bad garnish
                    if (garnishSO.GarnishPrefab != null)
                    {
                        int garnishIndex = _activeDish.AddedGarnishCount - 1;
                        if (garnishIndex < GarnishAttachmentPoints.Count)
                        {
                            Transform attachmentPoint = GarnishAttachmentPoints[garnishIndex];
                            Instantiate(garnishSO.GarnishPrefab, attachmentPoint.position, attachmentPoint.rotation, attachmentPoint);
                        }
                        else
                        {
                            Debug.LogWarning("Added more garnishes than there are attachment points!");
                        }
                    }
                    
                    break;
                case GarnishAddResult.Failure_DuplicateOrFull:
                    Debug.Log("Duplicate or too many garnishes. Spawning Fire.");
                    //TODO: hook up new sound for duplicate or full
                    ParticleManager.instance.SpawnParticleWithEmit(Particle.Meh, gameObject.transform.position, 1.0f, 1);
                    if (mehSound != null)
                    {
                        AudioSource.PlayClipAtPoint(mehSound, gameObject.transform.position);
                    }
                    break;
            }
            
            // In all cases, destroy the garnish that was used.
            Destroy(other.gameObject);
        }
    }
}
