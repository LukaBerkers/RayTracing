using OpenTK.Graphics.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace RayTracing;

public class Surface
{
    private static Surface? _font;
    private static int[]? _fontRedir;
    public int[] Pixels;

    public int Width, Height;

    // surface constructor
    public Surface(int w, int h)
    {
        Width = w;
        Height = h;
        Pixels = new int[w * h];
    }

    // surface constructor using a file
    public Surface(string fileName)
    {
        var bmp = Image.Load<Bgra32>(fileName);
        Width = bmp.Width;
        Height = bmp.Height;
        Pixels = new int[Width * Height];
        for (var y = 0; y < Height; y++)
        for (var x = 0; x < Width; x++)
            Pixels[y * Width + x] = (int)bmp[x, y].Bgra;
    }

    // create an OpenGL texture
    public int GenTexture()
    {
        var id = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, id);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0, PixelFormat.Bgra,
            PixelType.UnsignedByte, Pixels);
        return id;
    }

    // clear the surface
    public void Clear(int c)
    {
        for (int s = Width * Height, p = 0; p < s; p++) Pixels[p] = c;
    }

    // copy the surface to another surface
    public void CopyTo(Surface target, int x = 0, int y = 0)
    {
        var src = 0;
        var dst = 0;
        var srcWidth = Width;
        var srcHeight = Height;
        var dstWidth = target.Width;
        var dstHeight = target.Height;
        if (srcWidth + x > dstWidth) srcWidth = dstWidth - x;
        if (srcHeight + y > dstHeight) srcHeight = dstHeight - y;
        if (x < 0)
        {
            src -= x;
            srcWidth += x;
            x = 0;
        }

        if (y < 0)
        {
            src -= y * Width;
            srcHeight += y;
            y = 0;
        }

        if (srcWidth > 0 && srcHeight > 0)
        {
            dst += x + dstWidth * y;
            for (var v = 0; v < srcHeight; v++)
            {
                for (var u = 0; u < srcWidth; u++) target.Pixels[dst + u] = Pixels[src + u];
                dst += dstWidth;
                src += Width;
            }
        }
    }

    // draw a rectangle
    public void Box(int x1, int y1, int x2, int y2, int c)
    {
        var dest = y1 * Width;
        for (var y = y1; y <= y2; y++, dest += Width)
        {
            Pixels[dest + x1] = c;
            Pixels[dest + x2] = c;
        }

        var dest1 = y1 * Width;
        var dest2 = y2 * Width;
        for (var x = x1; x <= x2; x++)
        {
            Pixels[dest1 + x] = c;
            Pixels[dest2 + x] = c;
        }
    }

    // draw a solid bar
    public void Bar(int x1, int y1, int x2, int y2, int c)
    {
        var dest = y1 * Width;
        for (var y = y1; y <= y2; y++, dest += Width)
        for (var x = x1; x <= x2; x++)
            Pixels[dest + x] = c;
    }

    // helper function for line clipping
    // ReSharper disable once InconsistentNaming
    private int OUTCODE(int x, int y)
    {
        int xMin = 0, yMin = 0, xMax = Width - 1, yMax = Height - 1;
        return (x < xMin ? 1 : x > xMax ? 2 : 0) + (y < yMin ? 4 : y > yMax ? 8 : 0);
    }

    // draw a line, clipped to the window
    public void Line(int x1, int y1, int x2, int y2, int c)
    {
        int xMin = 0, yMin = 0, xMax = Width - 1, yMax = Height - 1;
        int c0 = OUTCODE(x1, y1), c1 = OUTCODE(x2, y2);
        var accept = false;
        while (true)
            if (c0 == 0 && c1 == 0)
            {
                accept = true;
                break;
            }
            else if ((c0 & c1) > 0)
            {
                break;
            }
            else
            {
                int x = 0, y = 0;
                var co = c0 > 0 ? c0 : c1;
                if ((co & 8) > 0)
                {
                    x = x1 + (x2 - x1) * (yMax - y1) / (y2 - y1);
                    y = yMax;
                }
                else if ((co & 4) > 0)
                {
                    x = x1 + (x2 - x1) * (yMin - y1) / (y2 - y1);
                    y = yMin;
                }
                else if ((co & 2) > 0)
                {
                    y = y1 + (y2 - y1) * (xMax - x1) / (x2 - x1);
                    x = xMax;
                }
                else if ((co & 1) > 0)
                {
                    y = y1 + (y2 - y1) * (xMin - x1) / (x2 - x1);
                    x = xMin;
                }

                if (co == c0)
                {
                    x1 = x;
                    y1 = y;
                    c0 = OUTCODE(x1, y1);
                }
                else
                {
                    x2 = x;
                    y2 = y;
                    c1 = OUTCODE(x2, y2);
                }
            }

        if (!accept) return;
        if (Math.Abs(x2 - x1) >= Math.Abs(y2 - y1))
        {
            if (x2 < x1)
            {
                (x2, x1) = (x1, x2);
                (y2, y1) = (y1, y2);
            }

            var l = x2 - x1;
            if (l == 0) return;
            var dy = (y2 - y1) * 8192 / l;
            y1 *= 8192;
            for (var i = 0; i < l; i++)
            {
                Pixels[x1++ + y1 / 8192 * Width] = c;
                y1 += dy;
            }
        }
        else
        {
            if (y2 < y1)
            {
                (x2, x1) = (x1, x2);
                (y2, y1) = (y1, y2);
            }

            var l = y2 - y1;
            if (l == 0) return;
            var dx = (x2 - x1) * 8192 / l;
            x1 *= 8192;
            for (var i = 0; i < l; i++)
            {
                Pixels[x1 / 8192 + y1++ * Width] = c;
                x1 += dx;
            }
        }
    }

    // plot a single pixel
    public void Plot(int x, int y, int c)
    {
        if (x >= 0 && y >= 0 && x < Width && y < Height) Pixels[x + y * Width] = c;
    }

    // print a string
    public void Print(string t, int x, int y, int c)
    {
        if (_font == null || _fontRedir == null)
        {
            _font = new Surface("../../../assets/font.png");
            // ReSharper disable StringLiteralTypo
            var ch = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_-+={}[];:<>,.?/\\ ";
            // ReSharper restore StringLiteralTypo
            _fontRedir = new int[256];
            for (var i = 0; i < 256; i++) _fontRedir[i] = 0;
            for (var i = 0; i < ch.Length; i++)
            {
                int l = ch[i];
                _fontRedir[l & 255] = i;
            }
        }

        for (var i = 0; i < t.Length; i++)
        {
            var f = _fontRedir[t[i] & 255];
            var dest = x + i * 12 + y * Width;
            var src = f * 12;
            for (var v = 0; v < _font.Height; v++, src += _font.Width, dest += Width)
            for (var u = 0; u < 12; u++)
                if ((_font.Pixels[src + u] & 0xffffff) != 0)
                    Pixels[dest + u] = c;
        }
    }
}