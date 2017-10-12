using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderSettings : MonoBehaviour {
	bool isMoving = false;
	public Vector2 aPosition1 = new Vector2(3,3);


	// Use this for initialization
	void Start() {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (isMoving) {
			aPosition1 = new Vector2(transform.position.x + 1, transform.position.y + 1);
			transform.position = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), aPosition1, 3 * Time.deltaTime);
		}
	}


	void OnTriggerEnter2D(Collider2D col) {
		
		switch (col.tag) {
			case "Player":
				Debug.Log ("It worked");
				isMoving = true;
				break;

		case "LevelBorder":
			
			break;
			
		}
	}

	void OnTriggerExit2D(Collider2D col) {

		switch (col.tag) {
		case "Player":
			isMoving = false;
			break;
		}
	}
}
