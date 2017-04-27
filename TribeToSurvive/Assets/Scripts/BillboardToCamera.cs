using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardToCamera : MonoBehaviour {

		// Update is called once per frame
	void Update () {
		if ( null == Camera.main ) return;
	
		this.transform.LookAt( Camera.main.transform.position );
	}
}
