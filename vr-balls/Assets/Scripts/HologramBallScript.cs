using UnityEngine;
using System.Collections;

public class HologramBallScript : MonoBehaviour
{
    private ObjectPoolerScript ballsPoolerScript;
    private Cardboard cardboard;
    private CardboardHead cardboardHead;
    private GameObject mainCamera;
    private Color[] colors = { Color.red, Color.blue, Color.green, Color.yellow };
    private MeshRenderer[] childrenMeshRenderers;
    private Color currentColor;

    void Start()
    {
        childrenMeshRenderers = GetComponentsInChildren<MeshRenderer>();
        ballsPoolerScript = GameObject.Find("BallsPooler").GetComponent<ObjectPoolerScript>();

        ChangeColor();
        cardboard = FindObjectOfType<Cardboard>();
        cardboardHead = FindObjectOfType<CardboardHead>();
        mainCamera = GameObject.Find("MainCamera");
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
        Vector3 direction = Vector3.forward;
        Vector3 position = Vector3.zero;

        if (mainCamera)
        {
            direction = mainCamera.transform.forward;
            position = mainCamera.transform.position;
        }

        if (cardboardHead)
        {
            direction = cardboardHead.Gaze.direction;
            position = cardboardHead.transform.position;
        }

        GameObject ball = ballsPoolerScript.GetPooledObject(position + direction * 1.5f, Quaternion.LookRotation(direction));

        if (ball != null)
        {
            ball.SetActive(true);
            ball.GetComponent<MeshRenderer>().material.color = currentColor;
            ball.GetComponent<BallScript>().SetTarget(direction * 1000, true);
        }
    }

    void ChangeColor()
    {
        int randomIndex = Random.Range(0, colors.Length);
        currentColor = colors[randomIndex];

        for (int i = 0; i < childrenMeshRenderers.Length; i++)
        {
            childrenMeshRenderers[i].material.color = currentColor;
        }
    }
}
