using UnityEngine;
using System.Collections;

public class MainBallScript : MonoBehaviour
{
    private ObjectPoolerScript ballsPoolerScript;
    private Cardboard cardboard;
    private CardboardHead cardboardHead;
    private Color[] colors = { Color.red, Color.blue, Color.green, Color.yellow };

    void Start()
    {
        ballsPoolerScript = GameObject.Find("BallsPooler").GetComponent<ObjectPoolerScript>();

        ChangeColor();
        cardboard = FindObjectOfType<Cardboard>();
        cardboardHead = FindObjectOfType<CardboardHead>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || (cardboard != null && cardboard.Triggered))
        {
            Fire();
            ChangeColor();
        }
    }

    void Fire()
    {
        Vector3 position = transform.position;
        Vector3 direction = transform.forward;
        if (cardboardHead)
        {
            direction = cardboardHead.Gaze.direction;
            position = cardboardHead.transform.position + direction * 1.5f;
        }

        GameObject ball = ballsPoolerScript.GetPooledObject(transform.position, transform.rotation);

        if (ball != null)
        {
            ball.SetActive(true);
            ball.GetComponent<MeshRenderer>().material.color = GetComponent<MeshRenderer>().material.color;
            ball.GetComponent<BallScript>().SetTarget(direction * 1000, true);
        }
    }

    void ChangeColor()
    {
        int randomIndex = Random.Range(0, colors.Length);
        GetComponent<MeshRenderer>().material.color = colors[randomIndex];
    }
}