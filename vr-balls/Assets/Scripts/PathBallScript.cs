using System;
using UnityEditor;
using UnityEngine;

public class PathBallScript : MonoBehaviour
{
    public float speed = 0.05f;

    private int currentPointIndex = 0;
    private int nextPointIndex = 1;
    private Color[] colors = { Color.red, Color.blue, Color.green, Color.yellow };
    private bool canMove = false;

    public int index = -1;

    private Vector3? target;

    void Awake()
    {
        SetRandomColor();
    }

    void Update()
    {
        if (canMove && target != null)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, (Vector3)target, step);
            float distance = Vector3.Distance(transform.position, (Vector3)target);

            if (distance < 0.03f)
            {
                currentPointIndex += 1;

                if (currentPointIndex >= PathCollidersManager.GetCount())
                {
                    //TODO: Game Over
                    Destroy(gameObject);
                }
                else
                {
                    target = PathCollidersManager.GetColliderPosition(currentPointIndex);
                }
            }
        }
    }

    public void SetIndex(int i)
    {
        index = i;
    }

    public int GetIndex()
    {
        return index;
    }

    public void SetCurrentPointIndex(int pointIndex)
    {
        currentPointIndex = pointIndex;
    }

    public int GetCurrentPointIndex()
    {
        return currentPointIndex;
    }

    public void SetColor(Color color)
    {
        GetComponent<MeshRenderer>().material.color = color;
    }

    public Color GetColor()
    {
        return GetComponent<MeshRenderer>().material.color;
    }

    public void Show()
    {
        GetComponent<MeshRenderer>().enabled = true;
    }

    //public void Move()
    //{
    //    if (canMove)
    //    {
    //        currentPointIndex += 1;

    //        if (currentPointIndex >= PathCollidersManager.GetCount())
    //        {
    //            //TODO: Game Over
    //            Destroy(gameObject);
    //        }
    //        else
    //        {
    //            Vector3 nextPoint = PathCollidersManager.GetColliderPosition(currentPointIndex);
    //            Vector3 direction = (nextPoint - transform.position).normalized;
    //            GetComponent<Rigidbody>().velocity = direction * speed;
    //        }
    //    }
    //}

    public void StopMoving()
    {
        canMove = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public void StartMoving()
    {
        canMove = true;
        target = PathCollidersManager.GetColliderPosition(currentPointIndex);
    }

    void OnTriggerEnter(Collider other)
    {
        string tag = other.gameObject.tag;
        bool isFirstCollider = PathCollidersManager.FindIndex(other.gameObject) == 0;

        if (tag == "PathCollider")
        {
            //Move();

            if (isFirstCollider)
            {
                Show();
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