using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouselook : MonoBehaviour {

	float rotationX =  0f;
	float rotationY = 0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		DoMouselook();
	}

	private void DoMouselook()
	{
		float sensitivityX = 3.0f;
		float sensitivityY = 3.0f;
		float minimumX = -360;
		float maximumX = 360;
		float minimumY = -60;
		float maximumY = 60;

		rotationX += Input.GetAxis("Mouse X") * sensitivityX;

		rotationX = ClampAngle(rotationX, minimumX, maximumX);

		rotationY += -Input.GetAxis("Mouse Y") * sensitivityY;
		rotationY = ClampAngle(rotationY, minimumY, maximumY);

		transform.localEulerAngles = new Vector3(rotationY, rotationX, 0);
	}

	static float ClampAngle(float angle, float min, float max)
	{
		if (!(min == 0) && angle < min)
		{
			angle = min;
		}

		if (!(max == 0) && angle > max)
		{
			angle = max;
		}
		
		return angle;
	}
}
