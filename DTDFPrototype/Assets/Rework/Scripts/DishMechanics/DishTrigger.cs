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
            GarnishAddResult result = _activeDish.TryAddGarnish(garnishSO._garnishType);

            switch (result)
            {
                case GarnishAddResult.Success_Preferred:
                case GarnishAddResult.Success_Meh:
                case GarnishAddResult.Failure_BadGarnish:
                case GarnishAddResult.Failure_Allergy:
                    AttachGarnishVisual(garnishSO.GarnishPrefab);
                    break;

                case GarnishAddResult.Failure_DuplicateOrFull:
                    Debug.Log("Duplicate or too many garnishes.");
                    break;
            }
            
            // In all cases, destroy the garnish that was used.
            Destroy(other.gameObject);
        }
    }

    private void AttachGarnishVisual(GameObject prefab)
    {
        if (prefab != null)
        {
            int garnishIndex = _activeDish.AppliedGarnishes.Count - 1;
            if (garnishIndex >= 0 && garnishIndex < GarnishAttachmentPoints.Count)
            {
                Transform attachmentPoint = GarnishAttachmentPoints[garnishIndex];
                Instantiate(prefab, attachmentPoint.position, attachmentPoint.rotation, attachmentPoint);
            }
            else
            {
                Debug.LogWarning("Added more garnishes than there are attachment points!");
            }
        }
    }
}
