﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using Tribal;

public class SettleAboveTerrain : MonoBehaviour {

	public float settleDistance = 0f;
	public bool settleChildren = true;
	public bool orientToNormal = false;

	void Awake()
	{
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
		SettleMe();
	}

	void OnDestroy()
	{
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	}

	private void OnLevelFinishedLoading(Scene scn, LoadSceneMode mode )
	{
		SettleMe();
	}

	public void SettleMe()
	{
		Map.Settle( this.gameObject, settleDistance, settleChildren, orientToNormal );
	}
}
