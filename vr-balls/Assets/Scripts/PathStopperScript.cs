using UnityEngine;
using System.Collections;

public class PathStopperScript : MonoBehaviour
{
    private bool neighbourBallsExited = false;
    public bool newBallInserted = false;

    public void SetNewBallInserted()
    {
        newBallInserted = true;
        CheckForFinishedInsertion();
    }

    void OnTriggerExit(Collider other)
    {
        string tag = other.gameObject.tag;

        if (tag == "PathBall")
        {
            BallsManager.StopMovingBalls();
            neighbourBallsExited = true;

            CheckForFinishedInsertion();
        }
    }

    private void CheckForFinishedInsertion()
    {
        if (neighbourBallsExited && newBallInserted)
        {
            Destroy(gameObject);
            BallsManager.SetBallsPathMovingDirection(PathMovingDirection.Forward);
            BallsManager.StartMovingBalls();
        }
    }
}
