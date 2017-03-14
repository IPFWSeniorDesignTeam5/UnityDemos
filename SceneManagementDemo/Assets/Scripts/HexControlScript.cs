using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tribal;

public class HexControlScript : MonoBehaviour {

	public MapNode HexMapNode { get; private set; }

	public delegate void HighlightedEventListener();

	public event HighlightedEventListener Highlighted;
	public event HighlightedEventListener Dehighlighted;

	private bool highlighted = false;
	private float lastOnSwitch = 0f;
	private MeshRenderer OutlineRenderer = null;

	void Awake()
	{
		Transform ot = transform.Find( "hex_pad" );

		if( null == ot ) return;

		GameObject outline = ot.gameObject;

		if( null == outline )
			Debug.LogError("No hex_pad child on hex control script object." );

		OutlineRenderer = outline.GetComponent<MeshRenderer>();
	}

	public void SetMapNode( MapNode node )
	{
		HexMapNode = node;
	}

	public void SetSelected( bool isSelected )
	{
		if( isSelected )
		{
			if( !highlighted && null != Highlighted )
				Highlighted();
		}
		else
			if( highlighted && null != Dehighlighted )
				Dehighlighted();

		highlighted = isSelected;
		SetOutlineEnabled( isSelected );
	}

	public void SetOutlineEnabled( bool isEnabled )
	{
		if( null == OutlineRenderer ) return;
		if( isEnabled ) lastOnSwitch = Time.time;
		OutlineRenderer.enabled = isEnabled;
	}
}
