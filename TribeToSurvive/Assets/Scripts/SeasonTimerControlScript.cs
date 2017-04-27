using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Tribal;

public class SeasonTimerControlScript : MonoBehaviour {

	GameObject SunlightObject = null;
	GameObject Stars = null;

	public int SeasonLengthSeconds = 10;

	public Gradient nightDayColor;

	public float maxIntensity = 3f;
	public float minIntensity = 0f;
	public float minPoint = -0.2f;

	public float dayMaxAmbient = 1f;
	public float dayMinAmbient = 0f;
	public float nightMaxAmbient = 1f;
	public float nightMinAmbient = 0f;
	public float ambientPoint = -0.2f;

	public Gradient nightDayFogColor;
	public AnimationCurve fogDensityCurve;
	public float fogScale = 1f;

	public float dayAtmosphereThickness = 0.4f;
	public float nightAtmosphereThickness = 0.87f;

	public Texture2D[] SummerTextures;
	public Texture2D[] SpringTextures;
	public Texture2D[] FallTextures;
	public Texture2D[] WinterTextures;

	public Material[] FallLeavesMaterials;

	Skybox sky;
	Material skyMat;
	Light sunLight;

	private int SeasonLength {
								get {return (int)SeasonTimer.SeasonLength;} 
								set { 	SeasonTimer.SetSeasonLength(value); }
							   }

	// Use this for initialization
	void Awake () {
		SunlightObject = GameObject.FindGameObjectWithTag( "Sun" );
		Stars = GameObject.FindGameObjectWithTag( "Stars" );

		if( null == SunlightObject )
			Debug.LogWarning( "Failed to find light marked with tag 'Sun'." );
		else
		{
			sunLight = SunlightObject.GetComponentInChildren<Light>();
			SunlightObject = sunLight.transform.gameObject;
		}

		skyMat = RenderSettings.skybox;

		SeasonLength = SeasonLengthSeconds;
		SeasonTimer.SeasonEndEvent += SeasonEnd;
	}

	void OnDestroy()
	{
		SeasonTimer.SeasonEndEvent -= SeasonEnd;
	}

	// Update is called once per frame
	void Update () {
		float tRange = 1 - minPoint;
		float dot = Mathf.Clamp01((Vector3.Dot(sunLight.transform.forward, Vector3.down) - minPoint) / tRange);
		float i = ((maxIntensity - minIntensity) * dot) + minIntensity;

		sunLight.intensity = i;
		sunLight.color = nightDayColor.Evaluate(dot);

		RenderSettings.ambientLight = sunLight.color;

		RenderSettings.fogColor = nightDayFogColor.Evaluate(dot);
		RenderSettings.fogDensity = fogDensityCurve.Evaluate(dot) * fogScale;

		tRange = 1 - ambientPoint;
		float amb = (Vector3.Dot(sunLight.transform.forward, Vector3.down) - ambientPoint);
		if( amb >= 0 )
		{
			dot = Mathf.Clamp01( amb / tRange);
			i = ((dayMaxAmbient - dayMinAmbient) * dot) + dayMinAmbient;
		} else
		{
			dot = Mathf.Clamp01( -amb / tRange);
			i = ((nightMaxAmbient - nightMinAmbient) * dot) + nightMinAmbient;
		}

		RenderSettings.ambientIntensity = i;

		i = ((dayAtmosphereThickness - nightAtmosphereThickness) * dot) + nightAtmosphereThickness;
		skyMat.SetFloat( "_AtmosphereThickness", i);

		SeasonTimer.Update( Time.time );
		float angleX = (360 * SeasonTimer.SeasonProgress);

		SunlightObject.transform.eulerAngles = new Vector3( angleX, 0, 0 );

		if( null != Stars )
			Stars.transform.rotation = SunlightObject.transform.rotation;
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

			/*
			SplatPrototype[] splatPrototype = new SplatPrototype[newTextures.Length];
			for (int i = 0; i < terrainData.splatPrototypes.Length; i++)
		    {
		        splatPrototype[i] = new SplatPrototype();
				splatPrototype[i].texture = newTextures[i];
		        splatPrototype[i].tileSize = new Vector2(terrainData.splatPrototypes[0].tileSize.x, terrainData.splatPrototypes[0].tileSize.y);    //Sets the size of the texture
		        splatPrototype[i].tileOffset = new Vector2(terrainData.splatPrototypes[0].tileOffset.x, terrainData.splatPrototypes[0].tileOffset.y);    //Sets the size of the texture
		    }
		    terrainData.splatPrototypes = splatPrototype;
		    */
		}

		AudioSource s = GetComponent<AudioSource>();
		s.Play();
	}
}
