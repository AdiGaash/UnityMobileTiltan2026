using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildAutomation : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        // Example check: Ensure we have min scene in the build
        if (EditorBuildSettings.scenes.Length == 0)
        {
            throw new BuildFailedException("Build aborted: No scenes added to Build Settings.");
        }
        
       
        BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
        if(buildTarget == BuildTarget.Android)
        {
            UpdateVersionData();
        }
        
        var scriptingDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildPipeline.GetBuildTargetGroup(buildTarget));
        if (scriptingDefineSymbols.Contains("Do_This_Before_Build"))
        {
            
        }
        
        
        
        
        // assuming you update assets in the process you will need to save the status and refresh ...
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        
        
        
    }

    void UpdateVersionData()
    {
        string currentVersion = PlayerSettings.bundleVersion;
        string[] versionParts = currentVersion.Split('.');
        int lastDigit = versionParts.Length > 0 ? 
            int.Parse(versionParts[versionParts.Length - 1]) : -1;

        // Increment the last digit
        lastDigit++;

        // Reconstruct the version string with the incremented last digit
        versionParts[versionParts.Length - 1] = lastDigit.ToString();
        string newVersionStr = string.Join(".", versionParts);

        PlayerSettings.bundleVersion = newVersionStr; // e.g., "1.0.4"
        PlayerSettings.Android.bundleVersionCode++;
        
        Debug.Log($"Updated version to {newVersionStr} and incremented Android bundle version code to {PlayerSettings.Android.bundleVersionCode}");
        
    }
}