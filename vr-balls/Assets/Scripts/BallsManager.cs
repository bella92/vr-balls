using System;
using System.Collections.Generic;
using UnityEngine;

public static class BallsManager
{
    private static List<GameObject> balls = new List<GameObject>();

    public static int GetCount()
    {
        return balls.Count;
    }

    public static void AddBall(GameObject ball)
    {
        balls.Add(ball);
        int index = balls.Count - 1;
        ball.GetComponent<PathBallScript>().SetIndex(index);
    }

    public static void InsertBall(int index, int currentPointIndex, GameObject ball, Color color)
    {
        GameObject hitBall = GetBallAtIndex(index);
        PathMovingDirection pathMovingDirection = hitBall.GetComponent<PathBallScript>().GetPathMovingDirection();

        int insertIndex = index;
        if (pathMovingDirection == PathMovingDirection.Forward)
        {
            insertIndex += 1;
        }

        for (int i = insertIndex; i < balls.Count; i++)
        {
            int previousIndex = balls[i].GetComponent<PathBallScript>().GetIndex();
            balls[i].GetComponent<PathBallScript>().SetIndex(previousIndex + 1);
        }

        balls.Insert(insertIndex, ball);
        ball.GetComponent<PathBallScript>().Init(insertIndex, currentPointIndex + 1, color);
    }

    public static GameObject GetBallAtIndex(int index)
    {
        if (index >= 0 && index < balls.Count)
        {
            return balls[index];
        }

        return null;
    }

    public static float GetDistanceToBallAtIndex(int index, Vector3 position)
    {
        GameObject ball = GetBallAtIndex(index);
        float ballDistance = float.MinValue;

        if (ball != null)
        {
            ballDistance = Vector3.Distance(ball.transform.position, position);
        }

        return ballDistance;
    }

    public static void StopMovingBalls(int startIndex = 0, int endIndex = -1)
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

    public static void StartMovingBalls(int startIndex = 0, int endIndex = -1)
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

    public static void SetBallsPathMovingDirection(PathMovingDirection direction, int startIndex = 0, int endIndex = -1)
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

    public static void ChangeBallsSpeed(float speed)
    {
        for (int i = 0; i < balls.Count; i++)
        {
            balls[i].GetComponent<PathBallScript>().ChangeSpeed(speed);
        }
    }
}