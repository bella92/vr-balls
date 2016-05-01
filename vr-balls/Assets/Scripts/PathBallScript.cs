using System;
using UnityEditor;
using UnityEngine;

public class PathBallScript : MonoBehaviour
{
    public float speed = 0.05f;

    private int currentPointIndex = 0;
    private Color[] colors = { Color.red, Color.blue, Color.green, Color.yellow };
    public bool canMove = false;
    public PathMovingDirection pathMovingDirection = PathMovingDirection.Forward;
    private bool isInserted = false;
    private int waitCount = 0;
    private bool isShown = false;

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
        if (isShown && canMove)
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

    public void SetPathMovingDirection(PathMovingDirection direction)
    {
        pathMovingDirection = direction;
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

    public void SetWaitCount(int waitCount)
    {
        this.waitCount = waitCount;
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
        isShown = true;
        GetComponent<MeshRenderer>().enabled = true;
    }

    public void StopMoving()
    {
        canMove = false;
    }

    public void StartMoving()
    {
        canMove = true;
        target = PathCollidersManager.GetColliderPosition(currentPointIndex);
    }

    void OnTriggerExit(Collider other)
    {
        string tag = other.gameObject.tag;

        if (tag == "PathBall")
        {
            if (isInserted)
            {
                int otherIndex = other.gameObject.GetComponent<PathBallScript>().GetIndex();

                if (otherIndex == index - 1)
                {
                    Debug.Log("before: " + (index - 1));
                    BallsManager.StopMovingBalls(0, index);
                    waitCount -= 1;
                }

                if (otherIndex == index + 1)
                {
                    Debug.Log("after: " + (index + 1));

                    BallsManager.StopMovingBalls(index + 1);
                    waitCount -= 1;
                }

                if (waitCount == 0)
                {
                    Debug.Log("waitCount " + waitCount);
                    isInserted = false;
                    BallsManager.SetBallsPathMovingDirection(PathMovingDirection.Forward);
                    BallsManager.ChangeBallsSpeed(speed);
                    BallsManager.StartMovingBalls();
                }
            }
            else
            {
                int ballAheadIndex = index - 1;
                if (pathMovingDirection == PathMovingDirection.Backward)
                {
                    ballAheadIndex = index + 1;
                }

                if (ballAheadIndex < 0 || ballAheadIndex >= BallsManager.GetCount() ||
                    ballAheadIndex == other.gameObject.GetComponent<PathBallScript>().index)
                {
                    Show();
                    StartMoving();
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