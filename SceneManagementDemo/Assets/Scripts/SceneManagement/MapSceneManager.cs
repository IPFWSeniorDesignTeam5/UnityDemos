using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSceneManagement;

public class MapSceneManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		VRSceneManager.LoadSceneState( this.GetComponent<MapOverviewState>() );
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
