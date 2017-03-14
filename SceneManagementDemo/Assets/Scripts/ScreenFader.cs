using UnityEngine;

public static class ScreenFader
{
	public delegate void ScreenFadeEventListener(OVRScreenFade.OVRScreenFadeMode m);

	public static event ScreenFadeEventListener FadeCompleteEvent;

	public static OVRScreenFade GetFader()
	{
		GameObject fadeObject = GameObject.FindGameObjectWithTag( "VRGameObject" );

		if( null == fadeObject ) {
			Debug.LogError( "Failed to find OVRCameraRig object with tag 'VRGameObject'." );
			return null;
		}

		OVRScreenFade fader = fadeObject.GetComponentInChildren<OVRScreenFade>();

		if( null == fader ) {
			Debug.LogError( "OVRCameraRig object has no child with OVRScreenFade component." );
			return null;
		}

		return fader;	
	}

	public static void StartFade( OVRScreenFade.OVRScreenFadeMode mode )
	{
		OVRScreenFade fader = GetFader();

		if( null != fader )
			fader.Fade( mode );
	}

	public static void SubscribeToFades( ScreenFadeEventListener del )
	{
		if( null != del )
			FadeCompleteEvent += del;
	}

	public static void UnsubscribeToFades( ScreenFadeEventListener del )
	{
		if( null != del )
			FadeCompleteEvent -= del;
	}

	public static void FadeComplete( OVRScreenFade.OVRScreenFadeMode m )
	{
		if( null != FadeCompleteEvent )
			FadeCompleteEvent( m );
	}
}