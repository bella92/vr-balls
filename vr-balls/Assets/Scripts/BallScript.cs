using UnityEngine;
using System.Collections;

public class BallScript : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        string tag = other.gameObject.tag;

        if (tag == "PathBall")
        {
        }
    }
}
