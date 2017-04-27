using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using Tribal;
using System;
using UnityEngine.UI;

public class FamilyInfoControlScript : MonoBehaviour {

    public float UpdateDelay = 1f;
    private float timeSinceUpdate = 0f;

    public Text ProductionHH_Text = null;
	public Text FarmingHH_Text = null;
	public Text GatheringHH_Text = null;
	public Text HuntingHH_Text = null;

	public Text Name_Text = null;
	public Text Population_Text = null;
	public Text Capability_Text = null;
	public Text Prosperity_Text = null;

	public Text RawMaterials_Text = null;
	public Text FinishedGoods_Text = null;

	Family m_Family = null;

	// Update is called once per frame
	void Update () {
        timeSinceUpdate += Time.deltaTime;

		if (timeSinceUpdate >= UpdateDelay)
        {
            timeSinceUpdate = 0f;
            UpdateData();
        }
	}

    public void SetFamily(Family info)
    {
    	bool update = false;

    	if( null == m_Family ) update = true;
    		
		m_Family = info;

		if( update )
			UpdateData();
    }

    public void UpdateData()
    {
    	if( null == m_Family ) return;

    	Name_Text.text = m_Family.FamilyName;
    	Population_Text.text = "Pop: " + m_Family.Population.ToString();
    	Capability_Text.text = "Capability: " + m_Family.Capability.ToString("##.##");
    	Prosperity_Text.text = "Prosperity: " + m_Family.Prosperity.ToString("##.##");

    	ProductionHH_Text.text = m_Family.FamilyNodes.Where( x => x.gameObject.GetComponent<FamilyControlScript>().CurrentActivity == Skill.SkillType.Production ).Count().ToString();
		FarmingHH_Text.text = m_Family.FamilyNodes.Where( x => x.gameObject.GetComponent<FamilyControlScript>().CurrentActivity == Skill.SkillType.Farming ).Count().ToString();
		GatheringHH_Text.text = m_Family.FamilyNodes.Where( x => x.gameObject.GetComponent<FamilyControlScript>().CurrentActivity == Skill.SkillType.Gathering ).Count().ToString();
		HuntingHH_Text.text = m_Family.FamilyNodes.Where( x => x.gameObject.GetComponent<FamilyControlScript>().CurrentActivity == Skill.SkillType.Hunting ).Count().ToString();

		Dictionary<string, RawMaterialType> matList = TribalAssets.MaterialDictionary;
		Dictionary<string, FinishedGoodType> goodList = TribalAssets.GoodsDictionary;

		RawMaterials_Text.text = "";
		FinishedGoods_Text.text = "";

		foreach( var key in matList )
			RawMaterials_Text.text += key.Key + ": [" + m_Family.RawMaterialCount(key.Value) + "]\n";

		foreach( var key in goodList )
			FinishedGoods_Text.text += key.Key + ": [" + m_Family.FinishedGoodCount(key.Value) + "]\n";
    }
}
