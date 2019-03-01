using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour {        //abstract makes code reusable/inheritible

    public float moveTime = 0.1f;
    public LayerMask blockingLayer;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private float inverseMoveTime;

	// Use this for initialization
	protected virtual void Start ()     //can be overwritten by there inheritible classes
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime;
    }

    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)       //out: needs to be referenced
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        boxCollider.enabled = false;                                //disable boxcollider to not hit our own collider in the next code line
        hit = Physics2D.Linecast(start, end, blockingLayer);        //cast a line from start to end and checking for a hit with blockinglayer
        boxCollider.enabled = true;

        if(hit.transform == null)                                   //if checked space is empty(no raycast hit) start to move into space via smoothmovement()
        {
            StartCoroutine(SmoothMovement(end));
            return true;                                               
        }

        return false;
    }

    protected IEnumerator SmoothMovement (Vector3 end)       
    {
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;   //calculating distance between currentPos and endPos// sqrMagnitude is faster than sqrt calculation of a vector
        
        while (sqrRemainingDistance > float.Epsilon)                            //epsilon: smallest float value, not zero
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);    //newPos = a point that moves to target point in a straight line at given speed/distance to move per call (inverseMoveTime * Time.deltaTime)
            rb2D.MovePosition(newPosition);                                                                     //move rb2d to newPos
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;                                     //recalculate remaining distance after we moved 
            yield return null;                                                                                  //wait until the next frame and then continue execution
        }
    }

    protected virtual void AttemptMove <T> (int xDir, int yDir)     //marks missing/incomplete part/implimentation, that will be overwritten by inheriting classes 
        where T : Component
    {
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);                   //set canMove to true if Move() was succesful(true), otherwise to false

        if (hit.transform == null)                                  //if nothing was hit by Linecast in Move() return, otherwise....
            return;

        T hitComponent = hit.transform.GetComponent<T>();           //...get a reference of what was hit 

        if (!canMove && hitComponent != null)                       //if canMove is false & hitComponent is not equal to null
            onCantMove(hitComponent);                               //call onCantMove function with hitcomponent parameter
                                                    //so we can use it for player and enemy, to do stuff accordingly to what they encounter
    }    

    protected abstract void onCantMove<T>(T component)      //marks missing/incomplete part/implimentation, that will be overwritten by inheriting classes 
        where T : Component;    
}
