using UnityEngine;
using UnityEngine.Profiling;

public sealed class Bresenham : MonoBehaviour, IPlayground
{
    [SerializeField] private RenderTexture target;
    [SerializeField] private Material upsampler;
    [SerializeField] private int size = 256;

    private Color32[] pixels;
    private Color32[] buffer;
    private Texture2D texture;
    private RenderTexture renderTexture;

    private Color explosionColorF = Color.red;
    private Color32 explosionColor = new Color32(255, 0, 0, 255);
    private Color32[] explosionPixels;

    private SnakeController controller1, controller2;
    private Snake snake1, snake2;

    void Start()
    {
        pixels = new Color32[size * size];
        buffer = new Color32[size * size];
        texture = new Texture2D(size, size, TextureFormat.ARGB32, false, true);
        texture.filterMode = FilterMode.Point;

        explosionPixels = new Color32[size];
        for (int i = 0; i < explosionPixels.Length; i++)
        {
            explosionPixels[i] = explosionColor;
        }

        var empty = new Color32(0, 0, 0, 0);
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = empty;
        }

        texture.SetPixels32(pixels);
        texture.Apply();

        controller1 = new SnakeController("Up1", "Down1", "Left1", "Right1", "Jump1");
        controller2 = new SnakeController("Up2", "Down2", "Left2", "Right2", "Jump2");
        snake1 = new Snake( size / 4 * 1, size / 8, Snake.Direction.Up, Color.red, controller1);
        snake2 = new Snake( size / 4 * 3, size / 8, Snake.Direction.Up, Color.blue, controller2);
    }

    void OnDestroy()
    {
        Destroy(texture);
    }

    void Update()
    {
        controller1.OnUpdate();
        controller2.OnUpdate();
    }

    bool previewFrame = false;
    void FixedUpdate()
    {
        /*
        Profiler.BeginSample("DrawExplosion");
        int cx = Random.Range(32, size - 32);
        int cy = Random.Range(32, size - 32);
        DrawExplosion2(cx, cy, 27, 30);
        Profiler.EndSample();

        Profiler.BeginSample("Regionize");
        int minx = Mathf.Max(0, cx - 30);
        int miny = Mathf.Max(0, cy - 30);
        int maxx = Mathf.Min(size, cx + 30);
        int maxy = Mathf.Min(size, cy + 30);
        int lenx = maxx - minx;
        int leny = maxy - miny;
        for (int y = 0; y < leny; y++)
        {
            System.Array.Copy(pixels, minx + (miny + y) * size, buffer, y * lenx, leny);
        }
        Profiler.EndSample();

        Profiler.BeginSample("SetPixels");
        texture.SetPixels32(minx, miny, lenx, leny, buffer, 0);
        texture.Apply();
        Profiler.EndSample();
        */

        bool preview = previewFrame;
        previewFrame = !previewFrame;
        if (preview)
        {
            var color1 = snake1.color;
            var color2 = snake2.color;

            var d1 = snake1.direction;
            var d2 = snake2.direction;

            switch (snake1.direction)
            {
                case Snake.Direction.Up:
                    color1 = Color.blue;
                    break;
                case Snake.Direction.Left:
                    color1 = Color.green;
                    break;
                case Snake.Direction.Right:
                    color1 = Color.yellow;
                    break;
                case Snake.Direction.Down:
                    color1 = Color.white;
                    break;
            }

            Profiler.BeginSample("Draw Preview");
            if (snake1.alive) texture.SetPixel(snake1.nextPosX, snake1.nextPosY, color1);
            if (snake2.alive) texture.SetPixel(snake2.nextPosX, snake2.nextPosY, color2);
            Profiler.EndSample();
        }
        else
        {
            var op1x = snake1.posX;
            var op1y = snake1.posY;

            var od1 = snake1.direction;
            var od2 = snake2.direction;

            Profiler.BeginSample("Update Snakes");
            snake1.DoUpdate(this);
            snake2.DoUpdate(this);
            Profiler.EndSample();

            var color1 = snake1.color;
            var color2 = snake2.color;

            var d1 = snake1.direction;
            var d2 = snake2.direction;

            switch (d1)
            {
                case Snake.Direction.Up:
                    switch (od1)
                    {
                        case Snake.Direction.Left:
                            color1.a = 1f / 32;
                            texture.SetPixel(op1x, op1y, color1);
                            break;
                    }
                    color1.a = 5f / 32;
                    break;
                case Snake.Direction.Down:
                    switch (od1)
                    {
                        case Snake.Direction.Left:
                            color1.a = 5f / 32;
                            texture.SetPixel(op1x, op1y, color1);
                            break;
                        case Snake.Direction.Right:
                            color1.a = 6f / 32;
                            texture.SetPixel(op1x, op1y, color1);
                            break;
                    }
                    color1.a = 5f / 32;
                    break;
                case Snake.Direction.Left:
                    switch (od1)
                    {
                        case Snake.Direction.Up:
                            color1.a = 6f / 32;
                            texture.SetPixel(op1x, op1y, color1);
                            break;
                        case Snake.Direction.Down:
                            color1.a = 3f / 32;
                            texture.SetPixel(op1x, op1y, color1);
                            break;
                    }
                    color1.a = 3f / 32;
                    break;
                case Snake.Direction.Right:
                    switch (od1)
                    {
                        case Snake.Direction.Down:
                            color1.a = 1f / 32;
                            texture.SetPixel(op1x, op1y, color1);
                            break;
                    }
                    color1.a = 3f / 32;
                    break;
            }

            Profiler.BeginSample("Draw Snakes");
            if (snake1.alive) texture.SetPixel(snake1.posX, snake1.posY, color1);
            if (snake2.alive) texture.SetPixel(snake2.posX, snake2.posY, color2);
            Profiler.EndSample();
        }

        Profiler.BeginSample("Upload");
        texture.Apply();
        Profiler.EndSample();

        Profiler.BeginSample("Upsample");
        Graphics.Blit(texture, target, upsampler);
        Profiler.EndSample();
    }

    private void DrawExplosion(int xc, int yc, int rad)
    {
        int x = rad;
        int y = 0;
        int err = 1 - x;

        while (x >= y)
        {
            SetPixel(xc + x, yc + y);
            SetPixel(xc - x, yc + y);
            SetPixel(xc + x, yc - y);
            SetPixel(xc - x, yc - y);
            SetPixel(xc + y, yc + x);
            SetPixel(xc - y, yc + x);
            SetPixel(xc + y, yc - x);
            SetPixel(xc - y, yc - x);

            y++;

            if (err < 0)
            {
                err += 2 * y + 1;
            }
            else
            {
                x--;
                err += 2 * (y - x + 1);
            }
        }
    }

    private void SetPixel(int x, int y)
    {
        pixels[x + y * size] = explosionColor;
    }

    private void DrawExplosion2(int xc, int yc, int inner, int outer)
    {
        int xo = outer;
        int xi = inner;
        int y = 0;
        int erro = 1 - xo;
        int erri = 1 - xi;

        while (xo >= y)
        {
            xLine(xc + xi, xc + xo, yc + y);
            xLine(xc - xo, xc - xi, yc + y);
            xLine(xc - xo, xc - xi, yc - y);
            xLine(xc + xi, xc + xo, yc - y);
            yLine(xc + y,  yc + xi, yc + xo);
            yLine(xc - y,  yc + xi, yc + xo);
            yLine(xc - y,  yc - xo, yc - xi);
            yLine(xc + y,  yc - xo, yc - xi);

            y++;

            if (erro < 0) {
                erro += 2 * y + 1;
            } else {
                xo--;
                erro += 2 * (y - xo + 1);
            }

            if (y > inner) {
                xi = y;
            } else {
                if (erri < 0) {
                    erri += 2 * y + 1;
                } else {
                    xi--;
                    erri += 2 * (y - xi + 1);
                }
            }
        }
    }

    private void xLine(int x0, int x1, int y)
    {
        Profiler.BeginSample("xLine");
        for (int x = x0; x < x1; x++)
        {
//            texture.SetPixel(x, y, explosionColorF);
            pixels[(x + y * size)] = explosionColor;
        }
//        int start = (x0 + y * size);
//        int len = x1 - x0;
//        System.Array.Copy(explosionPixels, 0, pixels, start, len);
        Profiler.EndSample();
    }

    private void yLine(int x, int y0, int y1)
    {
        Profiler.BeginSample("yLine");
        for (int y = y0; y < y1; y++)
        {
//            texture.SetPixel(x, y, explosionColorF);
            pixels[(x + y * size)] = explosionColor;
        }
        Profiler.EndSample();
    }

    public bool IsFree(int x, int y)
    {
        var col = texture.GetPixel(x, y);
        return (col.a == 0f);
    }
}
