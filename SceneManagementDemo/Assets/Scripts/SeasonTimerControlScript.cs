using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Tribal;

public class SeasonTimerControlScript : MonoBehaviour {

	GameObject Sunlight = null;

	public int SeasonLengthSeconds = 10;

	public Texture2D[] SummerTextures;
	public Texture2D[] SpringTextures;
	public Texture2D[] FallTextures;
	public Texture2D[] WinterTextures;

	public Material[] FallLeavesMaterials;

	private int SeasonLength {
								get {return (int)SeasonTimer.SeasonLength;} 
								set { 	SeasonTimer.SetSeasonLength(value); }
							   }

	// Use this for initialization
	void Awake () {
		Sunlight = GameObject.FindGameObjectWithTag( "Sun" );

		if( null == Sunlight )
			Debug.LogWarning( "Failed to find light marked with tag 'Sun'." );

		SeasonLength = SeasonLengthSeconds;

		SeasonTimer.SeasonEndEvent += SeasonEnd;
	}

	void OnDestroy()
	{
		SeasonTimer.SeasonEndEvent -= SeasonEnd;
	}

	// Update is called once per frame
	void Update () {
		SeasonTimer.Update( Time.time );
		float angleY = 90 * ((((int)SeasonTimer.CurrentSeason))-1) + (SeasonTimer.SeasonProgress * 90f);
		Sunlight.transform.eulerAngles = new Vector3( Sunlight.transform.eulerAngles.x, angleY, Sunlight.transform.eulerAngles.z );
	}

	void SeasonEnd( SeasonTimer.SeasonEndEventArgs e )
	{
		Terrain terrain = GameObject.FindGameObjectWithTag( "Terrain" ).GetComponent<Terrain>();
		TerrainData terrainData = terrain.terrainData;

		if( null != terrain )
		{
			Color leavesColor = new Color(1f,1f,1f);
			Texture2D [] newTextures = null;
			switch( e.start )
			{
				case SeasonTimer.SeasonType.Fall:
					newTextures = FallTextures;
					leavesColor.r = 220f/255f;
					leavesColor.g = 104f/255f;
					leavesColor.b = 44f/255f;
				break;
				case SeasonTimer.SeasonType.Spring:
					newTextures = SpringTextures;
				break;
				case SeasonTimer.SeasonType.Summer:
					newTextures = SummerTextures;
					leavesColor.r = 190f/255f;
					leavesColor.g = 251f/255f;
					leavesColor.b = 211f/255f;
				break;
				case SeasonTimer.SeasonType.Winter:
					newTextures = WinterTextures;
					leavesColor.r = 128f/255f;
					leavesColor.g = 114f/255f;
					leavesColor.b = 102f/255f;
				break;
			}

			foreach( Material mat in FallLeavesMaterials )
				mat.SetColor ("_Color", leavesColor );

			if( newTextures == null ) return;

			SplatPrototype[] splatPrototype = new SplatPrototype[newTextures.Length];
			for (int i = 0; i < terrainData.splatPrototypes.Length; i++)
		    {
		        splatPrototype[i] = new SplatPrototype();
				splatPrototype[i].texture = newTextures[i];
		        splatPrototype[i].tileSize = new Vector2(terrainData.splatPrototypes[0].tileSize.x, terrainData.splatPrototypes[0].tileSize.y);    //Sets the size of the texture
		        splatPrototype[i].tileOffset = new Vector2(terrainData.splatPrototypes[0].tileOffset.x, terrainData.splatPrototypes[0].tileOffset.y);    //Sets the size of the texture
		    }
		    terrainData.splatPrototypes = splatPrototype;
		}

		AudioSource s = GetComponent<AudioSource>();
		s.Play();
	}
}
