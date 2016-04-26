using System;
using UnityEditor;
using UnityEngine;

public class PathBallScript : MonoBehaviour
{
    public float defaultSpeed = 1f;
    public string movementDirection = "forward";

    private int currentPointIndex = -1;
    private int nextPointIndex = 1;
    private Color[] colors = { Color.red, Color.blue, Color.green, Color.yellow };
    private bool canMove = false;

    private int index = -1;

    void Awake()
    {
        SetRandomColor();
    }
    
    public void SetIndex(int i, bool move)
    {
        index = i;
        canMove = move;
        Move();
    }

    public void Show()
    {
        GetComponent<MeshRenderer>().enabled = true;
    }

    public void Hide()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }

    public void StopMove()
    {
        canMove = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public void StartMove()
    {
        canMove = true;
        Move();
    }

    public void ToggleMovementDirection()
    {
        if (movementDirection == "forward")
        {
            movementDirection = "backward";
        }
        else
        {
            movementDirection = "forward";
        }
    }

    public void Move()
    {
        if (canMove)
        {
            if (movementDirection == "forward")
            {
                MoveForward();
            }
            else
            {
                MoveBackward();
            }
        }
    }

    private void MoveForward()
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
            GetComponent<Rigidbody>().velocity = direction * defaultSpeed;
        }
    }

    private void MoveBackward()
    {
        currentPointIndex -= 1;

        if (currentPointIndex < 0)
        {
            Hide();
        }
        else
        {
            Vector3 nextPoint = PathCollidersManager.GetColliderPosition(currentPointIndex);
            Vector3 direction = (nextPoint - transform.position).normalized;
            GetComponent<Rigidbody>().velocity = direction * defaultSpeed;
        }
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

            //Color otherColor = other.GetComponent<MeshRenderer>().material.color;
            //Destroy(other.gameObject);

            //BallsManager.ShiftBallsColor(index, otherColor);
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

    public void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
