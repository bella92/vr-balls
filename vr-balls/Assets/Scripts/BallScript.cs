using UnityEngine;
using System.Collections;

public class BallScript : MonoBehaviour
{
    private bool passed = false;

    public GameObject pathBallPrefab;

    void OnTriggerEnter(Collider other)
    {
        string tag = other.gameObject.tag;

        if (!passed && tag == "PathCollider")
        {
            passed = true;

            Vector3 position = other.gameObject.transform.position;
            Destroy(gameObject);
            InsertToBalls(position);
        }
    }

    private void InsertToBalls(Vector3 position)
    {
        Debug.Log("InsertToBalls");
        int closestIndex = BallsManager.FindClosestBallIndex(position);
        GameObject ball = (GameObject)Instantiate(pathBallPrefab, position, Quaternion.identity);

        BallsManager.InsertBall(closestIndex, ball);
        ball.GetComponent<PathBallScript>().SetIndex(closestIndex, false);
        ball.GetComponent<MeshRenderer>().material.color = GetComponent<MeshRenderer>().material.color;
    }
}
