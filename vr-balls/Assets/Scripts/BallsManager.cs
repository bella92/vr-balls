using System.Collections.Generic;
using UnityEngine;

public static class BallsManager
{
    private static List<GameObject> balls = new List<GameObject>();
    private static float makeRoomSpeed = 1.0f;

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
        Debug.Log("insert ball index: " + index);
        int currentPointIndex = balls[index].GetComponent<PathBallScript>().GetCurrentPointIndex();

        balls.Insert(index, ball);
        ball.GetComponent<PathBallScript>().Init(index, currentPointIndex + 1, color);

        for (int i = index + 1; i < balls.Count; i++)
        {
            int previousIndex = balls[i].GetComponent<PathBallScript>().GetIndex();
            balls[i].GetComponent<PathBallScript>().SetIndex(previousIndex + 1);
        }

        ToggleBallsPathMovingDirection(index + 1, balls.Count);
        ChangeBallsSpeed(makeRoomSpeed);

        StartMovingBalls(0, index);
        StartMovingBalls(index + 1, balls.Count);
    }

    public static void StopMovingBalls(int startIndex = 0, int endIndex = 0)
    {
        if (endIndex == 0)
        {
            endIndex = balls.Count;
        }

        for (int i = startIndex; i < endIndex; i++)
        {
            balls[i].GetComponent<PathBallScript>().StopMoving();
        }
    }

    public static void StartMovingBalls(int startIndex = 0, int endIndex = 0)
    {
        if (endIndex == 0)
        {
            endIndex = balls.Count;
        }

        for (int i = startIndex; i < endIndex; i++)
        {
            balls[i].GetComponent<PathBallScript>().StartMoving();
        }
    }

    public static void ToggleBallsPathMovingDirection(int startIndex = 0, int endIndex = 0)
    {
        if (endIndex == 0)
        {
            endIndex = balls.Count;
        }

        for (int i = startIndex; i < endIndex; i++)
        {
            balls[i].GetComponent<PathBallScript>().TogglePathMovingDirection();
        }
    }

    public static void ChangeBallsSpeed(float speed)
    {
        for (int i = 0; i < balls.Count; i++)
        {
            balls[i].GetComponent<PathBallScript>().ChangeSpeed(speed);
        }
    }

    public static void ShiftBallsColor(int index, Color otherColor)
    {
        Color color = otherColor;

        for (int i = index; i < balls.Count; i++)
        {
            Color currentColor = balls[i].GetComponent<MeshRenderer>().material.color;
            balls[i].GetComponent<MeshRenderer>().material.color = color;
            color = currentColor;
        }
    }
}