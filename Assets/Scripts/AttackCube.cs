using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCube : MonoBehaviour {
    Player playercs;
    //määrittää mitä seurataan
    public Transform player;
    public Vector2 lastPosition;
    public float attackDistance = 1;
    public float attackTime;
    public float attackCooldown;
    public Rigidbody2D rb;
    public Character playerChar;
    public Player pc;
    public GameObject parentComponent;
    public GameObject playerObj;
    public Controller2D controller;
    private BoxCollider2D playerCollider;
    private BoxCollider2D enemyCollider;
    private Vector2 newPos;
    private float direction;
    private float attackTimer;
    private float cooldownTimer;
    private int counter;

    private bool attacking = false;
    private bool cooldown = false;
    private BoxCollider2D attackBox;
	private ActualNPCMovement aNPCMove;
	public bool inRange;


    private bool animationPlayed = false;

    private void Start()
    {
        parentComponent = transform.parent.gameObject;
        attackBox = GetComponent<BoxCollider2D>();
        controller = parentComponent.GetComponent<Controller2D>();
        playerObj = GameObject.Find("PlayerCharacter");
        pc = parentComponent.GetComponent<Player>();
        playerChar = parentComponent.GetComponent<Character>();
        rb = parentComponent.GetComponent<Rigidbody2D>();
        attackBox.enabled = false;
        lastPosition = parentComponent.transform.position;
        playerCollider = playerObj.GetComponent<BoxCollider2D>();
		aNPCMove = parentComponent.GetComponent<ActualNPCMovement> ();

        if (!pc)
        {
            counter = 0;
            enemyCollider = parentComponent.transform.Find("HitDetection").GetComponent<BoxCollider2D>();
        }
    }

    //sends information to others
    public bool IsAttacking()
    {
        return attacking;
    }
    public void Attack ()
    {
        if (!cooldown && !attacking)
        {
            //Debug.Log("attacked ");
            attackBox.enabled = true;
            attacking = true;

        }
	}

    private void Update()
    {
        //if NPC
        if (!pc)
        {
            counter++;
            //Debug.Log(enemyCollider.IsTouching(playerCollider));
            // has moved
			if (lastPosition.x != parentComponent.transform.position.x && !enemyCollider.IsTouching(playerCollider) && counter > 105) {
				//get player position
				Vector3 getDirection = transform.position - playerObj.transform.position;

				//assign direction to player location
				if (getDirection.x > 0) {
					direction = -1f;
				} else if (getDirection.x < 0) {
					direction = 1f;
				}

				counter = 0;
				lastPosition = parentComponent.transform.position;
				inRange = false;
			} else if (enemyCollider.IsTouching(playerCollider) && aNPCMove.wantedMoveType.Equals("AttackPlayer")) {
				Attack();
				if (attackBox.IsTouching(playerCollider)) {
					inRange = true;
				}
				//StartCoroutine(doDamage ());
			}
        }

        if (pc) // if Player
        {
            direction = controller.collisions.faceDir;
        }
        
        newPos.x = direction*attackDistance + transform.parent.position.x;
        newPos.y = transform.parent.position.y;
        transform.position = newPos;
    }

    private void FixedUpdate()
    {

        //animation caller
		if (!animationPlayed && attacking) {
			animationPlayed = true;
			Debug.Log("Attack animation should play");
			playerChar.Attack();              
		} 

        //timerlogic
        if (attacking)
        {
            attackTimer += 1.0F * Time.deltaTime;
        }


        if (attackTimer >= attackTime)
        {
            
            attackBox.enabled = false;
            attackTimer = 0;
            cooldown = true;
        }

        if (cooldown)
        {
            attacking = false;
            cooldownTimer += 1.0F * Time.deltaTime;
        }

        if (cooldownTimer >= attackCooldown)
        {
            cooldown = false;
            cooldownTimer = 0;
            animationPlayed = false;
        }
    }

	public bool isInRange() {
		return inRange;
	}
}
