using UnityEngine;
using System.Collections;
using System;

public class TestScript1 : MonoBehaviour
{
    public Transform destination;
    public Transform secondDestination;
    public float speed = 1f;
    public float minDistance = 0.03f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (Vector3.Distance(destination.position, transform.position) < minDistance)
        {
            ChangeDestination();
        }
        else
        {
            transform.LookAt(destination);
            //Vector3 directon = (destination.position - transform.position).normalized;
            //rb.MovePosition(transform.position + directon * Time.deltaTime * speed);

            Vector3.MoveTowards(transform.position, destination.position, 2f);
        }
    }

    public void ChangeDestination()
    {
        destination = secondDestination;
    }
}
