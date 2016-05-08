using UnityEngine;
using System.Collections;

public class PathRemoveStopperScript : MonoBehaviour
{
    private BallsPathScript ballsPath;

    void Start()
    {
        ballsPath = GameObject.Find("BallsPath").GetComponent<BallsPathScript>();
    }

    void OnTriggerStay(Collider other)
    {
        string tag = other.gameObject.tag;

        if (tag == "PathBall")
        {
            bool toBeStopped = other.gameObject.GetComponent<PathBallScript>().GetToBeStopped();

            if (toBeStopped)
            {
                float speed = other.gameObject.GetComponent<PathBallScript>().GetSpeed();
                float distance = Mathf.Abs(Vector3.Distance(transform.position, other.transform.position));

                if (distance <= speed * Time.deltaTime)
                {
                    other.transform.position = transform.position;
                    other.gameObject.GetComponent<PathBallScript>().SetToBeStopped(false);

                    Destroy(gameObject);

                    ballsPath.StopMovingBalls();
                    ballsPath.SetBallsSpeed(1f);
                    ballsPath.SetBallsPathMovingDirection(PathMovingDirection.Forward);
                    ballsPath.StartMovingBalls();

                    int newBallIndex = other.gameObject.GetComponent<PathBallScript>().GetIndex();
                    ballsPath.RemoveSameColoredBalls(newBallIndex);
                }
            }
        }
    }
}
