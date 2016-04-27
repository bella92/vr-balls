using UnityEngine;
using System.Collections;

public class SmoothFollow : MonoBehaviour
{
    public Transform target;
    float smoothTime = 2f;
    float xOffset = 0.8f;
    float yOffset = 0.8f;
    float zOffset = 0.8f;

    void LateUpdate()
    {
        float x = Mathf.Lerp(transform.position.x, target.position.x, Time.deltaTime * smoothTime);
        float y = Mathf.Lerp(transform.position.y, target.position.y, Time.deltaTime * smoothTime);
        float z = Mathf.Lerp(transform.position.z, target.position.z, Time.deltaTime * smoothTime);

        transform.position = new Vector3(x, y, z);
    }
}
