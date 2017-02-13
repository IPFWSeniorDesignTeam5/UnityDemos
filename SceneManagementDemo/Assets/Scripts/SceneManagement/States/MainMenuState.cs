using UnityEngine;
using System.Collections;
using VRSceneManagement;

public class MainMenuState : MonoBehaviour, SceneState {

	private int m_iSceneId;

	bool doLoad = false, doUnload = false;

	public Light mainLight = null;

	public void Load( )
	{
		doLoad = true;
		doUnload = false;
		mainLight.color = new Color (255, 0, 0);
	}

	public void Unload()
	{
		doLoad = false;
		doUnload = true;
		mainLight.color = new Color (255, 255, 255);
	}

	void Update()
	{
		if (doLoad) {

		} else if (doUnload) {
			
		}
	}
}
