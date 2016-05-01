using System;
using System.Collections.Generic;
using UnityEngine;

public static class BallsManager
{
    private static List<GameObject> balls = new List<GameObject>();
    private static float makeRoomSpeed = 0.3f;

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

    public static void InsertBall(int index, GameObject ball, Color color)
    {
        int currentPointIndex = balls[index].GetComponent<PathBallScript>().GetCurrentPointIndex();

        balls.Insert(index, ball);
        ball.GetComponent<PathBallScript>().Init(index, currentPointIndex + 1, color);

        int waitCount = 2;
        if (index == 0 || index == balls.Count - 1)
        {
            waitCount = 1;
        }
        ball.GetComponent<PathBallScript>().SetWaitCount(waitCount);

        for (int i = index + 1; i < balls.Count; i++)
        {
            int previousIndex = balls[i].GetComponent<PathBallScript>().GetIndex();
            balls[i].GetComponent<PathBallScript>().SetIndex(previousIndex + 1);
        }

        SetBallsPathMovingDirection(PathMovingDirection.Backward, index + 1, balls.Count);
        ChangeBallsSpeed(makeRoomSpeed);

        StartMovingBalls(0, index);
        StartMovingBalls(index + 1, balls.Count);
    }

    public static GameObject GetBallAtIndex(int index)
    {
        if (index >= 0 && index < balls.Count)
        {
            return balls[index];
        }

        return null;
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

    public static int FindInsertIndex(Vector3 position, float insideDiameter)
    {
        float minDistance = insideDiameter;
        int minDistanceIndex = -1;

        int secondMinDistanceIndex = -1;
        float secondMinDistance = insideDiameter;

        for (int i = 0; i < balls.Count; i++)
        {
            float distance = Vector3.Distance(position, balls[i].transform.position);

            if (distance < minDistance)
            {
                secondMinDistance = minDistance;
                secondMinDistanceIndex = minDistanceIndex;

                minDistance = distance;
                minDistanceIndex = i;
            }
            else if (distance < secondMinDistance && distance != minDistance)
            {
                secondMinDistance = distance;
                secondMinDistanceIndex = i;
            }
        }

        int index = Math.Max(minDistanceIndex, secondMinDistanceIndex);
        return index;
    }
}