using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using Tribal;

[RequireComponent(typeof(HexControlScript) )]
public class FamilyControlScript : MonoBehaviour {

	HexControlScript HexControl = null;

	public GameObject houseObject = null;

	public Text NameText;

	bool spinning = false;

	[Range(0f, 500f)]
	public float SpinSpeed = 300f;

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

		if( null == houseObject )
			Debug.LogError( "Null house object on FamilyControlScript." );
	}

	public void Highlighted()
	{
		spinning = true;
	}

	public void Dehighlighted()
	{
		spinning = false;
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if( spinning )
		{
			houseObject.transform.Rotate( (Vector3.up * 300f) * Time.deltaTime);
		} else
		{
			houseObject.transform.rotation = this.transform.rotation;
		}
	}
}
