using UnityEngine;
using System.Collections;
using VRSceneManagement;

public class ChangeSceneAfterFade : MonoBehaviour {

    public string NewScene = "";

    private OVRScreenFade fader = null;

    void Awake()
    {
    	fader = GameObject.FindGameObjectWithTag( "VRGameObject" ).GetComponentInChildren<OVRScreenFade>();
		ScreenFader.SubscribeToFades(FadeEvent);
    }

	void FadeEvent( OVRScreenFade.OVRScreenFadeMode e )
	{
		if( e == OVRScreenFade.OVRScreenFadeMode.In )
			fader.Fade( OVRScreenFade.OVRScreenFadeMode.Out );
		else
		{
			ScreenFader.UnsubscribeToFades(FadeEvent);
			VRSceneManager.LoadScene(NewScene);
		}
	}
}
