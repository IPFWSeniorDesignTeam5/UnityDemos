using UnityEngine;
using System.Collections;
using VRSceneManagement;

[RequireComponent(typeof(MainMenuState))]
[RequireComponent(typeof(CreditsState))]
public class ChangeSceneAfterSeconds : MonoBehaviour {

	float elapsedTime = 0;
	bool loaded = false;

	// Use this for initialization
	void Start () {
		VRSceneManager.LoadSceneState ( this.GetComponent<MainMenuState>() );
	}
	
	// Update is called once per frame
	void Update () {
		elapsedTime += Time.deltaTime;	

		if (elapsedTime >= 3f && !loaded) {
			VRSceneManager.LoadSceneState (this.GetComponent<CreditsState> ());
			loaded = true;
		}
	}
}
