using System.Collections.Generic;
using UnityEngine;

public static class PathCollidersManager
{
    private static List<GameObject> pathColliders = new List<GameObject>();

    public static void AddCollider(GameObject collider)
    {
        pathColliders.Add(collider);
    }

    public static Vector3 GetColliderPosition(int index)
    {
        return pathColliders[index].transform.position;
    }

    public static int GetCount()
    {
        return pathColliders.Count;
    }

    public static GameObject GetPathColliderAtIndex(int index)
    {
        if (index >= 0 && index < pathColliders.Count)
        {
            return pathColliders[index];
        }

        return null;
    }

    public static int FindIndex(GameObject collider)
    {
        int index = -1;

        for (int i = 0; i < pathColliders.Count; i++)
        {
            if (collider.transform.position == pathColliders[i].transform.position)
            {
                index = i;
                break;
            }
        }

        return index;
    }
}