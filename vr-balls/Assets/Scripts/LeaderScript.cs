using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LeaderScript : MonoBehaviour
{
    //enum bondType {constant , delayed, flock}
    public enum bondType { constant, delayed } // This is an enum for the two different follow types
    public bondType Type = bondType.delayed;
    //
    public bool canBreakFollowers = true; // Set this true to allow leader to breakbonds ala "Snake"
    public bool followersCanBreak = true; //This is allows the chain to break
    public bool followersCanBond = false; // this allows followers to make bonds also (Can Be unstable if "On", and alot of collisions are going On)
    public bool followersBreakWithParent = false;
    public static bool followerBonding = true; //this is what the followers refrence to make sure they can make bonds
                                               //
    public int maxFollowers = 10; //Max number of followers that can be in the chain
                           //ArrayList followers = new ArrayList(); //Our Follower List
    public List<GameObject> followers = new List<GameObject>();
    public static bool editingList = false; // We use this to check that we are not editingList before we add or remove
                                 //User Move And  Turn
    private Vector3 moveDirection; //The Move Direction 
    public float moveSpeed = 10; // Speed Of Character 
    public float turnSpeed = 5; //Speed of Character turn
    public float followersLinearSpeed = 5;
    //The Indicator Points to last part of chain
    public static Transform last; //This always points to the last follower//We make it static so it is universal
    private Transform me; // Our tranform
    Transform indicator; // This is used to show the last follower
    public float indicatorOffset = 0.5f; // How far up is the indicator from the follower
                                //
    public float bondDistance = 2;
    public float bondDamping = 3;
    //Controller
    public float vDirection;
    public float hDirection;
    //Follower Move
    private Quaternion wantedRotation;
    private Quaternion currentRotation;
    private float wantedRotationAngle;
    private float currentRotationAngle;
    private Vector3 wantedPosition;
    /*/flock
    Vector3 flockCenter;
    Vector3 flockVelocity;
    private float minVelocity = 5;
    private float maxVelocity = 10;
    private float randomness = 1;
    */

    private bool isMoving = false; // Check if the user is moving

    Transform ReturnLast()
    { //This determines that last follower on the chain
        if (followers.Count > 0)
        {
            return followers[followers.Count - 1].transform;
        }
        else
        {
            return me;
        } //If we have more than 0 followers return the last 
                            //else the player is the last;
    }
    //User Functions 
    void Start()
    {
        ballsPath = GameObject.FindWithTag("BallsPath").GetComponent<BallsPathScript>();

        me = transform; //Cashe us
        me.tag = "Player"; // Lest Tag ourself //this helps the followers find "This object"
                           // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
        {
            GetComponent<Rigidbody>().freezeRotation = true;
        }
    }

    public BallsPathScript ballsPath;
    public Vector3[] points;

    public int currentPointIndex = 0;
    Vector3 targetPoint;

    public float speed = 0.05f;
    
    void Walk()
    {
        Debug.Log("Walk");
        // rotate towards the target
        transform.forward = Vector3.RotateTowards(transform.forward, targetPoint - transform.position, speed * Time.deltaTime, 0.0f);

        // move towards the target
        transform.position = Vector3.MoveTowards(transform.position, targetPoint, speed * Time.deltaTime);

        if (transform.position == targetPoint)
        {
            currentPointIndex++;
            //targetWayPoint = wayPointList[currentWayPoint];
        }
    }

    //bool Controller()
    //{
    //    //hDirection = Input.GetAxis("Horizontal");
    //    //vDirection = Input.GetAxis("Vertical");


    //    PathCollidersManager.GetColliderPosition(currentPointIndex);

    //    //Process Last MoveDirection 
    //    moveDirection = Vector3.zero; // Reset Move Direction
    //    moveDirection = new Vector3(0, 0, vDirection);
    //    me.Translate(moveDirection * Time.deltaTime * moveSpeed); // Move the leader

    //    me.Rotate(Vector3.up * Time.deltaTime * hDirection * 100 * turnSpeed); // This Rotates the  leader

    //    if (!Input.GetKey("escape"))
    //    { // Press Escape to free the mouse
    //      //Screen.lockCursor = true;
    //    }
    //    else { Screen.lockCursor = false; }

    //    return (Mathf.Abs(hDirection + vDirection) > 0);
    //}



    void Update()
    {
        if (points == null || points.Length == 0)
        {
            //points = ballsPath.GetPoints();

            points = new Vector3[2];
            points[0] = new Vector3(0, 4, 0);
            points[1] = new Vector3(0, 8, 0);
        }

        if (currentPointIndex < points.Length)
        {
            if (targetPoint == Vector3.zero)
            {
                targetPoint = points[currentPointIndex];
            }

            Walk();
        }

        //Gather MoveDirection and move user
        //isMoving = Controller(); //This creates our Move Direction

        //This Keeps our indicator alwasy pointed to the last
        last = ReturnLast();

        if (indicator)
        { //if we have an indicator
            indicator.position = last.position + new Vector3(0, indicatorOffset, 0);
            indicator.rotation = transform.rotation;
        }

        followerBonding = followersCanBond; //This tells the followers wheater they can bond

        Transform prevFollower;
        Transform follower;

        Vector3 theCenter = transform.position;
        Vector3 theVelocity = Vector3.zero;

        //foreach(GameObject boid in followers) {
        //theCenter   = theCenter + boid.transform.position;
        //theVelocity = theVelocity + boid.rigidbody.velocity;
        //}
        //flockCenter = theCenter/(followers.Count);	
        //flockVelocity = theVelocity/(followers.Count);

        for (int i = 0; i < followers.Count; i++)
        {
            if (i == 0)
            {
                prevFollower = this.transform;
            }
            else
            {
                prevFollower = followers[i - 1].transform;
            }
            
            follower = followers[i].transform;

            if (Type == bondType.constant)
            { // Constant follow type
                ConstantMovement(prevFollower, follower);
            }
            else if (Type == bondType.delayed)
            { // Delayed Follow Type. // This separates the position and rotation
                DelayedMovement(prevFollower, follower);
            }
            //else if(Type == bondType.flock){
            //FlockMovement(prevFollower,follower);
            //}
        }
    }

    private void ConstantMovement(Transform prevFollower, Transform follower)
    {
        // Calculate the current rotation angles
        wantedRotationAngle = prevFollower.eulerAngles.y;
        currentRotationAngle = follower.transform.eulerAngles.y;

        // Damp the rotation around the y-axis
        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, bondDamping * Time.deltaTime);
        // Convert the angle into a rotation
        currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);
        // Set the position of the camera on the x-z plane to:
        // bondDistance meters behind the prevFollower

        follower.transform.position = prevFollower.position;
        //follower.rigidbody.MovePosition((prevFollower.position - currentRotation * Vector3.forward * bondDistance) * Time.deltaTime * followersLinearSpeed);
        follower.transform.position -= currentRotation * Vector3.forward * bondDistance;
        //Always look at the prevFollower
        follower.transform.LookAt(prevFollower);
    }

    private void DelayedMovement(Transform prevFollower, Transform follower)
    {
        wantedPosition = prevFollower.TransformPoint(0, 0, -bondDistance);
        follower.transform.position = Vector3.Lerp(follower.transform.position, wantedPosition, Time.deltaTime * bondDamping);

        wantedRotation = Quaternion.LookRotation(prevFollower.position - follower.transform.position, prevFollower.up);
        follower.transform.rotation = Quaternion.Slerp(follower.transform.rotation, wantedRotation, Time.deltaTime * bondDamping);
    }
    /*
    private void  FlockMovement ( Transform prevFollower ,  Transform follower  ){
        FIXME_VAR_TYPE randomize= Vector3((Random.value *2) -1, (Random.value * 2) -1, (Random.value * 2) -1);

        randomize.Normalize();
        randomize *= randomness;

        Vector3 follow = transform.position;

        flockCenter = transform.position - follower.position;
        flockVelocity = flockVelocity - follower.rigidbody.velocity;
        follow = follow - follower.position;

        Vector3 result = flockCenter + flockVelocity + follow + randomize;


        follower.rigidbody.velocity = Vector3.Lerp(follower.rigidbody.velocity,follower.rigidbody.velocity + result,Time.deltaTime);

        // enforce minimum and maximum speeds for the boids
        FIXME_VAR_TYPE speed= follower.rigidbody.velocity.magnitude;
        if (speed > maxVelocity) {
            follower.rigidbody.velocity = follower.rigidbody.velocity.normalized * maxVelocity;
        } else if (speed < minVelocity) {
            follower.rigidbody.velocity = follower.rigidbody.velocity.normalized * minVelocity;
        }

        wantedRotation = Quaternion.LookRotation(prevFollower.position - follower.transform.position, transform.up);
        follower.transform.rotation = Quaternion.Slerp (follower.transform.rotation, wantedRotation, Time.deltaTime * bondDamping);

    }
    */
    // This Functions handle making new bonds whether the user hits the object or the object hits any part of the body
    void OnCollisionEnter(Collision obj)
    { // This if for when the leader hits an Object
        if (obj.collider.transform.tag == "Follower")
        { // We have found a follower obj
          //Debug.Log("We found" + obj.transform.name);
            if (!followers.Contains(obj.gameObject))
            {
                Debug.Log("Collided With : " + obj.gameObject.name + " : " + Time.time);
                if (followers.Count < maxFollowers) BondThis(obj.gameObject.transform); //We call the bond Functions	
            }
            else
            { //We hit a follower already on the chain
                if (canBreakFollowers)
                {
                    RemoveMe(obj.gameObject.GetComponent<FollowerScript>().bondId);
                } //Lets the Bond Id then remove
            }
        }
    }
    public void BondThis(Transform obj)
    { //This function is called from the follower who hits a random Object
        if (followers.Contains(obj.gameObject) || followers.Count >= maxFollowers)
        {
            return;
        }
        if (!editingList)
        {
            editingList = true; // Set this to return true so we cant add while editingList
            obj.gameObject.GetComponent<FollowerScript>().MakeBond(followers.Count);
            followers.Add(obj.gameObject); //Now add the object
            editingList = false; //Done Editing
        }
    }
    //This section deals with editingList followers from chain
    public void RemoveMe(int id)
    { // This function is the "MASTER BREAKER" breaking begins here
        if (!followersCanBreak) { return; } // Only Run if followers can Break Off

        if (editingList) { return; } // We are already removing something
                                     //we use a for loop to remove every follower starting from the follower that called this
        editingList = true; // Set this to return true so we cant add while editingList

        if (followersBreakWithParent)
        {
            for (int i = followers.Count - 1; i > 0; i--)
            {
                FollowerScript follower = followers[i].GetComponent<FollowerScript>();
                follower.RemoveMe();
                followers.RemoveAt(i);
                if (follower.bondId == id)
                {
                    break;
                }
            }
        }
        else
        {
            for (int j = followers.Count - 1; j > 0; j--)
            {
                FollowerScript follower = followers[j].GetComponent<FollowerScript>();
                if (follower.bondId == id)
                {
                    follower.RemoveMe();
                    followers.RemoveAt(j);
                    break;
                }
            }
        }
        editingList = false;
    }
    void RemoveAll()
    {
        editingList = true;
        for (int i = followers.Count - 1; i > 0; i--)
        {
            followers[i].GetComponent<FollowerScript>().RemoveMe();
            followers.RemoveAt(i);
        }
        editingList = false;
    }

    void MustBreak()
    { //This is called by a follower that is constantly colliding with another one causing chain to jitter
        followersCanBreak = true;
    }
}
