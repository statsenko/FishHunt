using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class AudioManager : MonoBehaviour 
{

	/*
	public static string intro = "Intro";
	public static string main_theme = "chefstory_main_theme";
	public static string gameplay_theme = "chefstory_gameplay";

	public static string lottery_sector_sfx = "lottery_sector";
	public static string button_click_sfx = "button_click";
	public static string lobby_panel_open_sfx = "window_open";
	public static string money_operation_sfx = "money_operation";
	public static string alert_appear_sfx = "window_appear";
	public static string recipe_notice_sfx = "recipe_notice";
	public static string friend_notice_sfx = "friend_notice";
	public static string build_notice_sfx = "build_notice";
	public static string recipe_cooking_begin_sfx = "recipe_begin";
	public static string recipe_profit_grab_sfx = "recipe_end";
	public static string recipe_mastered_sfx = "craft_end";
	public static string level_up_sfx = "level_up";
	public static string exp_money_count_sfx = "exp_money_loop";
	
	protected static AudioManager mInstance = null;
	
	public static AudioManager Instance
	{
		get 
		{
			if (mInstance == null)
			{
                mInstance = new GameObject("_AudioManager").AddComponent(typeof(AudioManager)) as AudioManager;
				DontDestroyOnLoad(mInstance.gameObject);
				mInstance.sfxVolume_ = GameConstants.defaultSfxVolume;
				mInstance.musicVolume_ = GameConstants.defaultMusicVolume;
				
				mInstance.Load();
				mInstance.gameObject.AddComponent(typeof(AudioListener));
				
				AudioSource mainThemeAudio = mInstance.gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
				mainThemeAudio.loop = true;
				mainThemeAudio.playOnAwake = false;
				mInstance.audioSources = new List<AudioSource>();
				
            }
            return mInstance;
		}
	}
	
	public static float SfxVolume
	{
		get {return Instance.sfxVolume_;}
		set 
		{
			Instance.sfxVolume_ = value;
			foreach (AudioSource audioSrc in Instance.audioSources)
			{
				audioSrc.volume = value;
			}
			
			Instance.Save();
		}
	}
	protected float sfxVolume_ = GameConstants.defaultSfxVolume;
	
	public static float MusicVolume
	{
		get {return Instance.musicVolume_;}
		set 
		{
			Instance.musicVolume_ = value;
			Instance.audio.volume = value;
			Instance.Save();
		}
	}
	protected float musicVolume_ = GameConstants.defaultMusicVolume;
		
	public static void PlaySfxClip(AudioClip clip, bool destroyOnLoad = true)
	{
		Instance.PlaySfxClip_(clip, destroyOnLoad);
	}
	
	public static void PlaySfxClip(string assetName, bool destroyOnLoad = true)
	{
		AudioClip clip = Resources.Load(assetName, typeof(AudioClip)) as AudioClip;
		if (clip == null)
		{
			Debug.LogWarning("WARNING: Audio asset not loaded: " + assetName);
			return;
		}
		
		Instance.PlaySfxClip_(clip, destroyOnLoad);
	}
	
	protected void PlaySfxClip_(AudioClip clip, bool destroyOnLoad = true)
	{
		if (clip == null)
			return;
		
		if (destroyOnLoad)
		{
			AudioSource.PlayClipAtPoint(clip, transform.position, SfxVolume);
		}
		else
		{
			if (GetAudioSourceWithClip(clip) == null)
			{
				GameObject srcGO = new GameObject("_SFX_"+clip.name);
				srcGO.transform.parent = transform;
				AudioSource src = CreateAudioSource_(clip, srcGO);
				src.volume = SfxVolume;
				src.loop = false;
				src.Play();
				Destroy(srcGO, clip.length);
			}
		}
	}
	
	protected List<AudioSource> audioSources = null;
	public static AudioSource CreateAudioSource(AudioClip clip, GameObject parent = null)
	{
		return Instance.CreateAudioSource_(clip, parent);
	}
	public static AudioSource CreateAudioSource(string assetName, GameObject parent = null)
	{
		AudioClip clip = Resources.Load(assetName, typeof(AudioClip)) as AudioClip;
		if (clip == null)
		{
			Debug.LogWarning("WARNING: Audio asset not loaded: " + assetName);
			return null;
		}
		
		return Instance.CreateAudioSource_(clip, parent);
	}
	protected AudioSource CreateAudioSource_(AudioClip clip, GameObject parent = null)
	{
		if (clip == null)
			return null;
		
		if (parent == null)
		{
			parent = new GameObject("_SFX_"+clip.name);
		}
		
		AudioSource audioSrc = parent.audio;
		if (audioSrc == null)
		{
			audioSrc = parent.AddComponent(typeof(AudioSource)) as AudioSource;
		}
		
		if (!audioSources.Contains(audioSrc))
			audioSources.Add(audioSrc);
		
		DestroyListner destroyListner = parent.GetComponent<DestroyListner>();
		if (destroyListner == null)
			destroyListner = parent.AddComponent(typeof(DestroyListner)) as DestroyListner;
		destroyListner.onDestroyCallback += OnAudioSourceDestroyed;
		
		audioSrc.Stop();
		audioSrc.volume = SfxVolume;
		audioSrc.clip = clip;
		
		return audioSrc;
	}
	protected AudioSource GetAudioSourceWithClip(AudioClip clip)
	{
		return audioSources.SingleOrDefault (source => string.Equals (source.clip.name, clip.name) && source.isPlaying && source.time < 0.25f);
	}

	public void OnAudioSourceDestroyed(GameObject sender)
	{
		AudioSource audioSrc = sender.GetComponent<AudioSource>();
		if (audioSrc)
			audioSources.Remove(audioSrc);
	}
	public static void PlayBackgroundMusic(AudioClip clip, bool fade = true, bool loop = true)
	{
		if (clip == null)
		{
			Debug.LogWarning("WARNING: Audio clip not loaded");
			return;
		}
		
		Instance.PlayBackgroundMusic_(clip, fade, loop);
	}
	public static void PlayBackgroundMusic(string assetName, bool fade = true, bool loop = true)
	{
		AudioClip clip = Resources.Load(assetName, typeof(AudioClip)) as AudioClip;
		PlayBackgroundMusic(clip, fade, loop);
	}
	protected void PlayBackgroundMusic_(AudioClip clip, bool fade = true, bool loop = true)
	{
		StopCoroutine("fadeMainTheme");
		StopCoroutine("FadeOut");
		StopCoroutine("FadeIn");
		
		audio.loop = loop;
		
		if (fade)
			StartCoroutine("fadeMainTheme", clip);
		else
		{
			audio.clip = clip;
			audio.Play();
		}
		
		
	}
	
	public static void StopBackgroundMusic(bool fade = true)
	{
		Instance.StopBackgroundMusic_(fade);
	}
	
	protected void StopBackgroundMusic_(bool fade = true)
	{
		StopCoroutine("fadeMainTheme");
		StopCoroutine("FadeOut");
		StopCoroutine("FadeIn");
		
		if (fade)
		{
			StartCoroutine("FadeOut");
		}
		else
		{
			audio.volume = 0.0f;
			audio.Stop();
		}
	}
	
	public static float fadeRate = 3f;
	IEnumerator fadeMainTheme( AudioClip clip) 
	{
 		if(clip == null)
  		{
    		yield break;
  		}
		
		if (audio.clip != null)
  			yield return StartCoroutine("FadeOut");
		else
			audio.volume = 0f;

 		audio.clip = clip;
 		audio.Play();

  		yield return StartCoroutine("FadeIn");
	}

	IEnumerator FadeOut()
	{
  		while( audio.volume > 0.1f )
		{
			audio.volume = Mathf.Lerp( audio.volume, 0.0f, fadeRate * Time.deltaTime );
			yield return null;
  		}
		audio.volume = 0.0f;
		audio.Stop();
	}

	IEnumerator FadeIn()
	{
		while( audio.volume < musicVolume_ )
		{
			audio.volume = Mathf.Lerp( audio.volume, musicVolume_, fadeRate * Time.deltaTime);
    		yield return null;
  		}
		audio.volume = musicVolume_;
	}
	
	
	void OnDestroy()
	{
		DestroyListner.RemoveCallback(OnAudioSourceDestroyed);
		Save();
		mInstance = null;
	}
	
	protected void Load()
	{
		sfxVolume_ = PlayerPrefs.GetFloat("AudioManager_SfxVolume", GameConstants.defaultSfxVolume);
		musicVolume_ = PlayerPrefs.GetFloat("AudioManager_MusicVolume", GameConstants.defaultMusicVolume);
	}
	
	protected void Save()
	{
		PlayerPrefs.SetFloat("AudioManager_SfxVolume", sfxVolume_);
		PlayerPrefs.SetFloat("AudioManager_MusicVolume", musicVolume_);
	}
	*/
}
