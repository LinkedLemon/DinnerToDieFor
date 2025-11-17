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
            bool addedSuccessfully = _activeDish.TryAddGarnish(garnishSO);
            Debug.Log($"Garnish addition status: {(addedSuccessfully ? "Success" : "Failed")}");

            // If the garnish was not added (e.g., list full, duplicate), destroy it.
            if (!addedSuccessfully)
            {
                Debug.Log($"Destroying redundant garnish: {garnishProvider.name}. Spawning Fire.");

                ParticleManager.instance.SpawnParticleWithEmit(Particle.Fire, gameObject.transform.position, 1.0f, 1);
                
                Destroy(other.gameObject);
            }
            else
            {
                // Optional: Do something with the garnish object after it's successfully added.
                // For now, we'll just destroy it.
                Debug.Log("Spawning Sparkle.");
                ParticleManager.instance.SpawnParticleWithEmit(Particle.Sparkle, gameObject.transform.position, 1.0f, 1);
                Destroy(other.gameObject);
            }
        }
    }
}
