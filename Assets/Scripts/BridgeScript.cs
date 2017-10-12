using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeScript : MonoBehaviour {
	GameObject originalBridge;
	GameObject fallingBridge;
	GameObject rightSide;
	GameObject leftSide;
	BoxCollider2D boxCollider;
	EdgeCollider2D edgeCollider;
	Rigidbody2D rb2D;
	public float timer;
	bool startTimer;

	// Use this for initialization
	void Start () {
		originalBridge = GameObject.Find("firstMapS/bridge");
		fallingBridge = GameObject.Find("TheRealMiddlePart");

		rightSide = GameObject.Find("RightSideOfTheBridge");
		leftSide = GameObject.Find("LeftSideOfTheBridge");
		rightSide.SetActive(false);
		leftSide.SetActive(false);

		boxCollider = fallingBridge.GetComponent<BoxCollider2D>();
		edgeCollider = fallingBridge.GetComponent<EdgeCollider2D>();
		rb2D = fallingBridge.GetComponent<Rigidbody2D>();

		startTimer = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (startTimer) {
			timer -= Time.deltaTime;

			if (timer <= 0) {
				rb2D.bodyType = RigidbodyType2D.Kinematic;
				edgeCollider.enabled = false;
			}
		}
	}

	void OnTriggerEnter2D(Collider2D col) {
		
		switch (col.tag) {
			case "Player":
				Debug.Log("Falling");
				DestroyObject(originalBridge);
				rightSide.SetActive(true);
				leftSide.SetActive(true);
				rb2D.bodyType = RigidbodyType2D.Dynamic;
				boxCollider.enabled = false;
				startTimer = true;
				break;
		}
	}
}
