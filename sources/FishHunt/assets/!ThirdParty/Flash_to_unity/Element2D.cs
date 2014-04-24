using UnityEngine;
using System.Collections;

public class Element2D {

	public Keyframe2D[] keyframes;
	
	public Transform target;
	private int count = 0;
	private float prevX = 0;
	private float prevY = 0;
	private float prevScaleX = 0;
	private float prevScaleY = 0;
	private float prevRotation = 0;
	private float prevR = 1;
	private float prevG = 1;
	private float prevB = 1;
	private float prevA = 1;
	
	private float fadeX = 0;
	private float fadeY = 0;
	private float fadeScaleX = 0;
	private float fadeScaleY = 0;
	private float fadeRotation = 0;
	private float fadeR = 1;
	private float fadeG = 1;
	private float fadeB = 1;
	private float fadeA = 1;
	
	private float initScaleX = 1f;
	private float initScaleY = 1f;
	
	private Color color;
	private Color [] colors = new Color[4];
	
	private Mesh mesh;
		
	public Element2D(Animation2D parent, string name, float x, float y, float scaleX, float scaleY, float rotation, float px1, float py1, float px2, float py2, float uvx1, float uvy1, float uvx2, float uvy2, float r, float g, float b, float a, int keyframes) {
		this.keyframes = new Keyframe2D[keyframes];
		
		GameObject o = new GameObject("Element" + parent.transform.childCount + (name != "" ? " : " + name : ""));
		this.target = o.transform;
	
		this.target.parent = parent.transform;
		this.target.localPosition = new Vector3(x, y, -parent.transform.childCount);
		this.target.localScale = new Vector3(scaleX, scaleY, 1);
		this.target.eulerAngles = new Vector3(0, 0, rotation);
		
		MeshFilter filter = (MeshFilter)o.AddComponent(typeof(MeshFilter));
		MeshRenderer renderer = (MeshRenderer)o.AddComponent(typeof(MeshRenderer));
		mesh = new Mesh();
		
		renderer.material = parent.material;
		
		Vector3 [] vertices = new Vector3[4];
		int [] triangles = new int[6];
		Vector2 [] uv = new Vector2[4];
		
		// Vertices
		vertices[0] = new Vector3(px1, py1, 0);
		vertices[1] = new Vector3(px2, py1, 0);
		vertices[2] = new Vector3(px1, py2, 0);
		vertices[3] = new Vector3(px2, py2, 0);
		
		// Triangles
		triangles[0] = 0;
		triangles[1] = 1;
		triangles[2] = 2;
		triangles[3] = 1;
		triangles[4] = 3;
		triangles[5] = 2;
		
		// UVS
		uv[0] = new Vector2(uvx1, uvy1);
		uv[1] = new Vector2(uvx2, uvy1);
		uv[2] = new Vector2(uvx1, uvy2);
		uv[3] = new Vector2(uvx2, uvy2);
		
		// Colors
		this.color = new Color(r, g, b, a);
		this.colors[0] = this.color;
		this.colors[1] = this.color;
		this.colors[2] = this.color;
		this.colors[3] = this.color;
		
		this.mesh.vertices = vertices;
		this.mesh.triangles = triangles;
		this.mesh.colors=this.colors;
		this.mesh.uv = uv;
		
		this.mesh.RecalculateNormals();
		
		filter.sharedMesh = this.mesh;
		
		mesh.colors = colors;
		
	}
	
	public Element2D(Animation2D parent, string name, float x, float y, float scaleX, float scaleY, float rotation, int keyframes) {
		this.keyframes = new Keyframe2D[keyframes];
		this.target = parent.transform.Find(name);
		this.initScaleX = this.target.localScale.x;
		this.initScaleY = this.target.localScale.y;
		this.target.localPosition = new Vector3(x, y, this.target.localPosition.z);
		this.target.localScale = new Vector3(this.initScaleX * scaleX, this.initScaleY * scaleY, this.target.localScale.z);
		this.target.eulerAngles = new Vector3(0, 0, rotation);
	}
	
	public void AddKeyframe(Keyframe2D keyframe) {
		this.keyframes[count] = keyframe;
		count++;
	}
	
	public void StartFade(int frame) {
		this.prevX = this.target.localPosition.x;
		this.prevY = this.target.localPosition.y;
		this.prevScaleX = this.target.localScale.x / this.initScaleX;
		this.prevScaleY = this.target.localScale.y / this.initScaleX;
		this.prevRotation = this.target.eulerAngles.z;
		this.prevR = this.color.r;
		this.prevG = this.color.g;
		this.prevB = this.color.b;
		this.prevA = this.color.a;
		
		Keyframe2D begin = this.keyframes[0];
		Keyframe2D end = null;

		for(int i = 0; i < this.keyframes.Length; i++) {
			Keyframe2D k = this.keyframes[i];
			if(k.frame <= frame) begin = k;
			if(k.frame > frame && end == null) end = k;
		}
		
		float p;
		
		if(end == null) {
			end = begin;
			p = 1;
		} else {
			p = ((float)frame - (float)begin.frame)/((float)end.frame - (float)begin.frame);
		}
		
		this.fadeX = begin.x + (p * (end.x - begin.x));
		this.fadeY = begin.y + (p * (end.y - begin.y));
		
		this.fadeScaleX = begin.scaleX + (p * (end.scaleX - begin.scaleX));
		this.fadeScaleY = begin.scaleY + (p * (end.scaleY - begin.scaleY));
		
		// find shortest rotation direction, could be more optimized
		float r1 = begin.rotation;
		float r2 = end.rotation;
		
		while(r1 < 0f) r1 += 360f;
		while(r2 < 0f) r2 += 360f;
		
		float r3 = r2 - 360f;
		if(Mathf.Abs(r2 - r1) > Mathf.Abs(r3 - r1)) r2 = r3;
		r3 = r2 + 360f;
		if(Mathf.Abs(r2 - r1) > Mathf.Abs(r3 - r1)) r2 = r3;
		
		this.fadeRotation=r1 + (p * (r2 - r1));
		
		this.fadeR=begin.r + (p * (end.r - begin.r));
		this.fadeG=begin.g + (p * (end.g - begin.g));
		this.fadeB=begin.b + (p * (end.b - begin.b));
		this.fadeA=begin.a + (p * (end.a - begin.a));
	}
	
	public void Fade(float p) {
		
		float x = this.prevX + (p * (fadeX - this.prevX));
		float y = this.prevY + (p * (fadeY - this.prevY));
		this.target.localPosition = new Vector3(x, y, this.target.localPosition.z);
		
		x = this.prevScaleX + (p * (fadeScaleX - this.prevScaleX));
		y = this.prevScaleY + (p * (fadeScaleY - this.prevScaleY));
		this.target.localScale = new Vector3(x * this.initScaleX, y * this.initScaleY, this.target.localScale.z);
		
		// find shortest rotation direction, could be more optimized
		float r1 = this.prevRotation;
		float r2 = fadeRotation;
		
		while(r1 < 0f) r1 += 360f;
		while(r2 < 0f) r2 += 360f;
		
		float r3 = r2 - 360f;
		if(Mathf.Abs(r2 - r1) > Mathf.Abs(r3 - r1)) r2 = r3;
		r3 = r2 + 360f;
		if(Mathf.Abs(r2 - r1) > Mathf.Abs(r3 - r1)) r2 = r3;
		
		this.target.eulerAngles = new Vector3(this.target.eulerAngles.x, this.target.eulerAngles.y, r1 + (p * (r2 - r1)));
		
		if(mesh != null && (this.color.r != fadeR || this.color.g != fadeG || this.color.b != fadeB || this.color.a != fadeA)) {
			this.color=new Color(
				this.prevR + (p * (fadeR - this.prevR)),
				this.prevG + (p * (fadeG - this.prevG)),
				this.prevB + (p * (fadeB - this.prevB)),
				this.prevA + (p * (fadeA - this.prevA))
			);
			this.colors[0] = this.colors[1] = this.colors[2] = this.colors[3] = this.color;
			mesh.colors = this.colors;
		}
	}
	
	public void Update(float frame) {
		Keyframe2D begin = null;
		Keyframe2D end = null;
		
		for(int i = 0; i < this.keyframes.Length; i++) {
			Keyframe2D k = this.keyframes[i];
			if(k.frame <= frame) begin = k;
			if(k.frame > frame && end == null) end = k;
		}
		
		float p;
		
		if(end == null) {
			end = begin;
			p = 1;
		} else {
			p = (frame - begin.frame)/(end.frame - begin.frame);
		}
		
		float x = begin.x + (p * (end.x - begin.x));
		float y = begin.y + (p * (end.y - begin.y));
		this.target.localPosition = new Vector3(x, y, this.target.localPosition.z);
		
		x = begin.scaleX + (p * (end.scaleX - begin.scaleX));
		y = begin.scaleY + (p * (end.scaleY - begin.scaleY));
		this.target.localScale = new Vector3(x * this.initScaleX, y * this.initScaleY, this.target.localScale.z);
		
		// find shortest rotation direction, could be more optimized
		float r1 = begin.rotation;
		float r2 = end.rotation;
		
		while(r1 < 0f) r1 += 360f;
		while(r2 < 0f) r2 += 360f;
		
		float r3 = r2 - 360f;
		if(Mathf.Abs(r2 - r1) > Mathf.Abs(r3 - r1)) r2 = r3;
		r3 = r2 + 360f;
		if(Mathf.Abs(r2 - r1) > Mathf.Abs(r3 - r1)) r2 = r3;
		
		this.target.eulerAngles = new Vector3(this.target.eulerAngles.x, this.target.eulerAngles.y, r1 + (p * (r2 - r1)));
		
		if(mesh != null && (this.color.r != end.r || this.color.g != end.g || this.color.b != end.b || this.color.a != end.a)) {
			this.color=new Color(
				begin.r + (p * (end.r - begin.r)),
				begin.g + (p * (end.g - begin.g)),
				begin.b + (p * (end.b - begin.b)),
				begin.a + (p * (end.a - begin.a))
			);
			this.colors[0] = this.colors[1] = this.colors[2] = this.colors[3] = this.color;
			mesh.colors = this.colors;
		}
	}
}