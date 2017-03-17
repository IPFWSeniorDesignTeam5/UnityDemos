using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using Tribal;

[RequireComponent(typeof(HexControlScript) )]
public class FamilyControlScript : MonoBehaviour {

	HexControlScript HexControl = null;

	public GameObject houseObject = null;

	public Text NameText, ActivityText, StatsText, GoodsText;

	private Skill.SkillType Activity;

	public Skill.SkillType CurrentActivity  {
		get {
			return Activity;
		}
		set
		{ 
			Activity = value;
			ActivityProgress = 0f; 
			UpdateActivityIcon();
		} 
	}

    public float ActivityProgress { get; private set; }

	public Family FamilyInfo { get; private set; }

	public void SetFamily( Family fam )
	{
		if( null != fam )
		{
			FamilyInfo = fam;
			NameText.text = fam.FamilyName;
		}
	}

	void Awake ()
	{
		HexControl = GetComponent<HexControlScript>();
		HexControl.Highlighted += Highlighted;
		HexControl.Dehighlighted += Dehighlighted;

		UpdateActivityIcon();

		if( null == houseObject )
			Debug.LogError( "Null house object on FamilyControlScript." );
	}

	public void Highlighted()
	{
		TriggerHighlighted( true );
	}

	public void Dehighlighted()
	{
		TriggerHighlighted( false );
	}

	private void TriggerHighlighted( bool is_highlighted )
	{
		HexControlScript hControl = null;

		foreach( MapNode n in FamilyInfo.FamilyNodes )
		{
			hControl = n.gameObject.GetComponent<HexControlScript>();

			if( n == FamilyInfo.StartingNode )
				hControl.gameObject.GetComponent<FamilyControlScript>().SetTextVisible( is_highlighted );

			if( null != hControl )
				hControl.SetOutlineEnabled( is_highlighted );
		}
	}

	private void UpdateStatsText()
	{
		if( null == StatsText ) {
			Debug.LogError( "Null stats text object." );
			return;
		}

		StatsText.text = "Population: " + Math.Round(FamilyInfo.Population);
		StatsText.text += "\nCapability: " + Math.Round(FamilyInfo.Capability,2);
		StatsText.text += "\nProsperity: " + Math.Round(FamilyInfo.Prosperity, 2);

		bool first = true;
		string line = "";

		foreach( RawMaterialType typ in Enum.GetValues(typeof(RawMaterialType)) )
		{
			line = Enum.GetName( typeof(RawMaterialType), typ ) + ": [" + FamilyInfo.RawMaterialCount( typ ) + "]";
			if( first )  {
				GoodsText.text = line;
				first = false;
			} else
				GoodsText.text += "\n" + line;
		}
	}

	private void UpdateActivityIcon()
	{
		switch( CurrentActivity )
		{
			case Skill.SkillType.None:
				ActivityText.text = "ø";
				ActivityText.color = Color.red;
			break;
			case Skill.SkillType.Farming:
				ActivityText.text = "F";
				ActivityText.color = Color.white;
			break;
			case Skill.SkillType.Production:
				ActivityText.text = "P";
				ActivityText.color = Color.white;
			break;
			case Skill.SkillType.Hunting:
				ActivityText.text = "H";
				ActivityText.color = Color.white;
			break;
			case Skill.SkillType.Gathering:
				ActivityText.text = "G";
				ActivityText.color = Color.white;
			break;
		}
	}

	public void SetTextVisible( bool vis )
	{
		NameText.enabled = vis;
	}

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		if( null != FamilyInfo && null != HexControl && FamilyInfo.StartingNode == HexControl.HexMapNode )	// I'm a main node
			UpdateStatsText();
	}
}
