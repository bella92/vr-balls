using UnityEngine;
using System.Collections;

public class ExplosionScript : MonoBehaviour
{
    private ParticleSystem[] particleSystems;

    void Start()
    {
        particleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    void OnEnable()
    {
        if (particleSystems != null)
        {
            float slowestDuration = float.MaxValue;

            for (int i = 0; i < particleSystems.Length; i++)
            {
                particleSystems[i].Play();

                if (particleSystems[i].duration < slowestDuration)
                {
                    slowestDuration = particleSystems[i].duration;
                }
            }

            Invoke("Destroy", slowestDuration);
        }
    }

    void Destroy()
    {
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        CancelInvoke();
    }
}
