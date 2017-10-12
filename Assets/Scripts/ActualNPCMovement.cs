using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActualNPCMovement : MonoBehaviour {
	public float speed = 1f;
	float clockTimer = 3f;
	float jumpTimer = 1f;
	public float maxJumpTimer = 1f;
	public float jumpHeight = 3f;

	//Select "WallToWallMove", "TimedMovement", "JumpingMovement", "FollowPlayer" or "AttackPlayer"
	public string wantedMoveType;
	Rigidbody2D rb2D;

	private GameObject player;
	private Vector3 playerPos;

	Character npc;
	private Animator anim;
	private Vector2 currentPosition;
	private Vector2 lastPosition;
	private bool isMoving;
	EdgeCollider2D edgeCollider;
	private SpriteRenderer rend;
	private PolygonCollider2D levelBorder;
	private BoxCollider2D groundCollider;
	private bool inAir;

	// Use this for initialization
	void Start () {
		rb2D = GetComponent<Rigidbody2D> ();
		player = GameObject.Find("PlayerCharacter");
		anim = GetComponent<Animator> ();
		edgeCollider = GetComponent<EdgeCollider2D> ();
		rend = GetComponent<SpriteRenderer> ();
		levelBorder = GameObject.FindGameObjectWithTag("LevelBorder").GetComponent<PolygonCollider2D>();
		groundCollider = transform.GetChild (2).GetComponent<BoxCollider2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		
		//choosing move type
		if (wantedMoveType.Equals("WallToWallMove")) {
			WallToWallMove();
		} else if (wantedMoveType.Equals("TimedMovement")) {
			TimedMovement();
		} else if (wantedMoveType.Equals("JumpingMovement")) {
			JumpingMovement();
		} else if (wantedMoveType.Equals("FollowPlayer")) {
			FollowPlayer();
		} else if (wantedMoveType.Equals("AttackPlayer")) {
			AttackPlayer();
		}

		//checking if the npc is in air for the animator
		if (wantedMoveType.Equals ("FollowPlayer") || wantedMoveType.Equals ("JumpingMovement") || wantedMoveType.Equals("AttackPlayer")) {
			if (!groundCollider.IsTouching(levelBorder)) {
				inAir = true;
			} else {
				inAir = false;
			}
		}
		anim.SetBool("InAir", inAir);

		//checking wether the npc is moving
		currentPosition = transform.position;
		if (currentPosition == lastPosition) {
			isMoving = false;
		} else {
			isMoving = true;
		}
		anim.SetBool("IsMoving", isMoving);
		lastPosition = currentPosition;

		//flipping the sprite when the npc changes direction
		if (player.transform.position.x < transform.position.x) {
			rend.flipX = false;
		} else {
			rend.flipX = true;
		}

	}
	//This determines what happens when our object collides with a wall
	void OnTriggerEnter2D(Collider2D col) {

		switch (col.tag) {

			case "LevelBorder":
			if (edgeCollider.IsTouching(col)) {	
					if (wantedMoveType.Equals("WallToWallMove") || wantedMoveType.Equals("TimedMovement") || wantedMoveType.Equals("JumpingMovement")) {
						if (speed > 0) {
							speed = -1f;
						} else if (speed < 0) {
							speed = 1f;
						}
					} else if (wantedMoveType.Equals("FollowPlayer") || wantedMoveType.Equals("AttackPlayer")) {
						rb2D.AddForce (new Vector2 (0f, jumpHeight * 100f));
						rb2D.gravityScale = 1;
					}
			}	
			break;

		}
	}

	//Moves left or right until it collides with a wall
	//Colliding with a wall switches movement direction
	void WallToWallMove() {
		if (speed > 0) {
			transform.Translate (speed * Time.deltaTime, 0, 0);
		} else if (speed < 0) {
			transform.Translate (speed * Time.deltaTime, 0, 0);
		}
	}

	//Basic WallToWallMove with a timer
	//Once the timer expires, movement will stop
	void TimedMovement() {
		if (clockTimer > 0) {
			WallToWallMove ();
			clockTimer -= Time.deltaTime;
		}
	}

	//Basic WallToWallMove with jumping added
	//You can set the "jumpHeight" and how often the object jumps with "maxJumpTimer" 
	void JumpingMovement() {
		if (jumpTimer < 0) {
			jumpTimer = maxJumpTimer;
			rb2D.AddForce (new Vector2 (0f, jumpHeight * 100f));
			rb2D.gravityScale = 1;
		} else {
			WallToWallMove ();
		}
		jumpTimer -= Time.deltaTime;
	}

	//npc follows the player
	void FollowPlayer() {
		playerPos = new Vector3(player.transform.position.x, transform.position.y, 0);
		transform.position = Vector3.MoveTowards(transform.position, playerPos, speed * Time.deltaTime);
	}

	void AttackPlayer() {
		FollowPlayer();

		if (transform.position.x > playerPos.x && transform.position.x - playerPos.x <= 2f/*insert attack range here*/) {
			//Attack left
		} else if (transform.position.x < playerPos.x && playerPos.x - transform.position.x <= 2f/*insert attack range here*/) {
			//Attack right
		}
	}
}
