/************************************************************************************

Copyright   :   Copyright 2014 Oculus VR, LLC. All Rights reserved.

Licensed under the Oculus VR Rift SDK License Version 3.3 (the "License");
you may not use the Oculus VR Rift SDK except in compliance with the License,
which is provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at

http://www.oculus.com/licenses/LICENSE-3.3

Unless required by applicable law or agreed to in writing, the Oculus VR SDK
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

************************************************************************************/
using System.Collections.Generic;
using UnityEngine;
using System.Collections; // required for Coroutines
using UnityEngine.SceneManagement;

/// <summary>
/// Fades the screen from black after a new scene is loaded.
/// </summary>
public class OVRScreenFade : MonoBehaviour
{
    public enum OVRScreenFadeMode
    {
        In = 0,
        Out
    }

	/// <summary>
	/// How long it takes to fade.
	/// </summary>
	public float fadeTime = 2.0f;

	/// <summary>
	/// The initial screen color.
	/// </summary>
	public Color fadeInColor = new Color(0.01f, 0.01f, 0.01f, 1.0f);
	public Color fadeOutColor = new Color(0.01f, 0.01f, 0.01f, 0f);

	private Material fadeMaterial = null;
	private bool isFading = false;
	private YieldInstruction fadeInstruction = new WaitForEndOfFrame();

	/// <summary>
	/// Initialize.
	/// </summary>
	void Awake()
	{
		// create the fade material
		fadeMaterial = new Material(Shader.Find("Oculus/Unlit Transparent Color"));
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	/// <summary>
	/// Starts a fade in when a new level is loaded
	/// </summary>
	private void OnLevelFinishedLoading(Scene scn, LoadSceneMode mode )
	{
        StartCoroutine(DoFade(OVRScreenFadeMode.In));
	}

    public void Fade( OVRScreenFadeMode dir )
    {
        StartCoroutine(DoFade(dir)); 
    }

	/// <summary>
	/// Cleans up the fade material
	/// </summary>
	void OnDestroy()
	{
		if (fadeMaterial != null)
		{
			Destroy(fadeMaterial);
		}
	}

	/// <summary>
	/// Fades alpha from 1.0 to 0.0
	/// </summary>
	IEnumerator DoFade( OVRScreenFadeMode mode )
	{
		float elapsedTime = 0.0f;

		fadeMaterial.color = (mode == OVRScreenFadeMode.In ? fadeInColor : fadeOutColor);
		Color color = fadeMaterial.color;
		isFading = true;

		while (elapsedTime < fadeTime)
		{
			yield return fadeInstruction;
			elapsedTime += Time.deltaTime;

            if (mode == OVRScreenFadeMode.In )
			    color.a = 1.0f - Mathf.Clamp01(elapsedTime / fadeTime);
            else
                color.a = Mathf.Clamp01(elapsedTime / fadeTime);

			fadeMaterial.color = color;
		}

		if( mode == OVRScreenFadeMode.In )
			isFading = false;

		ScreenFader.FadeComplete( mode );
	}

	/// <summary>
	/// Renders the fade overlay when attached to a camera object
	/// </summary>
	void OnPostRender()
	{
		if (isFading)
		{
			fadeMaterial.SetPass(0);
			GL.PushMatrix();
			GL.LoadOrtho();
			GL.Color(fadeMaterial.color);
			GL.Begin(GL.QUADS);
			GL.Vertex3(0f, 0f, -12f);
			GL.Vertex3(0f, 1f, -12f);
			GL.Vertex3(1f, 1f, -12f);
			GL.Vertex3(1f, 0f, -12f);
			GL.End();
			GL.PopMatrix();
		}
	}
}
