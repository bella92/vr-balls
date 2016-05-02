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
            bool isHit = other.gameObject.GetComponent<PathBallScript>().GetIsHit();

            if (isHit)
            {
                BallsManager.StopMovingBalls();
                neighbourBallsExited = true;

                other.gameObject.GetComponent<PathBallScript>().SetIsHit(false);
                CheckForFinishedInsertion();
            }
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
