
using UnityEngine;
using System.Collections;

class TouchInfo
{
	public float timeTouchStarted;
    public Vector2 touchPosition;
	public bool touchEnded;
}

public class InputController : MonoBehaviour 
{
	static InputController instance_ = null;
	public static InputController Instance()
	{
		if (instance_ == null)
		{
			instance_ = GameObject.FindObjectOfType(typeof(InputController)) as InputController;
		}
		return instance_;
	}
	public static InputState State {get{ return InputController.Instance().State_;}}
	public static Vector2 ScreenInputPosition {get{ return InputController.Instance().ScreenInputPosition_();}}
	
	public enum InputState
	{
		NA = 0,
		Hold = 1,
		Tap = 2,
		SwipeLeft = 3,
		SwipeRight = 4,
		SwipeUp = 5,
		SwipeDown = 6,
	}
	//public member vars
	public int swipeLength = 40;
	public float timeToSwipe = 0.0f;
	
	InputState State_
	{
		get
		{
			if (enabled == false)
				return InputState.NA;
			
			if (needUpdate)
				UpdateInput();
			
			return state;
		}
	}
	//private member vars
	private TouchInfo touchInfo = null;
	private InputState state = InputState.NA;
	private bool needUpdate = false;
	//methods
	void Awake()
	{

		touchInfo = new TouchInfo();
		state = InputState.NA;
		needUpdate = false;
	}
    void Start()
    {
		
    }   
	void Update()
	{
		needUpdate = true;
	}
	
	void UpdateInput()
	{
#if UNITY_IPHONE && !UNITY_EDITOR
		UpdateTouchInput();
#else
		UpdateMouseInput();
#endif
	}
	
	void UpdateMouseInput()
	{
		if (Input.GetMouseButtonDown(0))
		{
			if(touchInfo == null)
				touchInfo = new TouchInfo();
			
			state = InputState.Hold;
				
			touchInfo.touchPosition = Input.mousePosition;
			touchInfo.timeTouchStarted = Time.time;
			touchInfo.touchEnded = false;
		}
		else if (Input.GetMouseButton(0) && !touchInfo.touchEnded)
		{
			if(Input.mousePosition.y > (touchInfo.touchPosition.y + swipeLength))
			{
				//swipe up
				if (SwipeComplete())
				{
					state = InputState.SwipeUp;
					touchInfo.touchEnded = true;
				}
				else
					touchInfo.touchPosition.y = Input.mousePosition.y;
			}
			if (Input.mousePosition.y < (touchInfo.touchPosition.y - swipeLength))
			{
				//swipe down
				if (SwipeComplete())
				{
					state = InputState.SwipeDown;
					touchInfo.touchEnded = true;
				}
				else
					touchInfo.touchPosition.y = Input.mousePosition.y;
			}
			if (Input.mousePosition.x > (touchInfo.touchPosition.x + swipeLength))
			{
				//Swipr Right
				if (SwipeComplete())
				{
					state = InputState.SwipeRight;
					touchInfo.touchEnded = true;
				}
				else
					touchInfo.touchPosition.x = Input.mousePosition.x;
			}
			if (Input.mousePosition.x < (touchInfo.touchPosition.x - swipeLength))
			{
				//Swipe Left
				if (SwipeComplete())
				{
					state = InputState.SwipeLeft;
					touchInfo.touchEnded = true;
				}
				else
					touchInfo.touchPosition.x = Input.mousePosition.x;
			}
		}
		else if (Input.GetMouseButtonUp(0) && !touchInfo.touchEnded)
		{
			//tap
			if (TapComplete())
			{
				state = InputState.Tap;
				touchInfo.touchEnded = true;
			}
		}
		else
			state = InputState.NA;

		needUpdate = false;
	}
	
    void UpdateTouchInput()
    {
		state = InputState.NA;
		if (Input.touchCount > 0)
		{
			Touch touch = Input.touches[0];

			if(touchInfo == null)
				touchInfo = new TouchInfo();
			
			if(touch.phase == TouchPhase.Began)
			{
				state = InputState.Hold;
				
				touchInfo.touchPosition = touch.position;
				touchInfo.timeTouchStarted = Time.time;
				touchInfo.touchEnded = false;
			}
			
			if (touchInfo.touchEnded)
				return;
			
			if(touch.position.y > (touchInfo.touchPosition.y + swipeLength))
			{
				//swipe up
				if (SwipeComplete())
				{
					state = InputState.SwipeUp;
					touchInfo.touchEnded = true;
				}
				else
					touchInfo.touchPosition.y = touch.position.y;
			}
			if (touch.position.y < (touchInfo.touchPosition.y - swipeLength))
			{
				//swipe down
				if (SwipeComplete())
				{
					state = InputState.SwipeDown;
					touchInfo.touchEnded = true;
				}
				else
					touchInfo.touchPosition.y = touch.position.y;
			}
			if (touch.position.x > (touchInfo.touchPosition.x + swipeLength))
			{
				//Swipr Right
				if (SwipeComplete())
				{
					state = InputState.SwipeRight;
					touchInfo.touchEnded = true;
				}
				else
					touchInfo.touchPosition.x = touch.position.x;
			}
			if (touch.position.x < (touchInfo.touchPosition.x - swipeLength))
			{
				//Swipe Left
				if (SwipeComplete())
				{
					state = InputState.SwipeLeft;
					touchInfo.touchEnded = true;
				}
				else
					touchInfo.touchPosition.x = touch.position.x;
			}
			if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
			{
				//tap
				if (TapComplete())
				{
					state = InputState.Tap;
					touchInfo.touchEnded = true;
				}
			}
		}
		
		needUpdate = false;
	}   

    bool SwipeComplete()
    {
        return 	((timeToSwipe == 0.0f) || (timeToSwipe > 0.0f && (Time.time - touchInfo.timeTouchStarted) <= timeToSwipe)) &&
				((int)(state & InputState.SwipeLeft | InputState.SwipeRight | InputState.SwipeDown | InputState.SwipeUp) != 1);
    }
	
	bool TapComplete()
	{
		return ((int)(state & InputState.SwipeLeft | InputState.SwipeRight | InputState.SwipeDown | InputState.SwipeUp) != 1);
	}
	
	Vector3 WorldInputPosition_()
	{
		if (touchInfo != null)
			return Camera.main.ScreenToWorldPoint(touchInfo.touchPosition);
		else
			return Vector3.zero; 
	}
	
	Vector2 ScreenInputPosition_()
	{
		if (touchInfo != null)
			return touchInfo.touchPosition;
		else
			return Vector2.zero;
	}
}