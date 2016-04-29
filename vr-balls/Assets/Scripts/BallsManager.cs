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

    public static void InsertBall(int index, GameObject ball, Color color)
    {
        int currentPointIndex = balls[index].GetComponent<PathBallScript>().GetCurrentPointIndex();

        balls.Insert(index, ball);
        ball.GetComponent<PathBallScript>().SetIndex(index);
        ball.GetComponent<PathBallScript>().SetCurrentPointIndex(currentPointIndex + 1);
        ball.GetComponent<PathBallScript>().SetColor(color);

        for (int i = index + 1; i < balls.Count; i++)
        {
            int previousIndex = balls[i].GetComponent<PathBallScript>().GetIndex();
            balls[i].GetComponent<PathBallScript>().SetIndex(previousIndex + 1);
        }
    }

    public static void StopMovingBalls()
    {
        for (int i = 0; i < balls.Count; i++)
        {
            balls[i].GetComponent<PathBallScript>().StopMoving();
        }
    }

    public static void StartMovingBalls()
    {
        for (int i = 0; i < balls.Count; i++)
        {
            balls[i].GetComponent<PathBallScript>().StartMoving();
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