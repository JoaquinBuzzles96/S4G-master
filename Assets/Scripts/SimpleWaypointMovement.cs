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
    Transform originalPos;

    void Start()
    {
        animator = this.GetComponent<Animator>();
        waypoints = path.GetComponentsInChildren<Transform>();
        GameObject gameObjectInitPoint = new GameObject("InitPos");
        var instance = Instantiate(gameObjectInitPoint, this.transform.position, this.transform.rotation);
        originalPos = instance.transform; //si le asigno directamente la transform hara un puntero e ira actualizando la posicion, y no queremos eso
        wayToFollow = waypoints; //por si acaso se le llamase sin nada, pero siempre deberia indicarse antes de llamar
    }

    void Update()
    {
        //Debug.Log(SpecialCases.Instance.playingAnimation);

        if (canMove)
        {
            /*
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
            */

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
                    //SpecialCases.Instance.playingAnimation = false; //esto lo gestionaremos fuera, ya que llegar al destino no siemore significa que se acaba este evento
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

    public void SetPathAndPlay(Transform[] _waypoints)
    {
        waypoints = _waypoints;
        wayToFollow = waypoints;
        canMove = true;
    }

    public void ResetPosition()
    {
        ClearWaypoints();
        waypoints[0] = originalPos;
        wayToFollow = waypoints;
        canMove = true;
    }

    public void ClearWaypoints()
    {
        System.Array.Clear(waypoints, 0, waypoints.Length);
    }
}
