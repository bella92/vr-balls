using UnityEngine;
using System.Collections;

public class BallScript : MonoBehaviour
{
    private Vector3 target;
    private Vector3 fireTarget;
    private bool isPathColliderHit = false;
    private float minDistance;
    private GameObject closestPathBall;
    private bool ballHit = false;

    void Start()
    {
        minDistance = transform.localScale.x;
    }

    void Update()
    {
        float step = 12.0f * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target, step);

        if (closestPathBall != null)
        {
            float distance = Vector3.Distance(transform.position, target);

            if (distance < 0.03f)
            {
                Debug.Log("reached");

                Destroy(gameObject);

                BallsManager.StopMovingBalls();

                int index = closestPathBall.GetComponent<PathBallScript>().GetIndex();
                BallsPathScript ballsPathScript = GameObject.Find("BallsPath").GetComponent<BallsPathScript>();

                Color color = GetComponent<MeshRenderer>().material.color;
                ballsPathScript.InsertBall(index, target, color);
            }
        }
        else
        {
            SetTarget(fireTarget);
        }
    }

    public void SetFireTarget(Vector3 newFireTarget)
    {
        fireTarget = newFireTarget;
        target = newFireTarget;
    }

    public void SetTarget(Vector3 newTarget)
    {
        target = newTarget;
    }

    void OnTriggerStay(Collider other)
    {
        string tag = other.gameObject.tag;

        if (ballHit && !isPathColliderHit && tag == "PathCollider")
        {
            isPathColliderHit = true;

            SetTarget(other.gameObject.transform.position);
        }
        else if (tag == "PathBall")
        {
            ballHit = true;
            float distance = Vector3.Distance(transform.position, other.gameObject.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestPathBall = other.gameObject;
            }
        }
    }
}