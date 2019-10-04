using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Profiling;

public class GameOfLife : MonoBehaviour
{
    [SerializeField] private RenderTexture playground;
    [SerializeField] private int queryX, queryY;
    [SerializeField] private bool result;

    private Texture2D playgroundPixels;
    private Rect playgroundRect;

    void Start()
    {
        Assert.AreEqual(RenderTextureFormat.ARGB32, playground.format);
        playgroundPixels = new Texture2D(playground.width, playground.height, TextureFormat.ARGB32, false, true);
        playgroundRect = new Rect(0, 0, playground.width, playground.height);
    }

    void OnDestroy()
    {
        Destroy(playgroundPixels);
    }

    void Update()
    {
        Profiler.BeginSample("SetActiveRenderTexture");
        var prevActive = RenderTexture.active;
        RenderTexture.active = playground;
        Profiler.EndSample();

        Profiler.BeginSample("ReadPixels");
        playgroundPixels.ReadPixels(playgroundRect, 0, 0, false);
        playgroundPixels.Apply();
        Profiler.EndSample();

        Profiler.BeginSample("RestoreActiveRenderTexture");
        RenderTexture.active = prevActive;
        Profiler.EndSample();

        Profiler.BeginSample("Query");
        var pixel = playgroundPixels.GetPixel(queryX, queryY);
        result = (pixel == Color.red);
        Profiler.EndSample();
    }
}
