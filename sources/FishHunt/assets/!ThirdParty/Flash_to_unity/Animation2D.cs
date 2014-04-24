using UnityEngine;
using System.Collections;

public class Animation2D : MonoBehaviour {

	public Element2D [] elements;
	public Material material;
	
	private bool _isLooping = false;
	private string [] labels;
	private Hashtable labelsLookup;
	private int count = 0;
	private bool _isPlaying = false;
	private int framerate;
	private int frames;
	private float startTime;
	private int fadeAmount;
	private int fade;
	private int firstFrame;
	private int lastFrame;
	private int totalFrames;
	
	public bool isPlaying {
		get {
			return this._isPlaying;
		}
	}
	
	public bool isLooping {
		get {
			return this._isLooping;
		}
	}
	
	public void Initialize(int framerate, int frames, int elementcount) {
		this.framerate = framerate;
		this.frames = frames;
		this.elements = new Element2D[elementcount];
		this.labelsLookup = new Hashtable();
		this.labels = new string[frames];
	}
	
	public void AddElement(Element2D element) {
		this.elements[count] = element;
		count++;
	}
	
	public void AddLabel(int frame, string name) {
		this.labelsLookup[name] = frame;
		this.labels[frame] = name;
	}
	
	public void Play() {
		this.Play(0, this.frames - 1, true, 0);
	}
	
	public void Play(int fade) {
		this.Play(0, this.frames - 1, true, fade);
	}
	
	public void Play(bool loop) {
		this.Play(0, this.frames - 1, loop, 0);
	}
	
	public void Play(bool loop, int fade) {
		this.Play(0, this.frames - 1, loop, fade);
	}
	
	public void Play(string start) {
		this.Play(start, true, 0);
	}
	
	public void Play(string start, string end) {
		this.Play(start, end, true, 0);
	}
	
	public void Play(string start, bool loop) {
		this.Play(start, loop, 0);
	}
	
	public void Play(string start, string end, bool loop) {
		this.Play(start, end, loop, 0);
	}
	
	public void Play(string start, int fade) {
		this.Play(start, true, fade);
	}
	
	public void Play(string start, string end, int fade) {
		this.Play(start, end, true, fade);
	}
	
	public void Play(string start, bool loop, int fade) {
		this.Play((int)this.labelsLookup[start], loop, fade);
	}
	
	public void Play(string start, string end, bool loop, int fade) {
		this.Play((int)this.labelsLookup[start], (int)this.labelsLookup[end] - 1, loop, fade);
	}
	
	public void Play(int start, int end) {
		this.Play(start, end, true, 0);
	}
	
	public void Play(int start, bool loop) {
		this.Play(start, loop,0);
	}
	
	public void Play(int start, int end, bool loop) {
		this.Play(start, end, loop, 0);
	}
	
	public void Play(int start, int end, int fade) {
		this.Play(start, end, true, fade);
	}
	
	public void Play(int start, bool loop, int fade) {
		int end = this.frames - 1;
		for(int i = start + 1; i < this.labels.Length; i++) {
			if(this.labels[i] != null && this.labels[i] != "") {
				end = i - 1;
			}
		}
		this.Play(start, end, loop, fade);
	}
	
	public void Play(int start, int end, bool loop, int fade) {
		if(end < start) end = start;
		this.firstFrame = start;
		this.lastFrame = end;
		this.totalFrames = end - start;
		this.startTime = Time.realtimeSinceStartup;
		this._isPlaying = true;
		this._isLooping = loop;
		this.fade = fade;
		for(int i = 0; i < this.elements.Length; i++) {
			if(this.fade > 0) {
				this.elements[i].StartFade(this.firstFrame);
			}
		}
		this.Update();
	}
	
	public void Stop() {
		this._isPlaying = false;
	}
	
	public void Update() {
		if(this._isPlaying) {
			float time = Time.realtimeSinceStartup - this.startTime;
			float frame = this.framerate * time;
			
			if(frame < this.fade) {
				float p = frame/this.fade;
				for(int i = 0; i < this.elements.Length; i++) {
					this.elements[i].Fade(p);
				}
				return;
			}
			frame -= this.fade;
			frame += this.firstFrame;
			if(this.isLooping) {
				if(this.lastFrame == this.firstFrame) frame = this.firstFrame;
				while(frame > this.lastFrame) frame -= this.totalFrames;
			} else {
				if(frame > this.lastFrame) {
					this.Stop();
					return;
				}
			}
			for(int i = 0; i < this.elements.Length; i++) {
				this.elements[i].Update(frame);
			}
		}
	}
	
	public void Pose(string label) {
		Pose((int)this.labelsLookup[label]);
	}
	
	public void Pose(int frame) {
		for(int i = 0; i < this.elements.Length; i++) {
			this.elements[i].Update(frame);
		}
	}
	
}
