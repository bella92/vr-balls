using UnityEngine;
using System.Collections;
using System;

public class PathBallScript : MonoBehaviour
{
    public float speed = 0.4f;

    private int currentPointIndex;
    private Vector3[] pathPoints;

    void Update()
    {
        Vector3 position = transform.position;

        int index = Array.FindIndex(pathPoints, point => point.x == position.x &&
            point.y == position.y &&
            point.z == position.z);

        if (index != -1)
        {
            MoveToNextPoint();
        }
    }

    public void SetPathPoints(Vector3[] points)
    {
        currentPointIndex = 0;
        pathPoints = points;
    }

    public void MoveToNextPoint()
    {
        currentPointIndex += 1;

        if (currentPointIndex >= pathPoints.Length)
        {
            Destroy(gameObject);
        }
        else
        {
            Vector3 nextPoint = pathPoints[currentPointIndex];

            Vector3 direction = (nextPoint - transform.position).normalized;
            GetComponent<Rigidbody>().velocity = direction * speed;
        }
    }

    public void Stop()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    void OnTriggerEnter(Collider other)
    {
        string tag = other.gameObject.tag;

        if (tag == "PathCollider")
        {
            MoveToNextPoint();
        }
        else if (tag == "Ball")
        {
            GameObject[] ballsPaths = GameObject.FindGameObjectsWithTag("BallsPath");

            if (ballsPaths.Length > 0)
            {
                GameObject ballsPath = ballsPaths[0];
                ballsPath.GetComponent<BallsPathScript>().Stop();
            }

            //GameObject[] pathBalls = GameObject.FindGameObjectsWithTag("PathBall");
            //Debug.Log(pathBalls.Length);

            //for (int i = 0; i < pathBalls.Length; i++)
            //{
            //    pathBalls[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
            //}
        }
    }
}
