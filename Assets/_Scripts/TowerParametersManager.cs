
using UnityEngine;
using System.Collections.Generic;


    [CreateAssetMenu(fileName = "TowerParametersManager", menuName = "GameSO/TowerParametersManager", order = 1)]
    public class TowerParametersManager : ScriptableObject
    {
        [System.Serializable]
        public class DifficultyLevel
        {
            [Header("Difficulty Settings")]
            public string difficultyName = "Easy";
            public int segmentsRequired = 10; // Number of segments before moving to next difficulty
            
            [Header("Tower Parameters Reference")]
            public TowerParameters towerParameters;
        }
        
        [Header("Difficulty Progression")]
        public List<DifficultyLevel> difficultyLevels = new List<DifficultyLevel>();
        
        [Header("Current State")]
        [SerializeField] private int currentLevelIndex = 0;
        [SerializeField] private int segmentsGenerated = 0;
        
        public TowerParameters CurrentTowerParameters => 
            difficultyLevels.Count > 0 && currentLevelIndex < difficultyLevels.Count 
                ? difficultyLevels[currentLevelIndex].towerParameters 
                : null;
        
        public DifficultyLevel CurrentDifficultyLevel =>
            difficultyLevels.Count > 0 && currentLevelIndex < difficultyLevels.Count 
                ? difficultyLevels[currentLevelIndex] 
                : null;
        
        public int CurrentDifficultyIndex => currentLevelIndex;
        public string CurrentDifficultyName => CurrentDifficultyLevel?.difficultyName ?? "Unknown";
        
        /// <summary>
        /// Called when a new segment is generated. Returns true if difficulty changed.
        /// </summary>
        public bool OnSegmentGenerated()
        {
            if (CurrentDifficultyLevel == null) return false;
            
            segmentsGenerated++;
            
            // Check if we should advance to the next difficulty
            if (segmentsGenerated >= CurrentDifficultyLevel.segmentsRequired)
            {
                return AdvanceToNextDifficulty();
            }
            
            return false;
        }
        
        /// <summary>
        /// Advances to the next difficulty level. Returns true if successful.
        /// </summary>
        public bool AdvanceToNextDifficulty()
        {
            if (currentLevelIndex + 1 < difficultyLevels.Count)
            {
                currentLevelIndex++;
                segmentsGenerated = 0;
                Debug.Log($"Difficulty advanced to: {CurrentDifficultyName}");
                return true;
            }
            
            // Stay at maximum difficulty
            segmentsGenerated = 0;
            return false;
        }
        
        /// <summary>
        /// Resets to the first difficulty level.
        /// </summary>
        public void ResetDifficulty()
        {
            currentLevelIndex = 0;
            segmentsGenerated = 0;
        }
        
        /// <summary>
        /// Gets progress towards next difficulty (0.0 to 1.0)
        /// </summary>
        public float GetProgressToNextDifficulty()
        {
            if (CurrentDifficultyLevel == null) return 1.0f;
            return Mathf.Clamp01((float)segmentsGenerated / CurrentDifficultyLevel.segmentsRequired);
        }
        
        /// <summary>
        /// Forces a specific difficulty level
        /// </summary>
        public void SetDifficulty(int difficultyIndex)
        {
            if (difficultyIndex >= 0 && difficultyIndex < difficultyLevels.Count)
            {
                currentLevelIndex = difficultyIndex;
                segmentsGenerated = 0;
            }
        }
        
        /// <summary>
        /// Gets remaining segments needed for next difficulty
        /// </summary>
        public int GetSegmentsUntilNextDifficulty()
        {
            if (CurrentDifficultyLevel == null) return 0;
            return Mathf.Max(0, CurrentDifficultyLevel.segmentsRequired - segmentsGenerated);
        }
    }
