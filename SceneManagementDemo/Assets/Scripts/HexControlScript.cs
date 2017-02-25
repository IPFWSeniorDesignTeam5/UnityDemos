using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tribal;

public class HexControlScript : MonoBehaviour {

	public MapNode HexMapNode { get; private set; }

	public delegate void HighlightedEventListener();

	public event HighlightedEventListener Highlighted;
	public event HighlightedEventListener Dehighlighted;

	public Light Spotlight;

	public float LightLingerTime = 0.3f;

	private float lastOnSwitch = 0f;

	public void SetMapNode( MapNode node )
	{
		HexMapNode = node;
	}

	public void SwitchSpotlight( bool lightEnabled )
	{
		if( lightEnabled )
		{
			lastOnSwitch = Time.time;

			if( !Spotlight.enabled && null != Highlighted )
				Highlighted();
		}
		else
			if( null != Dehighlighted )
				Dehighlighted();
		
		Spotlight.enabled = lightEnabled;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if( Spotlight.enabled && (Time.time - lastOnSwitch > LightLingerTime ) )
			SwitchSpotlight( false );
	}
}
