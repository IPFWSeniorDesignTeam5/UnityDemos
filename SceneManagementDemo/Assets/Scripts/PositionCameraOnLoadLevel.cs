using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class PositionCameraOnLoadLevel : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	private void OnLevelFinishedLoading(Scene scn, LoadSceneMode mode )
	{
        GameObject cam = GameObject.FindGameObjectWithTag( "VRGameObject" );

        if( null == cam ) 
	        Debug.LogError( "Failed to position camera on load level." );
	    else
	    	cam.gameObject.transform.position = this.transform.position;
	}
}
