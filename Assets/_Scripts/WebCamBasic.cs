using System;
using UnityEngine;
using UnityEngine.UI;


public class WebCamBasic : MonoBehaviour
    {
        public RawImage rawImage;
        private WebCamTexture webCamTexture;
        WebCamDevice[] devices;
        
        private void Start()
        {
            devices = WebCamTexture.devices;
            if (devices.Length > 0)
            {
                WebCamDevice device = devices[0]; // Use the first available camera
                webCamTexture = new WebCamTexture(device.name);
                rawImage.texture = webCamTexture;
                webCamTexture.Play();   
            }
        }
    }
