using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Tribal;

public class SeasonTimerControlScript : MonoBehaviour {

	GameObject Sunlight = null;

	public int SeasonLengthSeconds {
									get {return (int)SeasonTimer.SeasonLength;} 
									set { 	SeasonTimer.SetSeasonLength(value); }
								   }

	// Use this for initialization
	void Awake () {
		Sunlight = GameObject.FindGameObjectWithTag( "Sun" );

		if( null == Sunlight )
			Debug.LogWarning( "Failed to find light marked with tag 'Sun'." );

		SeasonLengthSeconds = 10;
		SeasonTimer.SeasonEndEvent += SeasonEnd;
	}

	// Update is called once per frame
	void Update () {
		SeasonTimer.Update( Time.time );
		float angleY = 90 * ((((int)SeasonTimer.CurrentSeason))-1) + (SeasonTimer.SeasonProgress * 90f);
		Sunlight.transform.eulerAngles = new Vector3( Sunlight.transform.eulerAngles.x, angleY, Sunlight.transform.eulerAngles.z );
	}

	void SeasonEnd( SeasonTimer.SeasonEndEventArgs e )
	{
		AudioSource s = GetComponent<AudioSource>();
		s.Play();
	}
}
