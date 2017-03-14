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
	}
}
