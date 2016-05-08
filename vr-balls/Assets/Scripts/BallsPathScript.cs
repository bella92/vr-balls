using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class BallsPathScript : MonoBehaviour
{
    public GameObject pathBallPrefab;
    public GameObject pathTrailPrefab;
    public GameObject pathColliderPrefab;
    public GameObject entrancePrefab;
    public GameObject exitPrefab;
    public int collidersDensity = 20;
    public float hiddenPart = 0.05f;
    public GameObject pathRemoveStopper;

    private iTweenPath path;
    private List<GameObject> balls = new List<GameObject>();

    void Start()
    {
        iTweenPath path = GetComponent<iTweenPath>();
        Vector3[] nodes = iTweenPath.GetPath(path.pathName);
        Vector3[] vector3s = PathControlPointGenerator(nodes);

        int collidersAmount = nodes.Length * collidersDensity;

        for (int i = 1; i <= collidersAmount; i++)
        {
            float pm = (float)i / collidersAmount;
            Vector3 currentPoint = Interp(vector3s, pm);
            GameObject pathCollider = (GameObject)Instantiate(pathColliderPrefab, currentPoint, Quaternion.identity);
            PathCollidersManager.AddCollider(pathCollider);
        }

        int entranceIndex = Mathf.FloorToInt(collidersAmount * hiddenPart);
        SetEnd(entranceIndex, entrancePrefab);
        int exitIndex = collidersAmount - 1 - entranceIndex;
        SetEnd(exitIndex, exitPrefab);

        //InitTrail();

        for (int i = 0; i < 20; i++)
        {
            AddBall(i);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SetBallsPathMovingDirection(PathMovingDirection.Forward);
            StartMovingBalls();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            StopMovingBalls();
        }
    }

    private void InitTrail()
    {
        Vector3 spawnPoint = PathCollidersManager.GetColliderPosition(0);
        Instantiate(pathTrailPrefab, spawnPoint, Quaternion.identity);
    }

    private void SetEnd(int index, GameObject prefab)
    {
        Vector3 entrancePoint = PathCollidersManager.GetColliderPosition(index);
        Vector3 nextPoint = PathCollidersManager.GetColliderPosition(index + 1);
        Quaternion rotation = Quaternion.LookRotation(nextPoint - entrancePoint);
        Instantiate(prefab, entrancePoint, rotation);
    }

    private void AddBall(int index)
    {
        Vector3 firstPoint = PathCollidersManager.GetColliderPosition(0);
        Vector3 secondPoint = PathCollidersManager.GetColliderPosition(1);

        Vector3 differnce = (firstPoint - secondPoint).normalized;
        Vector3 spawnPoint = firstPoint + differnce * pathBallPrefab.transform.localScale.x * (index + 1);

        GameObject ball = (GameObject)Instantiate(pathBallPrefab, spawnPoint, UnityEngine.Random.rotation);

        ball.GetComponent<PathBallScript>().ChangeCurrentPointIndex();
        ball.GetComponent<PathBallScript>().StartMoving();

        AddBall(ball);
    }

    public void InsertBall(int index, int currentPointIndex, Vector3 position, Color color, bool rearHit)
    {
        GameObject ball = (GameObject)Instantiate(pathBallPrefab, position, UnityEngine.Random.rotation);
        InsertBall(index, currentPointIndex, ball, color, rearHit);
    }

    private Vector3[] PathControlPointGenerator(Vector3[] path)
    {
        Vector3[] suppliedPath;
        Vector3[] vector3s;

        //create and store path points:
        suppliedPath = path;

        //populate calculate path;
        int offset = 2;
        vector3s = new Vector3[suppliedPath.Length + offset];
        Array.Copy(suppliedPath, 0, vector3s, 1, suppliedPath.Length);

        //populate start and end control points:
        //vector3s[0] = vector3s[1] - vector3s[2];
        vector3s[0] = vector3s[1] + (vector3s[1] - vector3s[2]);
        vector3s[vector3s.Length - 1] = vector3s[vector3s.Length - 2] + (vector3s[vector3s.Length - 2] - vector3s[vector3s.Length - 3]);

        //is this a closed, continuous loop? yes? well then so let's make a continuous Catmull-Rom spline!
        if (vector3s[1] == vector3s[vector3s.Length - 2])
        {
            Vector3[] tmpLoopSpline = new Vector3[vector3s.Length];
            Array.Copy(vector3s, tmpLoopSpline, vector3s.Length);
            tmpLoopSpline[0] = tmpLoopSpline[tmpLoopSpline.Length - 3];
            tmpLoopSpline[tmpLoopSpline.Length - 1] = tmpLoopSpline[2];
            vector3s = new Vector3[tmpLoopSpline.Length];
            Array.Copy(tmpLoopSpline, vector3s, tmpLoopSpline.Length);
        }

        return (vector3s);
    }

    private Vector3 Interp(Vector3[] pts, float t)
    {
        int numSections = pts.Length - 3;
        int currPt = Mathf.Min(Mathf.FloorToInt(t * (float)numSections), numSections - 1);
        float u = t * (float)numSections - (float)currPt;

        Vector3 a = pts[currPt];
        Vector3 b = pts[currPt + 1];
        Vector3 c = pts[currPt + 2];
        Vector3 d = pts[currPt + 3];

        return .5f * (
            (-a + 3f * b - 3f * c + d) * (u * u * u)
            + (2f * a - 5f * b + 4f * c - d) * (u * u)
            + (-a + c) * u
            + 2f * b
        );
    }

    public int GetCount()
    {
        return balls.Count;
    }

    public void AddBall(GameObject ball)
    {
        balls.Add(ball);
        int index = balls.Count - 1;
        ball.GetComponent<PathBallScript>().SetIndex(index);
    }

    public void InsertBall(int index, int currentPointIndex, GameObject ball, Color color, bool rearHit)
    {
        GameObject hitBall = GetBallAtIndex(index);
        PathMovingDirection pathMovingDirection = hitBall.GetComponent<PathBallScript>().GetPathMovingDirection();

        int insertIndex = index;
        if (rearHit)
        {
            insertIndex += 1;
        }

        balls.Insert(insertIndex, ball);
        ResetBallsIndexes();

        ball.GetComponent<PathBallScript>().Init(currentPointIndex + 1, color);
    }

    public void ResetBallsIndexes()
    {
        for (int i = 0; i < balls.Count; i++)
        {
            balls[i].GetComponent<PathBallScript>().SetIndex(i);
        }
    }

    public GameObject GetBallAtIndex(int index)
    {
        if (index >= 0 && index < balls.Count)
        {
            return balls[index];
        }

        return null;
    }

    public float GetDistanceToBallAtIndex(int index, Vector3 position)
    {
        GameObject ball = GetBallAtIndex(index);
        float ballDistance = float.MinValue;

        if (ball != null)
        {
            ballDistance = Mathf.Abs(Vector3.Distance(ball.transform.position, position));
        }

        return ballDistance;
    }

    public void StopMovingBalls(int startIndex = 0, int endIndex = -1)
    {
        if (endIndex == -1)
        {
            endIndex = balls.Count;
        }

        for (int i = startIndex; i < endIndex; i++)
        {
            balls[i].GetComponent<PathBallScript>().StopMoving();
        }
    }

    public void StartMovingBalls(int startIndex = 0, int endIndex = -1)
    {
        if (endIndex == -1)
        {
            endIndex = balls.Count;
        }

        for (int i = startIndex; i < endIndex; i++)
        {
            balls[i].GetComponent<PathBallScript>().StartMoving();
        }
    }

    public void SetBallsPathMovingDirection(PathMovingDirection direction, int startIndex = 0, int endIndex = -1)
    {
        if (endIndex == -1)
        {
            endIndex = balls.Count;
        }

        for (int i = startIndex; i < endIndex; i++)
        {
            balls[i].GetComponent<PathBallScript>().SetPathMovingDirection(direction);
        }
    }

    public void SetBallsSpeed(float speed, int startIndex = 0, int endIndex = -1)
    {
        if (endIndex == -1)
        {
            endIndex = balls.Count;
        }

        for (int i = startIndex; i < endIndex; i++)
        {
            balls[i].GetComponent<PathBallScript>().SetSpeed(speed);
        }
    }

    public void RemoveSameColoredBalls(int newBallIndex)
    {
        int startIndex = newBallIndex;
        int endIndex = newBallIndex;

        Color newBallColor = balls[newBallIndex].GetComponent<MeshRenderer>().material.color;
        
        int index = newBallIndex - 1;

        while (index >= 0 && balls[index].GetComponent<MeshRenderer>().material.color == newBallColor)
        {
            startIndex = index;
            index -= 1;
        }

        index = newBallIndex + 1;

        while (index < balls.Count && balls[index].GetComponent<MeshRenderer>().material.color == newBallColor)
        {
            endIndex = index;
            index += 1;
        }

        int count = endIndex - startIndex + 1;

        if (count >= 3)
        {
            Transform stopperTransform = GetBallAtIndex(endIndex).transform;

            for (int i = startIndex; i <= endIndex; i++)
            {
                balls[i].GetComponent<PathBallScript>().SelfDestroy();
            }

            balls.RemoveRange(startIndex, count);
            ResetBallsIndexes();

            GameObject ballAhead = GetBallAtIndex(startIndex - 1);

            if (ballAhead != null)
            {
                StopMovingBalls();

                Instantiate(pathRemoveStopper, stopperTransform.position, Quaternion.identity);

                ballAhead.GetComponent<PathBallScript>().SetToBeStopped(true);
                SetBallsSpeed(4f, 0, startIndex);
                SetBallsPathMovingDirection(PathMovingDirection.Backward, 0, startIndex);
                StartMovingBalls(0, startIndex);
            }
        }
    }
}