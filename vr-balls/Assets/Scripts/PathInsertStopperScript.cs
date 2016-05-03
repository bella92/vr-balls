using UnityEngine;
using System.Collections;

public class PathInsertStopperScript : MonoBehaviour
{
    private int newBallIndex = -1;
    private bool neighbourBallsExited = false;
    private bool newBallInserted = false;
    private BallsPathScript ballsPath;

    void Start()
    {
        ballsPath = GameObject.Find("BallsPath").GetComponent<BallsPathScript>();
    }

    public void SetNewBallInserted(int newBallIndex)
    {
        this.newBallIndex = newBallIndex;
        newBallInserted = true;
        CheckForFinishedInsertion();
    }

    void OnTriggerExit(Collider other)
    {
        string tag = other.gameObject.tag;

        if (tag == "PathBall")
        {
            bool toBeStopped = other.gameObject.GetComponent<PathBallScript>().GetToBeStopped();

            if (toBeStopped)
            {
                ballsPath.StopMovingBalls();
                neighbourBallsExited = true;

                other.gameObject.GetComponent<PathBallScript>().SetToBeStopped(false);
                CheckForFinishedInsertion();
            }
        }
    }

    private void CheckForFinishedInsertion()
    {
        if (neighbourBallsExited && newBallInserted)
        {
            Destroy(gameObject);
            ballsPath.SetBallsPathMovingDirection(PathMovingDirection.Forward);
            ballsPath.StartMovingBalls();

            ballsPath.RemoveSameColoredBalls(newBallIndex);
        }
    }
}
