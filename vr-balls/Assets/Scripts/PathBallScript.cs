using UnityEngine;
using System.Collections;
using System;

public class PathBallScript : MonoBehaviour
{
    public float speed = 0.4f;
    public GameObject previousBall;
    public GameObject nextBall;
    public Color? newBallColor = null;

    private int currentPointIndex;
    private Vector3[] pathPoints;
    private Color[] colors = { Color.red, Color.blue, Color.green, Color.yellow };
    private BallsPathScript ballsPath;

    private float nextUsage;
    private float delay = 3f;

    void Start()
    {
        nextUsage = Time.time + delay;
    }

    public void Init(Vector3[] points, BallsPathScript ballsPathScript)
    {
        currentPointIndex = -1;
        pathPoints = points;
        ballsPath = ballsPathScript;

        SetInitialColor();

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
        else if (tag == "Ball")
        {
            GameObject[] pathBalls = GameObject.FindGameObjectsWithTag("PathBall");
            for (int i = 0; i < pathBalls.Length; i++)
            {
                pathBalls[i].GetComponent<PathBallScript>().Stop();
            }

            Color otherColor = other.GetComponent<MeshRenderer>().material.color;
            Destroy(other.gameObject);

            TransferColorToBallBehind(otherColor);
        }
    }

    public void TransferColorToBallBehind(Color otherColor)
    {
        Debug.Log("inside");
        if (previousBall == null)
        {
            newBallColor = otherColor;
            return;
        }

        Color currentColor = GetComponent<MeshRenderer>().material.color;
        previousBall.GetComponent<PathBallScript>().TransferColorToBallBehind(currentColor);

        GetComponent<MeshRenderer>().material.color = otherColor;
    }

    void OnTriggerExit(Collider other)
    {
        string tag = other.gameObject.tag;

        if (tag == "FirstPathCollider")
        {
            MoveToNextPoint();
            GetComponent<MeshRenderer>().enabled = true;
            ballsPath.AddTailBall();
        }
    }

    private void SetInitialColor()
    {
        int randomIndex = UnityEngine.Random.Range(0, colors.Length);
        Color color = colors[randomIndex];

        if (nextBall != null)
        {
            Color? newBallColor = nextBall.GetComponent<PathBallScript>().newBallColor;
            if (newBallColor != null)
            {
                color = (Color)newBallColor;
            }
        }

        GetComponent<MeshRenderer>().material.color = color;
    }
}
