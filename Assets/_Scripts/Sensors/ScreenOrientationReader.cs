using UnityEngine;

public class ScreenOrientationReader : MonoBehaviour
{
    private ScreenOrientation _previousOrientation;

    private void Start()
    {
        _previousOrientation = Screen.orientation;
        Debug.Log("Screen orientation initialized: " + _previousOrientation);
    }

    private void Update()
    {
        ScreenOrientation currentOrientation = Screen.orientation;

        if (currentOrientation != _previousOrientation)
        {
            Debug.Log("SCREEN ORIENTATION CHANGED: " + _previousOrientation + " → " + currentOrientation);
            _previousOrientation = currentOrientation;
        }
    }
}