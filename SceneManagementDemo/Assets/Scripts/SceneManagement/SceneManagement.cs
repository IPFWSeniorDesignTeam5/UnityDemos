using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace VRSceneManagement
{
	public interface SceneState
	{
		void Load ( );

		void Unload ();
	}

	public static class VRSceneManager {

		private static SceneState m_CurrentState = null;

		private static AsyncOperation sceneAsyncOp = null;

		public static void LoadSceneState( SceneState newState )
		{
			if (m_CurrentState != null && newState != m_CurrentState)
				m_CurrentState.Unload ();

			m_CurrentState = newState;
			m_CurrentState.Load ();
		}

		public static void LoadScene( string newScene )
		{
			SceneManager.LoadScene(newScene);
		}

		public static void LoadSceneAsync( string newScene )
		{
			Application.backgroundLoadingPriority = ThreadPriority.Low;
			sceneAsyncOp = SceneManager.LoadSceneAsync(newScene);
			sceneAsyncOp.allowSceneActivation = false;

	        // Wait until Unity has finished loading the scene.
	        // Wth allowSceneActivation = false Unity won't fully load the scene and will
	        // and async.progress will only go as far as 90%
			while (!sceneAsyncOp.isDone)
	        {
				float loadProgress = sceneAsyncOp.progress;

	            if (loadProgress >= 0.9f)
	            {
	                // Almost done.
	                break;
	            }
	        }

	        ScreenFader.StartFade(OVRScreenFade.OVRScreenFadeMode.Out);
			ScreenFader.SubscribeToFades( ScreenFadeComplete );
		}

		public static void ScreenFadeComplete(OVRScreenFade.OVRScreenFadeMode m)
		{
			if( m == OVRScreenFade.OVRScreenFadeMode.Out )
			{
				ScreenFader.UnsubscribeToFades( ScreenFadeComplete );
				sceneAsyncOp.allowSceneActivation = true;
			}
		}
	}
}
