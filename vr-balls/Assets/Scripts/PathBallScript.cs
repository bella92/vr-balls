using UnityEngine;
using System.Collections;
using System;

public class PathBallScript : MonoBehaviour
{
    public float speed = 0.4f;
    public GameObject previousBall;

    private int currentPointIndex;
    private Vector3[] pathPoints;
    private Color[] colors = { Color.red, Color.blue, Color.green, Color.yellow };
    private BallsPathScript ballsPath;

    public void Init(Vector3[] points, BallsPathScript ballsPathScript)
    {
        SetRandomColor();
        currentPointIndex = -1;
        pathPoints = points;
        ballsPath = ballsPathScript;
        MoveToNextPoint();
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
        else if (tag == "FirstPathCollider")
        {
            MoveToNextPoint();
            GetComponent<MeshRenderer>().enabled = true;
            ballsPath.AddTailBall();
        }
        else if (tag == "Ball")
        {
            GameObject[] pathBalls = GameObject.FindGameObjectsWithTag("PathBall");
            for (int i = 0; i < pathBalls.Length; i++)
            {
                pathBalls[i].GetComponent<PathBallScript>().Stop();
            }
        }
        //else if (tag == "PathBall")
        //{
        //    Destroy(gameObject);
        //}
    }

    private void SetRandomColor()
    {
        int randomIndex = UnityEngine.Random.Range(0, colors.Length);
        GetComponent<MeshRenderer>().material.color = colors[randomIndex];
    }
}
