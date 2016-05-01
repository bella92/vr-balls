using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class BallsPathScript : MonoBehaviour
{
    public GameObject pathBallPrefab;
    public GameObject pathTrailPrefab;
    public GameObject pathColliderPrefab;

    private iTweenPath path;

    void Start()
    {
        iTweenPath path = GetComponent<iTweenPath>();
        Vector3[] nodes = iTweenPath.GetPath(path.pathName);
        Vector3[] vector3s = PathControlPointGenerator(nodes);

        int smoothAmount = nodes.Length * 20;

        for (int i = 1; i <= smoothAmount; i++)
        {
            float pm = (float)i / smoothAmount;
            Vector3 currentPoint = Interp(vector3s, pm);
            GameObject pathCollider = (GameObject)Instantiate(pathColliderPrefab, currentPoint, Quaternion.identity);
            PathCollidersManager.AddCollider(pathCollider);
        }

        InitTrail();

        for (int i = 0; i < 20; i++)
        {
            AddBall(i);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            BallsManager.SetBallsPathMovingDirection(PathMovingDirection.Forward);
            BallsManager.ChangeBallsSpeed(1f);
            BallsManager.StartMovingBalls();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            BallsManager.StopMovingBalls();
        }
    }

    private void InitTrail()
    {
        Vector3 spawnPoint = PathCollidersManager.GetColliderPosition(0);
        Instantiate(pathTrailPrefab, spawnPoint, Quaternion.identity);
    }

    private void AddBall(int index)
    {
        Vector3 spawnPoint = PathCollidersManager.GetColliderPosition(0);

        GameObject ball = (GameObject)Instantiate(pathBallPrefab, spawnPoint, Quaternion.identity);

        if (index == 0)
        {
            ball.GetComponent<PathBallScript>().Show();
            ball.GetComponent<PathBallScript>().StartMoving();
        }

        BallsManager.AddBall(ball);
    }

    public void InsertBall(int index, Vector3 position, Color color)
    {
        GameObject ball = (GameObject)Instantiate(pathBallPrefab, position, Quaternion.identity);
        BallsManager.InsertBall(index, ball, color);
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