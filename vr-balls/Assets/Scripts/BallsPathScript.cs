using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class BallsPathScript : MonoBehaviour
{
    public GameObject pathBallPrefab;
    public GameObject pathTrailPrefab;
    public GameObject pathColliderPrefab;
    public Vector3[] points;
    public GameObject followBallPrefab;
    public GameObject headBallPrefab;

    private iTweenPath path;
    private GameObject lastBall;
    private List<GameObject> balls = new List<GameObject>();
    private bool canMove = false;

    void Start()
    {
        iTweenPath path = GetComponent<iTweenPath>();
        Vector3[] nodes = iTweenPath.GetPath(path.pathName);
        Vector3[] vector3s = PathControlPointGenerator(nodes);

        int smoothAmount = nodes.Length * 100;
        points = new Vector3[smoothAmount];

        for (int i = 1; i <= smoothAmount; i++)
        {
            float pm = (float)i / smoothAmount;
            Vector3 currentPoint = Interp(vector3s, pm);
            points[i - 1] = currentPoint;
            //GameObject pathCollider = (GameObject)Instantiate(pathColliderPrefab, currentPoint, Quaternion.identity);
            //PathCollidersManager.AddCollider(pathCollider);
        }

        InitTrail();

        SpawnHeadBall();

        for (int i = 0; i < 20; i++)
        {
            SpawnHeadBall();
        }

        canMove = true;

        //BallsManager.AllowBallToMove(0);
    }

    public Vector3[] GetPoints()
    {
        return points;
    }

    private void InitTrail()
    {
        //Vector3 spawnPoint = PathCollidersManager.GetColliderPosition(0);
        Instantiate(pathTrailPrefab, points[0], Quaternion.identity);
    }

    private void SpawnHeadBall()
    {
        //Vector3 firstPoint = PathCollidersManager.GetColliderPosition(0);
        //Vector3 secondPoint = PathCollidersManager.GetColliderPosition(1);

        //Vector3 differnce = (firstPoint - secondPoint).normalized;
        //Vector3 spawnPoint = firstPoint + differnce * pathBallPrefab.transform.localScale.x * (index + 1);

        Vector3 spawnPoint = points[0];

        GameObject ball = (GameObject)Instantiate(headBallPrefab, spawnPoint, Quaternion.identity);
        
        ball.GetComponent<HeadBall>().SetPoints(points);
        ball.GetComponent<HeadBall>().SetBallAhead(lastBall);

        balls.Add(ball);
        lastBall = ball;
        //BallsManager.AddBall(ball);
        //ball.GetComponent<PathBallScript>().SetIndex(index, true);
    }

    private void SpawnBall()
    {
        //Vector3 firstPoint = PathCollidersManager.GetColliderPosition(0);
        //Vector3 secondPoint = PathCollidersManager.GetColliderPosition(1);

        //Vector3 differnce = (firstPoint - secondPoint).normalized;
        //Vector3 spawnPoint = firstPoint + differnce * pathBallPrefab.transform.localScale.x * (index + 1);

        Vector3 spawnPoint = points[0];

        GameObject ball = (GameObject)Instantiate(followBallPrefab, spawnPoint, Quaternion.identity);
        ball.GetComponent<FollowBall>().SetBall(lastBall);

        balls.Add(ball);
        lastBall = ball;
        //BallsManager.AddBall(ball);
        //ball.GetComponent<PathBallScript>().SetIndex(index, true);
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

    private static Vector3 Interp(Vector3[] pts, float t)
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
}
