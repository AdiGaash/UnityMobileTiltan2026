using UnityEngine;

public class SegmentPopulator : MonoBehaviour
{
    public Transform[] faceTransforms; // Array of references to the four faces, each with six children
  
    
     
  

    public void AttachSubObjects()
    {
        foreach (Transform face in faceTransforms)
        {
            for (int i = 0; i < face.childCount-1; i++) // Assuming six positions per face
            {
                Transform positionTransform = face.GetChild(i);
                GameObject subObject = TaggedObjectPooler.Instance.GetPooledObject("ladders"); // Get a sub-object from the pool
                subObject.transform.parent = positionTransform;
                subObject.transform.localPosition = Vector3.zero; // Reset local position to align with the parent
            }
        }
    }

    public void AttachLaddersToFace(int faceNum, bool[] laddersSlots)
    {
        Transform face = faceTransforms[faceNum];

        for (int i = 0; i < face.childCount-1; i++) // Assuming six positions per face
        {
            if (laddersSlots[i])
            {
                Transform positionTransform = face.GetChild(i);
                GameObject subObject = TaggedObjectPooler.Instance.GetPooledObject("Ladders");

                subObject.transform.parent = positionTransform;
                subObject.transform.localPosition = Vector3.zero; // Reset local position to align with the parent
            }
        }
        
    }

    
    public void ClearSubObjects()
    {
        foreach (Transform face in faceTransforms)
        {
            for (int i = 0; i < face.childCount; i++)
            {
                Transform positionTransform = face.GetChild(i);
                if (positionTransform.childCount > 0)
                {
                    GameObject subObject = positionTransform.GetChild(0).gameObject; // Assuming one sub-object per position
                    TaggedObjectPooler.Instance.ReturnObject(subObject, "ladders"); // Return the sub-object to the pool
                }
            }
        }
        
    }
}