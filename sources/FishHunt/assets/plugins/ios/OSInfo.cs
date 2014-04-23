using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class OSInfo
{
	#if UNITY_IPHONE
	[DllImport("__Internal")]
	private static extern int _GetMajorOsVersion();
	[DllImport("__Internal")]
	private static extern int _GetMinorOsVersion();
	#endif
	
	protected static int minorOsVersion = -1;
	protected static int majorOsVersion = -1;
	
    public static int MajorOsVersion 
	{
        get 
		{
			if (majorOsVersion == -1)
			{
				#if UNITY_IPHONE
		            #if !UNITY_EDITOR
		                majorOsVersion = _GetMajorOsVersion();
		            #endif
	        	#endif
			}
					
            return majorOsVersion;
        }
    }
	
	public static int MinorOsVersion 
	{
        get 
		{
			if (minorOsVersion == -1)
			{
				#if UNITY_IPHONE
		            #if !UNITY_EDITOR
		                minorOsVersion = _GetMinorOsVersion();
		            #endif
	        	#endif
			}
					
            return minorOsVersion;
        }
    }
	
	#if UNITY_IPHONE
	[DllImport("__Internal")]
	private static extern string _GetCFBundleVersion();
	#endif

   
    public static string BundleVersion 
	{
        get 
		{
            if (m_bundleVersion == null) 
			{
                GetBundleVersion();
            }
            return m_bundleVersion;
        }
    }

    protected static string m_bundleVersion;

    protected static void GetBundleVersion() 
	{
		m_bundleVersion = "N/A";
        #if UNITY_IPHONE
            #if UNITY_EDITOR
                m_bundleVersion = PlayerSettings.bundleVersion;
            #else
                m_bundleVersion = _GetCFBundleVersion();
            #endif
        #endif
    }
	
	#if UNITY_IPHONE
	[DllImport("__Internal")]
	private static extern string _GetGUID();
	#endif
	
	protected static string guid = null;
	public static string GetGUID()
	{
		if (guid == null)
		{
			#if UNITY_IPHONE
	            #if UNITY_EDITOR
	                guid = SystemInfo.deviceUniqueIdentifier;
	            #else
	                guid = _GetGUID();
	            #endif
        	#endif
		}
		return guid;
	}
}
