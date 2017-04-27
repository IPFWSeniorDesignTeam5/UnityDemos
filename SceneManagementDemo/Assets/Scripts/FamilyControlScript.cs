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

	GameObject currentDetail;

	public Transform detailPosition;

	public GameObject FarmingActivityDetail;
	public GameObject HuntingActivityDetail;
	public GameObject GatheringActivityDetail;
	public GameObject ProductionActivityDetail;

	private Skill.SkillType Activity;

	public Skill.SkillType CurrentActivity  {
		get {
			return Activity;
		}
		set
		{ 
			if( null != currentDetail )
				Destroy(currentDetail);

			Activity = value;
			ActivityProgress = 0f; 
			UpdateActivity();
		} 
	}

    public float ActivityProgress { get; private set; }

	public Family FamilyInfo { get; private set; }

	public void SetFamily( Family fam )
	{
		if( null != fam )
		{
			FamilyInfo = fam;
		}
	}

	void Awake ()
	{
		HexControl = GetComponent<HexControlScript>();
		HexControl.Highlighted += Highlighted;
		HexControl.Dehighlighted += Dehighlighted;

		UpdateActivity();
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

			if( null != hControl )
				hControl.SetOutlineEnabled( is_highlighted );
		}
	}

	private void UpdateActivity()
	{
		switch( CurrentActivity )
		{
			case Skill.SkillType.None:
				
			break;
			case Skill.SkillType.Farming:
			currentDetail = GameObject.Instantiate( FarmingActivityDetail, detailPosition.position, detailPosition.rotation, this.transform );
			break;
			case Skill.SkillType.Production:
				if( null != ProductionActivityDetail )
				currentDetail = GameObject.Instantiate( ProductionActivityDetail, detailPosition.position, detailPosition.rotation, this.transform );
			break;
			case Skill.SkillType.Hunting:
			currentDetail = GameObject.Instantiate( HuntingActivityDetail, detailPosition.position, detailPosition.rotation, this.transform );
			break;
			case Skill.SkillType.Gathering:
			currentDetail = GameObject.Instantiate( GatheringActivityDetail, detailPosition.position, detailPosition.rotation, this.transform );
			break;
		}
	}

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		//if( null != FamilyInfo && null != HexControl && FamilyInfo.StartingNode == HexControl.HexMapNode )	// I'm a main node
		//	UpdateStatsText();
	}
}
