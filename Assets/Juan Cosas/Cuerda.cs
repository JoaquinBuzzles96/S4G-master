using System.Collections.Generic;
using UnityEngine;

public class Cuerda : MonoBehaviour
{
    public Transform player;
    public Transform originRender;

    public LineRenderer rope;
    public LayerMask collMask;

    public List<Vector3> ropePositions { get; set; } = new List<Vector3>();

    //Esto hace que el punto de inicio del vector sea el 0,0,0
    //private void Awake() => AddPosToRope(Vector3.zero);

    //Esto hace que el punto de inicio del vector
    private void Awake()
    {
        AddPosToRope(originRender.position);
    }

    private void Update()
    {
        UpdateRopePositions();
        LastSegmentGoToPlayerPos();

        DetectCollisionEnter();
        if (ropePositions.Count > 2) DetectCollisionExits();
    }

    private void DetectCollisionEnter()
    {
        RaycastHit hit;
        if (Physics.Linecast(player.position, rope.GetPosition(ropePositions.Count - 2), out hit, collMask))
        {
            ropePositions.RemoveAt(ropePositions.Count - 1);
            AddPosToRope(hit.point);
        }
    }

    private void DetectCollisionExits()
    {
        RaycastHit hit;
        if (!Physics.Linecast(player.position, rope.GetPosition(ropePositions.Count - 3), out hit, collMask))
        {
            ropePositions.RemoveAt(ropePositions.Count - 2);
        }
    }

    private void AddPosToRope(Vector3 _pos)
    {
        ropePositions.Add(_pos);
        ropePositions.Add(player.position); //Always the last pos must be the player
    }

    private void UpdateRopePositions()
    {
        rope.positionCount = ropePositions.Count;
        rope.SetPositions(ropePositions.ToArray());
    }

    private void LastSegmentGoToPlayerPos() => rope.SetPosition(rope.positionCount - 1, player.position);
}
