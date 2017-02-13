using UnityEngine;
using System.Collections;
using VRSceneManagement;

public class CreditsState : MonoBehaviour, SceneState  {

	public Light mainLight = null;

	bool switchLight = false;
	float elapsedTime = 0f;

	#region SceneState implementation

	public void Load ()
	{
		switchLight = true;
	}

	public void Unload ()
	{
		throw new System.NotImplementedException ();
	}

	#endregion

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		if (switchLight) {
			elapsedTime += Time.deltaTime;

			if (elapsedTime > 3f) {
				mainLight.color = Color.blue;
				switchLight = false;
			}
		}
	}
}
