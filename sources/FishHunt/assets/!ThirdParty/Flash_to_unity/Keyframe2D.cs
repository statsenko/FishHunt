using UnityEngine;
using System.Collections;

public class Keyframe2D {

	public int frame;
	public float x;
	public float y;
	public float scaleX;
	public float scaleY;
	public float rotation;
	public float r;
	public float g;
	public float b;
	public float a;
	
	public Keyframe2D(int frame, float x, float y, float scaleX, float scaleY, float rotation, float r, float g, float b, float a) {
		this.frame = frame;
		this.x = x;
		this.y = y;
		this.scaleX = scaleX;
		this.scaleY = scaleY;
		this.rotation = rotation;
		this.r = r;
		this.g = g;
		this.b = b;
		this.a = a;
	}
	
}
