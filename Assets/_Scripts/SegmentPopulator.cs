using UnityEngine;

public class SegmentPopulator : MonoBehaviour
{
    public GameObject subPrefab; // The prefab for the sub-object
    public Transform[] faceTransforms; // Array of references to the four faces, each with six children

  

    public void AttachSubObjects()
    {
        foreach (Transform face in faceTransforms)
        {
            for (int i = 0; i < face.childCount; i++) // Assuming six positions per face
            {
                Transform positionTransform = face.GetChild(i);
                GameObject subObject = Instantiate(subPrefab, positionTransform.position, positionTransform.rotation, positionTransform);
                subObject.transform.parent = positionTransform;
            }
        }
    }
}