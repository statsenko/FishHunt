using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour {

	public float forceY = 1500f;

	Transform tr;
	GameObject go;
	float speed = 3f;

	GameObject goRight;
	GameObject goLeft;

	float widthRight = 3f;
	float widthLeft = 3f;

	float limitXRight = 0f;
	float limitXLeft = 0f;

	float direction = 1;

	public playerState state;

	float deltaSecondsTime = 300f;
	DateTime deltaTimeFly;
	bool isFirstFly = false;

	public DateTime currentTimeRunup;
	public float maxRunupTime = 3500f;

	void Awake()
	{
		state = playerState.normal;
	}

	// Use this for initialization
	void Start () {
		deltaTimeFly = DateTime.Now;
		tr = transform;
		go = gameObject;
		goRight = GameObject.Find("right_stena");
		goLeft = GameObject.Find("left_stena");

		limitXRight = goRight.transform.localPosition.x - widthRight/2f;
		limitXLeft = goLeft.transform.localPosition.x + limitXLeft + 1f;

		//goRight.GetComponent<Collider2D>().
	}
	
	// Update is called once per frame
	void Update () {
		if(state == playerState.runup && currentTimeRunup.AddMilliseconds(maxRunupTime) < DateTime.Now)
		{
			startFlight(700f);
		}
		if(tr.localPosition.y <= -3.49f && state != playerState.runup && isFirstFly && deltaTimeFly.AddMilliseconds(deltaSecondsTime) < DateTime.Now)
			state = playerState.normal;
		if(state == playerState.normal)
		{
			if(limitXRight <= tr.localPosition.x)
				direction = -1f;
			//Debug.Log("limitXLeft:"+limitXLeft.ToString()+"|pos:"+tr.localPosition.x.ToString());
			if(limitXLeft >= tr.localPosition.x)
				direction = 1f;
			float deltaX = speed*Time.deltaTime;
			float newPositionX = tr.localPosition.x + deltaX*direction;
			//Debug.Log("newPositionX:"+newPositionX.ToString());
			tr.localPosition = new Vector3(newPositionX, tr.localPosition.y, tr.localPosition.z);
		}
	}
	void click()
	{
		if(state != playerState.flight)
		{
			float _deltaForce = (float)((DateTime.Now - currentTimeRunup).TotalMilliseconds/5f);
			Debug.Log ("_deltaForce:"+_deltaForce.ToString());
			startFlight(_deltaForce);
		}
	}
	void OnMouseDown()
	{
		Debug.Log("OnMouseDown()");
		state = playerState.runup;
		deltaTimeFly = DateTime.Now;
		currentTimeRunup = DateTime.Now;
	}

	void OnMouseUp()
	{
		click();
		Debug.Log("OnMouseUp()");
	}

	void startFlight(float deltaForce)
	{
		tr.rigidbody2D.AddForce(new Vector2(0,forceY+deltaForce));
		state = playerState.flight;
		deltaTimeFly = DateTime.Now;
		isFirstFly = true;
	}
}

public enum playerState
{
	normal,
	runup,
	flight,
}

