using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleWaypointMovement : MonoBehaviour
{
    Animator animator;
    public Transform path;
    public Transform[] waypoints;
    public Transform[] waypointsExit;
    public Transform[] waypointsEnter;
    int nextWaypoint = 0;
    public float speed;
    [HideInInspector] public bool canMove;
    public Transform lastPointToLook;
    public bool exitRoom = true;
    public bool isNurse = false;
    Transform[] wayToFollow;
    public bool firstTime = true;

    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
        waypoints = path.GetComponentsInChildren<Transform>();

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(SpecialCases.Instance.playingAnimation);

        if (canMove)
        {
            if (firstTime)
            {
                if (isNurse)
                {
                    if (exitRoom)
                    {
                        wayToFollow = waypointsExit;
                    }
                    else
                    {
                        wayToFollow = waypointsEnter;
                    }
                }
                else
                {
                    wayToFollow = waypoints;
                }
                firstTime = false;
            }

            if (wayToFollow[nextWaypoint] != null)
            {
                if (Vector3.Distance(this.transform.position, wayToFollow[nextWaypoint].transform.position) > 0.1f)
                {
                    if (!animator.GetBool("Walk"))
                    {
                        animator.SetBool("Walk", true);
                    }
                   // Vector3 vectorToGo = new Vector3(waypoints[nextWaypoint].position.x, this.gameObject.transform.position.y, waypoints[nextWaypoint].position.z);
                    this.transform.position = Vector3.MoveTowards(this.transform.position, wayToFollow[nextWaypoint].position, speed * Time.deltaTime);
                    this.transform.LookAt(wayToFollow[nextWaypoint]);
                }
                else if (nextWaypoint < wayToFollow.Length-1)
                {
                    Debug.Log("cambio de "+nextWaypoint);
                    nextWaypoint++;
                }
                else
                {
                    Debug.Log("acaba");
                    SpecialCases.Instance.playingAnimation = false;
                    canMove = false;
                    nextWaypoint = 0;
                    this.transform.LookAt(lastPointToLook);
                    if (animator.GetBool("Walk"))
                    {
                        animator.SetBool("Walk", false);
                    }
                }
            }
        }
    }
}
