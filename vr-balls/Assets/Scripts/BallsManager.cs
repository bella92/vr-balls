using System;
using System.Collections.Generic;
using UnityEditor;
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

    public static void InsertBall(int index, int currentPointIndex, GameObject ball, Color color, bool rearHit)
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

    public static void ResetBallsIndexes()
    {
        for (int i = 0; i < balls.Count; i++)
        {
            balls[i].GetComponent<PathBallScript>().SetIndex(i);
        }
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
            ballDistance = Mathf.Abs(Vector3.Distance(ball.transform.position, position));
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

    public static Transform RemoveSameColoredBalls(int newBallIndex)
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

                ballAhead.GetComponent<PathBallScript>().SetToBeStopped(true);
                SetBallsPathMovingDirection(PathMovingDirection.Backward, 0, startIndex);
                StartMovingBalls(0, startIndex);

                return stopperTransform;
            }
        }

        return null;
    }
}