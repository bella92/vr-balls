using System;
using UnityEditor;
using UnityEngine;

public class PathBallScript : MonoBehaviour
{
    private float speed = 7f;
    public bool canMove = false;
    public PathMovingDirection pathMovingDirection = PathMovingDirection.Forward;
    public int index = -1;

    public int currentPointIndex = 0;
    private Color[] colors = { Color.red, Color.blue, Color.green, Color.yellow };
    private bool isInserted = false;
    private int waitCount = 0;
    private Vector3 previousTarget;
    private Vector3 target;
    private bool toBeStopped = false;
    private float traveledDistance = 0f;

    void Awake()
    {
        SetRandomColor();
    }

    void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (canMove)
        {
            float step = speed * Time.deltaTime;
            traveledDistance += speed * Time.deltaTime;

            transform.position = Vector3.MoveTowards(previousTarget, target, traveledDistance);
            float distance = Mathf.Abs(Vector3.Distance(transform.position, target));

            if (distance <= step)
            {
                ChangeCurrentPointIndex();
            }
        }
    }

    public void ChangeCurrentPointIndex()
    {
        currentPointIndex += (int)pathMovingDirection;

        if (currentPointIndex >= PathCollidersManager.GetCount())
        {
            //TODO: Game Over
            currentPointIndex -= 1;
            StopMoving();
        }
        else if (currentPointIndex < 0)
        {
            currentPointIndex = 0;
            Hide();
            StopMoving();
        }
        else
        {
            traveledDistance = 0f;
            previousTarget = transform.position;
            target = PathCollidersManager.GetColliderPosition(currentPointIndex);
        }
    }

    public PathMovingDirection GetPathMovingDirection()
    {
        return pathMovingDirection;
    }

    public void SetPathMovingDirection(PathMovingDirection direction)
    {
        if (pathMovingDirection != direction)
        {
            pathMovingDirection = direction;
            ChangeCurrentPointIndex();
        }
    }

    public void SetSpeed(float speed)
    {
        traveledDistance = 0f;
        previousTarget = transform.position;
        this.speed = speed;
    }

    public bool GetToBeStopped()
    {
        return toBeStopped;
    }

    public void SetToBeStopped(bool toBeStopped)
    {
        this.toBeStopped = toBeStopped;
    }

    public void Init(int currentPointIndex, Color color)
    {
        this.currentPointIndex = currentPointIndex;
        target = PathCollidersManager.GetColliderPosition(currentPointIndex);
        GetComponent<MeshRenderer>().material.color = color;
        SetSpeed(1f);
        Show();
    }

    public void SetWaitCount(int waitCount)
    {
        this.waitCount = waitCount;
    }

    public bool GetIsShown()
    {
        return GetComponent<MeshRenderer>().enabled;
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

    public void Hide()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }

    public void StopMoving()
    {
        canMove = false;
    }

    public void StartMoving()
    {
        canMove = true;
    }

    void OnTriggerEnter(Collider other)
    {
        string tag = other.gameObject.tag;

        if (tag == "EntrancePoint" && pathMovingDirection == PathMovingDirection.Forward)
        {
            if (index == BallsManager.GetCount() / 3)
            {
                BallsManager.SetBallsSpeed(1f);
            }

            Show();
        }
    }

    void OnTriggerExit(Collider other)
    {
        string tag = other.gameObject.tag;
        
        if (tag == "EntrancePoint" && pathMovingDirection == PathMovingDirection.Backward)
        {
            Hide();
        }
    }

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