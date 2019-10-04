using UnityEngine;
using UnityEngine.Profiling;

public sealed class Playground : MonoBehaviour, IPlayground
{
    public enum Direction
    {
        Up,
        Right,
        Down,
        Left,
    }

    [SerializeField] private RenderTexture renderTexture;
    [SerializeField] private Material material;

    [SerializeField] private Material explosionMaterial;
    private Material explosionMaterialInstance;

    private Texture2D playground;
    private SnakeController controller1, controller2;
    private Snake snake1, snake2;
    private Color32[] colors;

    void Start()
    {
        int w = renderTexture.width;
        int h = renderTexture.height;
        playground = new Texture2D(w, h, TextureFormat.ARGB32, false, true);
        material.mainTexture = playground;

        colors = new Color32[w * h];
        playground.SetPixels32(colors);

        controller1 = new SnakeController("Up1", "Down1", "Left1", "Right1", "Jump1");
        controller2 = new SnakeController("Up2", "Down2", "Left2", "Right2", "Jump2");
        snake1 = new Snake( w / 4 * 1, h / 8, Snake.Direction.Up, Color.red,  controller1);
        snake2 = new Snake( w / 4 * 3, h / 8, Snake.Direction.Up, Color.blue, controller2);

        explosionMaterialInstance = new Material(explosionMaterial);

        InvokeRepeating("SpawnExplosion", Random.value, 5f);
    }

    int explodeX, explodeY;
    int explodeRadius = 50;

    void SpawnExplosion()
    {
        explodeX = Random.Range(0, 256);
        explodeY = Random.Range(0, 256);
        explodeRadius = 3;

        explosionMaterialInstance.SetFloat("_PositionX", (float)explodeX / 256);
        explosionMaterialInstance.SetFloat("_PositionY", (float)explodeY / 256);
    }

    void HandleExplosion()
    {
        if (explodeRadius >= 30)
            return;

        // draw explosion
        explosionMaterialInstance.SetFloat("_Radius", (float)explodeRadius / 256);
        explosionMaterialInstance.SetFloat("_InnerRadius", (float)(explodeRadius -1) / 256);
        Graphics.Blit(playground, renderTexture, explosionMaterialInstance);

        // copy back
        Profiler.BeginSample("ReadPixels");
        var prevActiveRenderTexture = RenderTexture.active;
        RenderTexture.active = renderTexture;
        playground.ReadPixels(new Rect(0, 0, 256, 256), 0, 0);
        RenderTexture.active = prevActiveRenderTexture;
        Profiler.EndSample();

        Profiler.BeginSample("SetPixels");
        playground.SetPixels32(colors);
        Profiler.EndSample();
        explodeRadius++;
    }

    void OnDestroy()
    {
        material.mainTexture = null;
        Destroy(playground);
        Destroy(explosionMaterialInstance);
    }

    void Update()
    {
        controller1.OnUpdate();
        controller2.OnUpdate();
    }

    void FixedUpdate()
    {
        Profiler.BeginSample("HandleExplosion");
        HandleExplosion();
        Profiler.EndSample();

        snake1.DoUpdate(this);
        snake2.DoUpdate(this);

        if (snake1.alive) playground.SetPixel(snake1.posX, snake1.posY, snake1.color);
        if (snake2.alive) playground.SetPixel(snake2.posX, snake2.posY, snake2.color);
        playground.Apply();
    }

    public bool IsFree(int x, int y)
    {
        var col = playground.GetPixel(x, y);
        return (col.a < 0.5f);
    }
}
