using System;
using UnityEngine;
using UnityEngine.UI;

public class CamImage : MonoBehaviour
{
    [SerializeField] RawImage _rawImage;
    [SerializeField] CameraCapture _cameraCapture;
    private void Start()
    {
        _rawImage.texture = _cameraCapture.camTexture;
    }
}
