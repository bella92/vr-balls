using UnityEngine;
using System.Collections;

public class MainBallScript : MonoBehaviour
{
    public GameObject ballPrefab;

    private Cardboard cardboard;
    private CardboardHead cardboardHead;
    private Color[] colors = { Color.red, Color.blue, Color.green, Color.yellow };

    void Start()
    {
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
        GameObject ball = (GameObject)Instantiate(ballPrefab, transform.position, transform.rotation);
        ball.GetComponent<MeshRenderer>().material.color = GetComponent<MeshRenderer>().material.color;

        Vector3 direction = transform.forward;
        if (cardboardHead)
        {
            direction = cardboardHead.Gaze.direction;
        }
        
        ball.GetComponent<Rigidbody>().velocity = direction * 6.0f;

        Destroy(ball, 1);
    }
    
    void ChangeColor()
    {
        int randomIndex = Random.Range(0, colors.Length);
        GetComponent<MeshRenderer>().material.color = colors[randomIndex];
    }
}
