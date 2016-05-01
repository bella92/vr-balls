using UnityEngine;
using System.Collections;
using System;

public class BallScript : MonoBehaviour
{
    private Vector3 target;
    private Vector3 fireTarget;
    private bool isPathColliderHit = false;
    private bool ballHit = false;

    void Update()
    {
        float step = 12.0f * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target, step);

        if (ballHit)
        {
            float distance = Vector3.Distance(transform.position, target);

            if (distance < 0.03f)
            {
                Debug.Log("reached");

                Destroy(gameObject);

                BallsManager.StopMovingBalls();

                float diameter = transform.localScale.x;
                int index = BallsManager.FindInsertIndex(target, diameter);

                Debug.Log("insert index: " + index);

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
        }
    }
}