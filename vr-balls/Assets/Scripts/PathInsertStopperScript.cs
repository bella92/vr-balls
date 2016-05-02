using UnityEngine;
using System.Collections;

public class PathInsertStopperScript : MonoBehaviour
{
    private int newBallIndex = -1;
    private bool neighbourBallsExited = false;
    private bool newBallInserted = false;

    public PathRemoveStopperScript pathRemoveStopperScript;

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
                BallsManager.StopMovingBalls();
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
            BallsManager.SetBallsPathMovingDirection(PathMovingDirection.Forward);
            BallsManager.StartMovingBalls();

            Transform stopperTransform = BallsManager.RemoveSameColoredBalls(newBallIndex);
            if (stopperTransform != null)
            {
                Instantiate(pathRemoveStopperScript, stopperTransform.position, Quaternion.identity);
            }
        }
    }
}
