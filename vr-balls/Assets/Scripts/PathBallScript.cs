using System;
using UnityEditor;
using UnityEngine;

public class PathBallScript : MonoBehaviour
{
    public float speed = 0.05f;

    private int currentPointIndex = -1;
    private int nextPointIndex = 1;
    private Color[] colors = { Color.red, Color.blue, Color.green, Color.yellow };
    private bool allowedToMove = false;

    private int index = -1;

    void Awake()
    {
        SetRandomColor();
    }
    
    public void SetIndex(int i)
    {
        index = i;
        Move();
    }

    public void Show()
    {
        GetComponent<MeshRenderer>().enabled = true;
    }

    public void AllowToMove()
    {
        Debug.Log("AllowToMove index: " + index);
        
        allowedToMove = true;
    }

    //private void Move()
    //{
    //    if (allowedToMove)
    //    {
            
    //        Vector3 nextPoint = PathCollidersManager.GetColliderPosition(nextPointIndex);

    //        float distance = Vector3.Distance(transform.position, nextPoint);
    //        Debug.Log("distance: " + distance);

    //        if (distance > 0.01f)
    //        {
    //            Vector3 direction = (nextPoint - transform.position).normalized;
    //            GetComponent<Rigidbody>().MovePosition(transform.position + direction * Time.deltaTime);
    //        }
    //        else
    //        {
    //            Debug.Log(distance == 0);
    //            nextPointIndex += 1;

    //            if (nextPointIndex >= PathCollidersManager.GetCount())
    //            {
    //                //TODO: Game Over
    //                Destroy(gameObject);
    //                return;
    //            }
    //        }
    //    }
    //}

    //public void IncrementNextPoint()
    //{
    //    currentPointIndex += 1;

    //    if (currentPointIndex >= PathCollidersManager.GetCount())
    //    {
    //        //TODO: Game Over
    //        Destroy(gameObject);
    //    }
    //    else
    //    {
    //        Debug.Log(index);
    //        Debug.Log(currentPointIndex);

    //        //Debug.Log("nextPoint " + nextPoint.x + ", " + nextPoint.y + ", " + nextPoint.z);


    //        //Vector3 direction = (nextPoint - transform.position).normalized;
    //        //GetComponent<Rigidbody>().velocity = direction * speed;
    //    }
    //}

    public void Move()
    {
        currentPointIndex += 1;

        if (currentPointIndex >= PathCollidersManager.GetCount())
        {
            //TODO: Game Over
            Destroy(gameObject);
        }
        else
        {
            Vector3 nextPoint = PathCollidersManager.GetColliderPosition(currentPointIndex);
            Vector3 direction = (nextPoint - transform.position).normalized;
            GetComponent<Rigidbody>().velocity = direction * speed;
        }
    }

    public void ForbidToMove()
    {
        allowedToMove = false;
    }

    void OnTriggerEnter(Collider other)
    {
        string tag = other.gameObject.tag;
        bool isFirstCollider = PathCollidersManager.FindIndex(other.gameObject) == 0;

        if (tag == "PathCollider")
        {
            Move();

            if (isFirstCollider)
            {
                Debug.Log("first");
                Show();
            }
        }
        else if (tag == "Ball")
        {
            //GameObject[] pathBalls = GameObject.FindGameObjectsWithTag("PathBall");
            //for (int i = 0; i < pathBalls.Length; i++)
            //{
            //    pathBalls[i].GetComponent<PathBallScript>().Stop();
            //}

            Color otherColor = other.GetComponent<MeshRenderer>().material.color;
            Destroy(other.gameObject);

            BallsManager.ShiftBallsColor(index, otherColor);
        }
    }

    //void OnTriggerStay(Collider other)
    //{
    //    string tag = other.gameObject.tag;
    //    bool areAtSamePoint = transform.position == other.transform.position;
        
    //    if (tag == "PathCollider" && areAtSamePoint)
    //    {
    //        IncrementNextPoint();
    //    }
    //}

    //void OnTriggerExit(Collider other)
    //{
    //    string tag = other.gameObject.tag;
    //    bool isFirstCollider = PathCollidersManager.FindIndex(other.gameObject) == 0;

    //    if (tag == "PathCollider" && isFirstCollider)
    //    {
    //        Show();
    //        Debug.Log("distance " + Vector3.Distance(transform.position, other.gameObject.transform.position));
    //        BallsManager.AllowBallToMove(index + 1);
    //    }
    //}

    private void SetRandomColor()
    {
        int randomIndex = UnityEngine.Random.Range(0, colors.Length);
        Color color = colors[randomIndex];

        GetComponent<MeshRenderer>().material.color = color;
    }
}
