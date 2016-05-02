using System;
using UnityEditor;
using UnityEngine;

public class PathBallScript : MonoBehaviour
{
    private float speed = 1f;
    public bool canMove = false;
    public PathMovingDirection pathMovingDirection = PathMovingDirection.Forward;
    public bool isShown = false;
    public int index = -1;

    public int currentPointIndex = 0;
    private Color[] colors = { Color.red, Color.blue, Color.green, Color.yellow };
    private bool isInserted = false;
    private int waitCount = 0;
    private Vector3 target;
    private bool isHit = false;

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
            currentPointIndex -= 1;
            BallsManager.StopMovingBalls();
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

    public PathMovingDirection GetPathMovingDirection()
    {
        return pathMovingDirection;
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

    public bool GetIsHit()
    {
        return isHit;
    }

    public void SetIsHit(bool isHit)
    {
        this.isHit = isHit;
    }

    public void Init(int index, int currentPointIndex, Color color)
    {
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
            int ballAheadIndex = index - 1;

            if (currentPointIndex == 0 && (ballAheadIndex < 0 || ballAheadIndex == other.gameObject.GetComponent<PathBallScript>().index))
            {
                Show();
                StartMoving();
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