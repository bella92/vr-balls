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
    }

    public static void AllowBallToMove(int index)
    {
        if (index >= 0 && index < balls.Count)
        {
            balls[index].GetComponent<PathBallScript>().AllowToMove();
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
