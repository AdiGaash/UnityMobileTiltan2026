using UnityEngine;
using UnityEditor;

public static class PlayerDataUtility
{
    // Asset location inside project
    private const string FolderPath =
        "Assets/Resources/GameData";

    private const string AssetPath =
        FolderPath + "/PlayerData.asset";

    
    [MenuItem("Tools/Create Player Data")]
    public static PlayerData GetOrCreatePlayerData()
    {
        // Try loading existing asset
        PlayerData data =
            AssetDatabase.LoadAssetAtPath<PlayerData>(AssetPath);

        // If asset already exists
        if (data != null)
        {
            Debug.Log("PlayerData found!");

            return data;
        }

        Debug.Log("PlayerData not found. Creating new asset.");

        // Create folder structure if missing
        CreateFoldersIfNeeded();

        // Create new ScriptableObject instance
        data = ScriptableObject.CreateInstance<PlayerData>();

        // Default values
        data.playerName = "New Player";
        data.health = 100;
        data.coins = 0;

        // Save asset into project
        AssetDatabase.CreateAsset(data, AssetPath);

        AssetDatabase.SaveAssets();

        AssetDatabase.Refresh();

        Debug.Log("PlayerData asset created!");

        return data;
    }

    private static void CreateFoldersIfNeeded()
    {
        // Create Resources folder if missing
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
        {
            AssetDatabase.CreateFolder("Assets", "Resources");
        }

        // Create GameData folder if missing
        if (!AssetDatabase.IsValidFolder(FolderPath))
        {
            AssetDatabase.CreateFolder(
                "Assets/Resources",
                "GameData");
        }
    }
}