using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Particle
{
    Good,
    Bad,
    Meh
}

public class ParticleManager : MonoBehaviour
{

    public static ParticleManager instance;

    private Dictionary<Particle,GameObject> _particleTypes = new Dictionary<Particle,GameObject>();
    private List<GameObject> _activeParticles = new List<GameObject>();

    [SerializeField] private List<GameObject> _prefabs = new List<GameObject>();
    [SerializeField] private List<Particle> _enumerators = new List<Particle>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        for (int i = 0; i< _enumerators.Count && i < _prefabs.Count; i++)
        {
            _particleTypes.Add(_enumerators[i], _prefabs[i]);
        }
    }

    //For spawning the paricles that spawn based on rate over time
    //Position has to be in world space
    public void SpawnParticle(Particle particle,Vector3 position,float despawnTimer, bool keepAlive)
    {
        if (_particleTypes[particle] == null)
        {
            Debug.LogError("Given value doesn't have a game object");
            return;
        }
        GameObject particleGM = Instantiate(_particleTypes[particle],position, _particleTypes[particle].transform.rotation);
        particleGM.transform.position = position;
        ParticleSystem PSystem = particleGM.GetComponent<ParticleSystem>();
        if (PSystem == null)
        {
            Debug.LogError("Game object doesn't have a particle system");
            return;
        }
        PSystem.Play();
        if(!keepAlive)
        {
            StartCoroutine(DespawnParticle(particleGM,despawnTimer));
        }
        else
        {
            _activeParticles.Add(particleGM);
        }
    }

    //For spawning the paricles that emits a specific amount of particles
    public void SpawnParticleWithEmit(Particle particle, Vector3 position, float despawnTimer,int particleCount, bool keepAlive)
    {
        if (_particleTypes[particle] == null)
        {
            Debug.LogError("Given value doesn't have a game object");
            return;
        }
                    GameObject particleGM = Instantiate(_particleTypes[particle], position, _particleTypes[particle].transform.rotation);
                    particleGM.transform.position = position;
                    ParticleSystem PSystem = particleGM.GetComponent<ParticleSystem>();
                            if (PSystem == null)
                            {
                                Debug.LogError("Game object doesn't have a particle system");
                                return;
                            }
                            if (keepAlive)
                            {
                                var main = PSystem.main;
                                main.startLifetime = 10000f;
                            }
                            PSystem.Emit(particleCount);                    if(!keepAlive)
                    {
                        StartCoroutine(DespawnParticle(particleGM, despawnTimer));
                    }
                    else
                    {
                        _activeParticles.Add(particleGM);
                    }    }

    public void SpawnGameObject(GameObject particle, Vector3 position, float despawnTimer, bool keepAlive)
    {
        GameObject particleGM = Instantiate(particle, position, particle.transform.rotation);
        particleGM.transform.position = position;
        ParticleSystem PSystem = particleGM.GetComponent<ParticleSystem>();
        if (PSystem == null)
        {
            Debug.LogError("Game object doesn't have a particle system");
            return;
        }
        PSystem.Play();
        if(!keepAlive)
        {
            StartCoroutine(DespawnParticle(particleGM, despawnTimer));
        }
        else
        {
            _activeParticles.Add(particleGM);
        }
    }

    //For spawning the paricles that emits a specific amount of particles
    public void SpawnGameObjectWithEmit(GameObject particle, Vector3 position, float despawnTimer, int particleCount, bool keepAlive)
    {
        GameObject particleGM = Instantiate(particle, position, particle.transform.rotation);
        particleGM.transform.position = position;
        ParticleSystem PSystem = particleGM.GetComponent<ParticleSystem>();
        if (PSystem == null)
        {
            Debug.LogError("Game object doesn't have a particle system");
            return;
        }
        if (keepAlive)
        {
            var main = PSystem.main;
            main.startLifetime = 10000f;
        }
        PSystem.Emit(particleCount);
        if(!keepAlive)
        {
            StartCoroutine(DespawnParticle(particleGM, despawnTimer));
        }
        else
        {
            _activeParticles.Add(particleGM);
        }
    }

    IEnumerator DespawnParticle(GameObject particleGM, float despawnTimer)
    {
        yield return new WaitForSeconds(despawnTimer);
        particleGM.GetComponent<ParticleSystem>().Stop(false, ParticleSystemStopBehavior.StopEmitting);
        yield return new WaitForSeconds(particleGM.GetComponent<ParticleSystem>().main.startLifetime.constant);
        Destroy(particleGM);
    }

    public void ClearActiveParticles()
    {
        foreach (GameObject particleGM in _activeParticles)
        {
            if (particleGM != null)
            {
                Destroy(particleGM);
            }
        }
        _activeParticles.Clear();
    }
}
