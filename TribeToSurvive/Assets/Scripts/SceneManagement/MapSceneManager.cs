using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSceneManagement;

public class MapSceneManager : MonoBehaviour {

    public GameObject VRCameraPrefab = null;

	void Awake () {
		Random.InitState (0);

        GameObject VRCamera = GameObject.FindGameObjectWithTag("VRGameObject");

        if (null == VRCamera)
            VRCamera = GameObject.Instantiate(VRCameraPrefab);

        VRSceneManager.LoadSceneState(VRCamera.GetComponent<MapOverviewState>());
	}
}
