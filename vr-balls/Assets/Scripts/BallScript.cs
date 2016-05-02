using UnityEngine;
using System.Collections;
using System;

public class BallScript : MonoBehaviour
{
    private Vector3 target;
    private bool ballHit = false;
    private int insertIndex = -1;
    private int insertCurrentPointIndex = -1;
    private float speed;
    private float targetSpeed = 1f;
    private float fireTargetSpeed = 10.0f;

    public GameObject pathStopperPrefab;

    void Update()
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target, step);

        if (ballHit)
        {
            float distance = Vector3.Distance(transform.position, target);

            if (distance < 0.03f)
            {
                Destroy(gameObject);
                PathStopperScript pathStopperScript = GameObject.FindWithTag("PathStopper").GetComponent<PathStopperScript>();
                pathStopperScript.SetNewBallInserted();

                Color color = GetComponent<MeshRenderer>().material.color;
                BallsPathScript ballsPathScript = GameObject.Find("BallsPath").GetComponent<BallsPathScript>();
                ballsPathScript.InsertBall(insertIndex, insertCurrentPointIndex, target, color);
            }
        }
    }

    public void SetTarget(Vector3 newTarget, bool isFireTarget = false)
    {
        target = newTarget;
        speed = isFireTarget ? fireTargetSpeed : targetSpeed;
    }

    void OnTriggerEnter(Collider other)
    {
        string tag = other.gameObject.tag;

        if (tag == "PathBall" && !ballHit && other.GetComponent<PathBallScript>().isShown)
        {
            ballHit = true;

            Vector3 target = other.gameObject.transform.position;
            Instantiate(pathStopperPrefab, target, Quaternion.identity);

            insertIndex = other.gameObject.GetComponent<PathBallScript>().GetIndex();
            insertCurrentPointIndex = other.gameObject.GetComponent<PathBallScript>().GetCurrentPointIndex();

            float frontBallDistance = BallsManager.GetDistanceToBallAtIndex(insertIndex - 1, transform.position);
            float rearBallDistance = BallsManager.GetDistanceToBallAtIndex(insertIndex + 1, transform.position);

            if (frontBallDistance <= rearBallDistance)
            {
                BallsManager.StopMovingBalls();
                BallsManager.SetBallsPathMovingDirection(PathMovingDirection.Backward, insertIndex);
                BallsManager.StartMovingBalls(insertIndex);
            }
            else
            {
                BallsManager.StopMovingBalls();
                BallsManager.SetBallsPathMovingDirection(PathMovingDirection.Forward, 0, insertIndex + 1);
                BallsManager.StartMovingBalls(0, insertIndex + 1);
            }

            SetTarget(target);
        }
    }
}