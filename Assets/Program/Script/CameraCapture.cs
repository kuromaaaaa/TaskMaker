using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CameraCapture : MonoBehaviour
{
    public WebCamTexture camTexture;

    [SerializeField] RawImage _rawImage;
    [SerializeField] private TaskFlowController _taskFlowController;
    IEnumerator Start()
    {
        // カメラ権限
        if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        }

        camTexture = new WebCamTexture(1280, 720);
        _rawImage.texture = camTexture;
        camTexture.Play();
    }

    public Texture2D Capture()
    {
        Texture2D tex = new Texture2D(
            camTexture.width,
            camTexture.height,
            TextureFormat.RGB24,
            false
        );

        tex.SetPixels(camTexture.GetPixels());
        tex.Apply();

        return tex;
    }
    

    public void OnCaptureButton()
    {
        Texture2D tex = Capture();
        tex= ResizeTexture(tex, 1024);
        _taskFlowController.Flow(tex);
    }
    
    private Texture2D ResizeTexture(Texture2D source, int targetWidth)
    {
        float ratio = (float)targetWidth / source.width;
        int targetHeight = Mathf.RoundToInt(source.height * ratio);

        RenderTexture rt = RenderTexture.GetTemporary(targetWidth, targetHeight);
        Graphics.Blit(source, rt);

        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D resized = new Texture2D(targetWidth, targetHeight);
        resized.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
        resized.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(rt);

        return resized;
    }
}