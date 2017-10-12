using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// class abstract because we will be inheriting from this class
public abstract class NPCMovement : MonoBehaviour {

	//time that it takes our object to move in seconds
	public float moveTime = 0.1f;

	//this is the layer where we check collision as we are moving. All objects that have collision should be in this layer
	public LayerMask blockingLayer;

	private BoxCollider2D boxCollider;
	private Rigidbody2D rb2D;

	//this is to make our movement calculations more efficient
	private float inverseMoveTime;

	//this can be overridden by the inheriting classes
	protected virtual void Start () {
		
		//getting a component reference and storing it
		boxCollider = GetComponent<BoxCollider2D>();

		//getting a component reference and storing it
		rb2D = GetComponent<Rigidbody2D>();

		// this is because we can multiply instead of divide in our calculations
		inverseMoveTime = 1f / moveTime;
	}

	//returns bool and RaycastHit2D
	protected bool Move (int xDir, int yDir, out RaycastHit2D hit) {

		//this is the current position
		Vector2 start = transform.position;

		//here we calculate the end-position
		Vector2 end = start + new Vector2(xDir, yDir);

		//we disable the collider so it wont our raycast rays wont hit our own collider
		boxCollider.enabled = false;

		//here we draw a line to check if there is collision on "blockinglayer"
		hit = Physics2D.Linecast(start, end, blockingLayer);

		//now we can re-enable our collider
		boxCollider.enabled = true;

		//here we check if there were any collisions in the drawn line
		if (hit.transform == null) {
			
			//this option happens if there were no collisions
			//now we complete the movement
			StartCoroutine(SmoothMovement(end));
			return true;

		} else {

			//This option is chosen if there were collisions
			//no movement happens
			return false;
		}
	}

	//here the "T" is replaced by the type of component we are going to interract with
	protected virtual void AttemptMove <T> (int xDir, int yDir) 

			//this specifies that "T" means "Component"
			where T : Component
	{
		RaycastHit2D hit;

		//this is true if the movement was succesfull and false if not
		bool canMove = Move(xDir, yDir, out hit);

		//this means that if there were no collisions in the "Move" method, we are not going to execute the following code
		if (hit.transform == null) {
			return;
		}

		//here we are going to attach the object we collided with and the object that attempted to move
		T hitComponent = hit.transform.GetComponent<T>();

		//this happens if the the object we collided with is something that we can interact with 
		if (!canMove && hitComponent != null) {
			
			//this makes our object possibly interact with the collided object
			OnCantMove(hitComponent);
		}
		
	}

	//with this we will move units from one place to another. parameter "end" is the place where we move to
	protected IEnumerator SmoothMovement (Vector3 end) {
		
		//calculating the remaining distance that we have to move. we use "sqr" because its more efficient
		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

		//here we check if we have reached the destination. "Epslilon" is the closest number to zero
		while (sqrRemainingDistance > float.Epsilon) {

			//finds a new position that is propotionally closer to the end, based on the "moveTime"
			Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);

			//here we move to the new position that we found
			rb2D.MovePosition(newPosition);

			//here set our new position as the current position 
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;

			//here we wait a frame before the while-loop runs again
			yield return null;
		}

	}

	// here the "abstract" tells that this is incomplete / subject to change
	// we can later determine what happens
	protected abstract void OnCantMove <T>(T component) 
		where T : Component;

}
