using UnityEngine;
using System.Collections;

public class PathRemoveStopperScript : MonoBehaviour
{
    void OnTriggerStay(Collider other)
    {
        string tag = other.gameObject.tag;

        if (tag == "PathBall")
        {
            bool toBeStopped = other.gameObject.GetComponent<PathBallScript>().GetToBeStopped();

            if (toBeStopped)
            {
                float distance = Mathf.Abs(Vector3.Distance(transform.position, other.transform.position));

                if (distance < 0.03f)
                {
                    other.transform.position = transform.position;
                    other.gameObject.GetComponent<PathBallScript>().SetToBeStopped(false);

                    Destroy(gameObject);

                    BallsManager.StopMovingBalls();
                    BallsManager.SetBallsPathMovingDirection(PathMovingDirection.Forward);
                    BallsManager.StartMovingBalls();

                    int newBallIndex = other.gameObject.GetComponent<PathBallScript>().GetIndex();
                    Transform stopperTransform = BallsManager.RemoveSameColoredBalls(newBallIndex);
                    if (stopperTransform != null)
                    {
                        Instantiate(gameObject, stopperTransform.position, Quaternion.identity);
                    }
                }
            }
        }
    }
}
