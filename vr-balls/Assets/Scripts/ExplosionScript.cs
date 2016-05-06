using UnityEngine;
using System.Collections;

public class ExplosionScript : MonoBehaviour
{
    public float destroyDelay = 3.0f;

    void OnEnable()
    {
        Invoke("Destroy", destroyDelay);
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
