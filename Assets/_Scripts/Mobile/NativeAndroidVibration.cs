using UnityEngine;

#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

// Add permission to your AndroidManifest.xml: 
// <uses-permission android:name="android.permission.VIBRATE"/>

public static class NativeAndroidVibration
{
    private static AndroidJavaObject vibrator = null;
    private static AndroidJavaClass vibrationEffectClass = null;
    private const string VibratePermission = "android.permission.VIBRATE";
    private static int apiLevel = 0;

    static NativeAndroidVibration()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        EnsurePermission();

        try
        {
            // Get current Android SDK API level
            using (AndroidJavaClass versionClass = new AndroidJavaClass("android.os.Build$VERSION"))
            {
                apiLevel = versionClass.GetStatic<int>("SDK_INT");
            }

            // Initialize standard Vibrator system service
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
            }

            // Initialize VibrationEffect class if running Android 8.0 (API 26) or higher
            if (apiLevel >= 26)
            {
                vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to initialize Android Vibrator JNI: {ex.Message}");
        }
#endif
    }

    private static void EnsurePermission()
    {
#if PLATFORM_ANDROID && !UNITY_EDITOR
        if (!Permission.HasUserAuthorizedPermission(VibratePermission))
        {
            Permission.RequestUserPermission(VibratePermission);
        }
#endif
    }

    /// <summary>
    /// Simple legacy one-shot vibration.
    /// </summary>
    public static void Vibrate(long milliseconds)
    {
        if (vibrator == null) return;

#if UNITY_ANDROID && !UNITY_EDITOR
        if (apiLevel >= 26 && vibrationEffectClass != null)
        {
            // On API 26+, standard "vibrate(long)" is deprecated. Use VibrationEffect.createOneShot
            // DEFAULT_AMPLITUDE = -1
            AndroidJavaObject effect = vibrationEffectClass.CallStatic<AndroidJavaObject>("createOneShot", milliseconds, -1);
            vibrator.Call("vibrate", effect);
        }
        else
        {
            vibrator.Call("vibrate", milliseconds);
        }
#endif
    }

    /// <summary>
    /// Advanced rhythmic haptic sequence using paired timings and custom power ranges.
    /// </summary>
    /// <param name="timings">Durations in milliseconds for each pattern step.</param>
    /// <param name="amplitudes">Motor force levels (0 to 255) for each timing step.</param>
    /// <param name="repeat">Array index element to loop back to. Use -1 to play exactly once.</param>
    public static void VibrateWaveform(long[] timings, int[] amplitudes, int repeat = -1)
    {
        if (vibrator == null) return;

#if UNITY_ANDROID && !UNITY_EDITOR
        if (apiLevel >= 26 && vibrationEffectClass != null)
        {
            AndroidJavaObject effect = vibrationEffectClass.CallStatic<AndroidJavaObject>("createWaveform", timings, amplitudes, repeat);
            vibrator.Call("vibrate", effect);
        }
        else
        {
            // Legacy fallback ignores amplitude arrays entirely
            vibrator.Call("vibrate", timings, repeat);
        }
#endif
    }

    /// <summary>
    /// Explicitly halts any ongoing single or looping vibration sequences.
    /// </summary>
    public static void Cancel()
    {
        if (vibrator != null)
        {
            vibrator.Call("cancel");
        }
    }
}