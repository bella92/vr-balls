using System.Collections.Generic;
using UnityEngine;

public static class BallsManager
{
    private const int minCountToRemove = 3;
    private static List<GameObject> balls = new List<GameObject>();

    public static int GetCount()
    {
        return balls.Count;
    }
    
    public static void AddBall(GameObject ball)
    {
        balls.Add(ball);
        //RemoveTriples();
    }

    public static void InsertBall(int index, GameObject ball)
    {
        balls.Insert(index, ball);
        Debug.Log("balls: " + balls.Count);

        StopMoveBalls();

        for (int i = index + 1; i < balls.Count; i++)
        {
            balls[i].GetComponent<PathBallScript>().ToggleMovementDirection();
        }

        StartMoveBalls();

        //RemoveTriples();
    }

    public static void StopMoveBalls()
    {
        for (int i = 0; i < balls.Count; i++)
        {
            balls[i].GetComponent<PathBallScript>().StopMove();
        }
    }

    public static void StartMoveBalls()
    {
        for (int i = 0; i < balls.Count; i++)
        {
            balls[i].GetComponent<PathBallScript>().StartMove();
        }
    }

    //public static void ShiftBallsColor(int index, Color otherColor)
    //{
    //    Color color = otherColor;
    //    int[] sameColorIndexes = new int[3];

    //    for (int i = index; i < balls.Count; i++)
    //    {
    //        Color currentColor = balls[i].GetComponent<MeshRenderer>().material.color;
    //        balls[i].GetComponent<MeshRenderer>().material.color = color;
    //        color = currentColor;
    //    }

    //    RemoveTriples();
    //}

    public static int FindClosestBallIndex(Vector3 position)
    {
        float minDistance = Vector3.Distance(position, balls[0].transform.position);
        int minIndex = 0;

        for (int i = 1; i < balls.Count; i++)
        {
            float distance = Vector3.Distance(position, balls[i].transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                minIndex = i;
            }
        }

        return minIndex;
    }

    private static void RemoveTriples()
    {
        int count = 1;
        Color? color = balls[0].GetComponent<MeshRenderer>().material.color;

        for (int i = 1; i < balls.Count - 2; i++)
        {
            Color currentColor = balls[i].GetComponent<MeshRenderer>().material.color;
            if (color == currentColor)
            {
                count += 1;
            }
            else
            {
                if (count >= minCountToRemove)
                {
                    DestroyRange(i - count, count);
                    balls.RemoveRange(i - count + 1, count);
                }

                count = 1;
                color = currentColor;
            }
        }
    }

    private static void DestroyRange(int index, int count)
    {
        for (int i = index; i < index + count; i++)
        {
            balls[i].GetComponent<PathBallScript>().SelfDestroy();
        }
    }
}
