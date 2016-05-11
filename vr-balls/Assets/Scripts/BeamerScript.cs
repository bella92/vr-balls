using UnityEngine;

public class BeamerScript : MonoBehaviour
{
    private bool test = false;

    void OnTriggerEnter(Collider other)
    {
        string tag = other.gameObject.tag;

        if (tag == "PathBall")
        {
            var speed = other.GetComponent<PathBallScript>().GetSpeed();

            Animator startBeamerAnimator = other.GetComponentInChildren<Animator>();
            startBeamerAnimator.SetFloat("Speed", speed);

            test = !test;
            startBeamerAnimator.SetBool("Shake", test);
        }
    }
}
