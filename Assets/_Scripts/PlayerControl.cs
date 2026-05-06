using UnityEngine;


    public class PlayerControl : MonoBehaviour
    {
        
        Transform playerTransform;
         
        void Awake()
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
        
        public void MoveUp()
        {
            playerTransform.position += Vector3.up;
        }
    }
