using System;
using System.Collections.Generic;
using UnityEngine;

#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

public enum AndroidPermission
{
    Camera,
    Microphone,
    FineLocation,
    CoarseLocation,
    ReadImages,       // Android 13+ (API 33+)
    ReadVideo,        // Android 13+ (API 33+)
    ReadAudio,        // Android 13+ (API 33+)
    ReadExternalStorage,  // Android 12 and below
    WriteExternalStorage  // Android 12 and below
}

public class AndroidPermissionManager : MonoBehaviour
{
    // Dictionary mapping the C# Enum values to the native Android OS permission strings 
    // notes that those are common ones but there are others...
    private static readonly Dictionary<AndroidPermission, string> NativePermissionMap = new Dictionary<AndroidPermission, string>
    {
        { AndroidPermission.Camera, "android.permission.CAMERA" },
        { AndroidPermission.Microphone, "android.permission.RECORD_AUDIO" },
        { AndroidPermission.FineLocation, "android.permission.ACCESS_FINE_LOCATION" },
        { AndroidPermission.CoarseLocation, "android.permission.ACCESS_COARSE_LOCATION" },
        { AndroidPermission.ReadImages, "android.permission.READ_MEDIA_IMAGES" },
        { AndroidPermission.ReadVideo, "android.permission.READ_MEDIA_VIDEO" },
        { AndroidPermission.ReadAudio, "android.permission.READ_MEDIA_AUDIO" },
        { AndroidPermission.ReadExternalStorage, "android.permission.READ_EXTERNAL_STORAGE" },
        { AndroidPermission.WriteExternalStorage, "android.permission.WRITE_EXTERNAL_STORAGE" }
    };

    private Dictionary<AndroidPermission, bool> batchResultMap = new Dictionary<AndroidPermission, bool>();

    /// <summary>
    /// Translates an enum selection into its corresponding native Android string identifier.
    /// </summary>
    private string GetNativeString(AndroidPermission permission)
    {
        if (NativePermissionMap.TryGetValue(permission, out string nativeString))
        {
            return nativeString;
        }
        throw new ArgumentOutOfRangeException(nameof(permission), $"Permission enum value not mapped: {permission}");
    }

    /// <summary>
    /// Checks if a single specific enum permission has already been authorized.
    /// </summary>
    public bool IsPermissionGranted(AndroidPermission permission)
    {
#if PLATFORM_ANDROID && !UNITY_EDITOR
        return Permission.HasUserAuthorizedPermission(GetNativeString(permission));
#else
        return true; 
#endif
    }

    /// <summary>
    /// Requests a batch list of permissions sequentially based on your selected enum array.
    /// </summary>
    public void RequestPermissionBatch(AndroidPermission[] permissions, Action<Dictionary<AndroidPermission, bool>> onBatchComplete)
    {
        batchResultMap.Clear();
#if PLATFORM_ANDROID && !UNITY_EDITOR
        Queue<AndroidPermission> permissionQueue = new Queue<AndroidPermission>(permissions);
        ProcessPermissionQueue(permissionQueue, onBatchComplete);
#else
        foreach (var perm in permissions)
        {
            batchResultMap[perm] = true;
        }
        onBatchComplete?.Invoke(batchResultMap);
#endif
    }

    private void ProcessPermissionQueue(Queue<AndroidPermission> queue, Action<Dictionary<AndroidPermission, bool>> onComplete)
    {
#if PLATFORM_ANDROID && !UNITY_EDITOR
        if (queue.Count == 0)
        {
            onComplete?.Invoke(batchResultMap);
            return;
        }

        AndroidPermission currentEnumPermission = queue.Dequeue();
        string nativePermissionString = GetNativeString(currentEnumPermission);

        if (Permission.HasUserAuthorizedPermission(nativePermissionString))
        {
            batchResultMap[currentEnumPermission] = true;
            ProcessPermissionQueue(queue, onComplete);
            return;
        }

        var callbacks = new PermissionCallbacks();
        callbacks.PermissionGranted += (name) => {
            batchResultMap[currentEnumPermission] = true;
            ProcessPermissionQueue(queue, onComplete);
        };
        callbacks.PermissionDenied += (name) => {
            batchResultMap[currentEnumPermission] = false;
            ProcessPermissionQueue(queue, onComplete);
        };
        callbacks.PermissionDeniedAndDontAskAgain += (name) => {
            batchResultMap[currentEnumPermission] = false;
            ProcessPermissionQueue(queue, onComplete);
        };

        Permission.RequestUserPermission(nativePermissionString, callbacks);
#endif
    }
}