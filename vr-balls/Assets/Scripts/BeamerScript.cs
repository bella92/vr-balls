using UnityEngine;
using System.Collections.Generic;

public class BeamerScript : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        string tag = other.gameObject.tag;

        if (tag == "PathBall")
        {
            var isShown = other.GetComponent<PathBallScript>().GetIsShown();

            if (isShown)
            {
                Animator animator = GetComponentInChildren<Animator>();
                animator.SetTrigger("Shake");
            }
        }
    }
}
