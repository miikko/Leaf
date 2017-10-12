using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour {
	Collider2D playerCollider;
	Character player;
	GameObject playerObject;


	// Use this for initialization
	void Start () {
		playerObject = GameObject.Find("PlayerCharacter");
		playerCollider = playerObject.GetComponent<BoxCollider2D>();
		player = playerObject.GetComponent<Character>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
