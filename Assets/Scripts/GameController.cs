using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
		//framerate controller
		Application.targetFrameRate = -1;

		// number of vsyncs per frame. Choose 0, 1 or 2
		QualitySettings.vSyncCount = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
