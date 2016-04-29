using System;
using UnityEditor;
using UnityEngine;

public class PathBallScript : MonoBehaviour
{
    public float speed = 0.05f;

    private int currentPointIndex = 0;
    private int nextPointIndex = 1;
    private Color[] colors = { Color.red, Color.blue, Color.green, Color.yellow };
    public bool canMove = false;
    private PathMovingDirection pathMovingDirection = PathMovingDirection.Forward;
    private bool isInserted = false;
    private int insertNeighboursMovedCount = 0;

    public int index = -1;

    private Vector3 target;

    void Awake()
    {
        SetRandomColor();
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        if (canMove)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target, step);
            float distance = Vector3.Distance(transform.position, target);

            if (distance < 0.03f)
            {
                ChangeCurrentPointIndex();
            }
        }
    }

    private void ChangeCurrentPointIndex()
    {
        currentPointIndex += (int)pathMovingDirection;

        if (currentPointIndex >= PathCollidersManager.GetCount())
        {
            //TODO: Game Over
            Destroy(gameObject);
        }
        else if (currentPointIndex < 0)
        {
            currentPointIndex = 0;
        }
        else
        {
            target = PathCollidersManager.GetColliderPosition(currentPointIndex);
        }
    }

    public void TogglePathMovingDirection()
    {
        pathMovingDirection = (PathMovingDirection)(-1 * (int)pathMovingDirection);
        ChangeCurrentPointIndex();
    }

    public void ChangeSpeed(float speed)
    {
        this.speed = speed;
    }

    public void SetTarget(Vector3 newTarget)
    {
        target = newTarget;
    }

    public void Init(int index, int currentPointIndex, Color color)
    {
        isInserted = true;
        this.index = index;
        this.currentPointIndex = currentPointIndex;
        GetComponent<MeshRenderer>().material.color = color;
        Show();
    }

    public int GetIndex()
    {
        return index;
    }

    public void SetIndex(int index)
    {
        this.index = index;
    }

    public int GetCurrentPointIndex()
    {
        return currentPointIndex;
    }

    public Color GetColor()
    {
        return GetComponent<MeshRenderer>().material.color;
    }

    public void Show()
    {
        GetComponent<MeshRenderer>().enabled = true;
    }

    public void StopMoving()
    {
        canMove = false;
    }

    public void StartMoving()
    {
        canMove = true;
        Debug.Log("start moving " + index);
        target = PathCollidersManager.GetColliderPosition(currentPointIndex);
    }

    void OnTriggerEnter(Collider other)
    {
        string tag = other.gameObject.tag;
        bool isFirstCollider = PathCollidersManager.FindIndex(other.gameObject) == 0;

        if (tag == "PathCollider")
        {
            if (isFirstCollider)
            {
                Show();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        string tag = other.gameObject.tag;

        if (tag == "PathBall")
        {
            if (isInserted)
            {
                if (insertNeighboursMovedCount == 2)
                {
                    //Debug.Log("insertNeighboursMovedCount");
                    //isInserted = false;
                    //insertNeighboursMovedCount = 0;
                    //BallsManager.ToggleBallsPathMovingDirection(index + 1);
                    //BallsManager.StartMovingBalls();
                }
                else
                {
                    int otherIndex = other.gameObject.GetComponent<PathBallScript>().GetIndex();

                    if (otherIndex == index - 1)
                    {
                        BallsManager.StopMovingBalls(0, index);
                        insertNeighboursMovedCount += 1;
                    }

                    if (otherIndex == index + 1)
                    {
                        BallsManager.StopMovingBalls(index + 1);
                        insertNeighboursMovedCount += 1;
                    }
                }
            }
        }
    }

    private void SetRandomColor()
    {
        int randomIndex = UnityEngine.Random.Range(0, colors.Length);
        Color color = colors[randomIndex];

        GetComponent<MeshRenderer>().material.color = color;
    }
}