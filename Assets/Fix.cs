#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Management;

// Prevents Meta XR simulator crashes when restarting Play Mode
[InitializeOnLoad]
public static class XRLoaderReset
{
    static XRLoaderReset()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (XRGeneralSettings.Instance?.Manager == null)
            return;

        var manager = XRGeneralSettings.Instance.Manager;

        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            if (manager.activeLoader != null)
            {
                Debug.Log("Deinitializing XR Loader safely...");
                manager.DeinitializeLoader();
            }
        }

        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            if (manager.activeLoader == null)
            {
                Debug.Log("Reinitializing XR Loader safely...");
                manager.InitializeLoaderSync();
                manager.StartSubsystems();
            }
        }
    }
}
#endif
