using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderingScript : MonoBehaviour {
	public double timer = 0;
	public float tmrSpeed = 20;
	public float movSpeed = 1;
	public int tmrState = 0;
	public int tmrMax = 1;
	public int movDir = 0;

	Vector2 vTwo = new Vector2();

	enum moveStyle { eightDir, randDir, sleep };
	moveStyle styleMove;


	void Start() {
		styleMove = moveStyle.eightDir;
	}
	//=======================================================================// Up D
	void Update() {
		//----//
		if(tmrState == 0) {
			//----//
			timer += 0.1 * Time.deltaTime * tmrSpeed;
			if(timer >= tmrMax) {
				timer = 0.0;
				if(styleMove == moveStyle.eightDir) {
					movDir = Random.Range(0, 8);
					GetPreVector( movDir ); // 8 Dir
				}
				else if(styleMove == moveStyle.randDir) {
					var ranX = Random.Range(-1.0f, 1.0f); 
					var ranY = Random.Range(-1.0f, 1.0f);
					vTwo.x = ranX; vTwo.y = ranY; // ( Vip )
				}
				else if(styleMove == moveStyle.sleep) {
					vTwo.x = 0; vTwo.y = 0; // ( Vip )
				}
			}
			//----//
			RandomDirMotor( vTwo );
			//----//
		}
		//----//
	}
	//=======================================================================// 8 Dir
	void GetPreVector( int mDir ) {
		//----//
		Vector2 nV = new Vector2(0,0);
		//----//
		if(mDir == 0) { nV.x = 0f; nV.y = 1.0f; } // Up
		if(mDir == 1) { nV.x = 0f; nV.y = -1.0f; } // Down
		if(mDir == 2) { nV.x = -1.0f; nV.y = 0f; } // Left
		if(mDir == 3) { nV.x = 1.0f; nV.y = 0f; } // Right
		if(mDir == 4) { nV.x = 1.0f; nV.y = 1.0f; } // UpRight
		if(mDir == 5) { nV.x = 1.0f; nV.y = -1.0f; } // DownRight
		if(mDir == 6) { nV.x = -1.0f; nV.y = -1.0f; } // DownLeft
		if(mDir == 7) { nV.x = -1.0f; nV.y = 1.0f; } // UpLeft
		//----//
		vTwo.x = nV.x; vTwo.y = nV.y; // ( Vip )
		//----//
	}
	//=======================================================================// Mov
	void RandomDirMotor( Vector2 vTwo ) {
		//----//
		var v2X = vTwo.x * Time.deltaTime * movSpeed;
		var v2Y = vTwo.y * Time.deltaTime * movSpeed;
		transform.Translate( v2X, v2Y, 0 ); // Movement
		//----//
	}
		
}
