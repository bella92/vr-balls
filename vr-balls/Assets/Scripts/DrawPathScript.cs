using UnityEngine;
using System.Collections;
using System;

public class DrawPathScript : MonoBehaviour
{
    public GameObject cylinderRef;

    private iTweenPath path;
    private LineRenderer lineRenderer;

    void Start()
    {
        path = GetComponent<iTweenPath>();

        Vector3[] nodes = iTweenPath.GetPath(path.pathName);
        Vector3[] vector3s = PathControlPointGenerator(nodes);

        //DrawLine(vector3s, nodes.Length);
        //DrawTube(vector3s, nodes.Length);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            GameObject[] cylinders = GameObject.FindGameObjectsWithTag("Cylinder");
            for (int i = 0; i < cylinders.Length; i++)
            {
                Destroy(cylinders[i]);
            }

            Vector3[] nodes = iTweenPath.GetPath(path.pathName);
            Vector3[] vector3s = PathControlPointGenerator(nodes);

            DrawTube(vector3s, nodes.Length);
        }
    }

    private void DrawLine(Vector3[] vector3s, int nodesLength)
    {
        lineRenderer = GetComponent<LineRenderer>();

        Vector3 prevPt = Interp(vector3s, 0);
        lineRenderer.SetPosition(0, prevPt);
        lineRenderer.SetWidth(0.1f, 0.1f);
        lineRenderer.SetColors(Color.red, Color.red);

        int smoothAmount = nodesLength * 20;
        lineRenderer.SetVertexCount(smoothAmount + 1);

        for (int i = 1; i <= smoothAmount; i++)
        {
            float pm = (float)i / smoothAmount;
            Vector3 currPt = Interp(vector3s, pm);

            lineRenderer.SetPosition(i, currPt);

            prevPt = currPt;
        }
    }

    private void DrawTube(Vector3[] vector3s, int nodesLength)
    {
        Vector3 prevPt = Interp(vector3s, 0);
        
        int smoothAmount = nodesLength * 20;
        
        for (int i = 1; i <= smoothAmount; i++)
        {
            float pm = (float)i / smoothAmount;
            Vector3 currPt = Interp(vector3s, pm);

            DrawCylinderBetweenTwoPoints(prevPt, currPt);

            prevPt = currPt;
        }
    }

    private void DrawCylinderBetweenTwoPoints(Vector3 start, Vector3 end)
    {
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.tag = "Cylinder";

        Vector3 localScale = cylinder.transform.localScale;
        float halfDistance = Vector3.Distance(start, end) / 2;
        cylinder.transform.localScale = new Vector3(0.2f, halfDistance * 1.2f, 0.2f);

        Vector3 halfDistanceVector = (end - start).normalized * halfDistance;

        cylinder.transform.position = start + halfDistanceVector;
        cylinder.transform.LookAt(start + 2 * halfDistanceVector);
        cylinder.transform.Rotate(new Vector3(1, 0, 0), 90.0f);
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
