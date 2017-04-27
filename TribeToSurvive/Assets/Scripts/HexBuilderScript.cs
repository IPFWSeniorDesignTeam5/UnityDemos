using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tribal;

public class HexBuilderScript : MonoBehaviour {

	HexControlScript hControl = null;

	void Awake()
	{
		hControl = GetComponent<HexControlScript>();
	}

	public void Settle()
	{
		Map.Settle( this.gameObject, 0f );
	}
}
