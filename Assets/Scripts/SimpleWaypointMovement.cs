using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleWaypointMovement : MonoBehaviour
{
    Animator animator;
    public Transform path;
     public Transform[] waypoints;
    int nextWaypoint = 0;
    public float speed;
    [HideInInspector] public bool canMove;
    public Transform lastPointToLook;

    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
        waypoints = path.GetComponentsInChildren<Transform>();

    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            if (waypoints[nextWaypoint] != null)
            {
                if (Vector3.Distance(this.transform.position, waypoints[nextWaypoint].transform.position) > 0.1f)
                {
                    if (!animator.GetBool("Walk"))
                    {
                        animator.SetBool("Walk", true);
                    }
                   // Vector3 vectorToGo = new Vector3(waypoints[nextWaypoint].position.x, this.gameObject.transform.position.y, waypoints[nextWaypoint].position.z);
                    this.transform.position = Vector3.MoveTowards(this.transform.position, waypoints[nextWaypoint].position, speed * Time.deltaTime);
                    this.transform.LookAt(waypoints[nextWaypoint]);
                }
                else if (nextWaypoint < waypoints.Length)
                {
                    nextWaypoint++;
                }
                else
                {
                    SpecialCases.Instance.playingAnimation = false;
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
