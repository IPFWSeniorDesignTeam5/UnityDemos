using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSceneManagement;

public class MapSceneManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Random.InitState (0);
		VRSceneManager.LoadSceneState( this.GetComponent<MapOverviewState>() );
		DontDestroyOnLoad( gameObject );
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
