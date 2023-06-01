using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

// The template provides you with a window which displays a 'linear frame buffer', i.e.
// a 1D array of pixels that represents the graphical contents of the window.

// Under the hood, this array is encapsulated in a 'Surface' object, and copied once per
// frame to an OpenGL texture, which is then used to texture 2 triangles that exactly
// cover the window. This is all handled automatically by the template code.

// Before drawing the two triangles, the template calls the Tick method in MyApplication,
// in which you are expected to modify the contents of the linear frame buffer.

// After (or instead of) rendering the triangles you can add your own OpenGL code.

// We will use both the pure pixel rendering as well as straight OpenGL code in the
// tutorial. After the tutorial you can throw away this template code, or modify it at
// will, or maybe it simply suits your needs.

namespace RayTracing;

public class OpenTKApp : GameWindow
{
    /**
     * IMPORTANT:
     * 
     * Modern OpenGL (introduced in 2009) does NOT allow Immediate Mode or
     * Fixed-Function Pipeline commands, e.g., GL.MatrixMode, GL.Begin,
     * GL.End, GL.Vertex, GL.TexCoord, or GL.Enable certain capabilities
     * related to the Fixed-Function Pipeline. It also REQUIRES you to use
     * shaders.
     * 
     * If you want to try prehistoric OpenGL code, such as many code
     * samples still found online, enable it below.
     * 
     * MacOS doesn't support prehistoric OpenGL anymore since 2018.
     */
    public const bool AllowPrehistoricOpenGl = false;

    // All the data for the vertices interleaved in one array:
    // - XYZ in normalized device coordinates
    // - UV
    private readonly float[] _vertices =
    {
        //  X      Y     Z     U     V
        -1.0f, -1.0f, 0.0f, 0.0f, 1.0f, // bottom-left  2-----3 triangles:
        1.0f, -1.0f, 0.0f, 1.0f, 1.0f, // bottom-right | \   |     012
        -1.0f, 1.0f, 0.0f, 0.0f, 0.0f, // top-left     |   \ |     123
        1.0f, 1.0f, 0.0f, 1.0f, 0.0f // top-right    0-----1
    };

    private MyApplication? _app; // instance of the application

    private int _screenId; // unique integer identifier of the OpenGL texture
    private bool _terminated; // application terminates gracefully when this is true
    public int ProgramId;

    // The following variables are only needed in Modern OpenGL
    public int VertexArrayObject;
    public int VertexBufferObject;

    public OpenTKApp()
        : base(GameWindowSettings.Default, new NativeWindowSettings
        {
            Size = new Vector2i(1280, 800),
            Profile = AllowPrehistoricOpenGl ? ContextProfile.Compatability : ContextProfile.Core,
            Flags = AllowPrehistoricOpenGl ? ContextFlags.Default : ContextFlags.ForwardCompatible
        })
    {
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        // called during application initialization
        GL.ClearColor(0, 0, 0, 0);
        GL.Disable(EnableCap.DepthTest);
        Surface screen = new(ClientSize.X, ClientSize.Y);
        _app = new MyApplication(screen);
        _screenId = _app.Screen.GenTexture();
        if (AllowPrehistoricOpenGl)
        {
            GL.Enable(EnableCap.Texture2D);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
        }
        else
        {
            // setting up a Modern OpenGL pipeline takes a lot of code

            // Vertex Array Object: will store the meaning of the data in the buffer
            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);
            // Vertex Buffer Object: a buffer of raw data
            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices,
                BufferUsageHint.StaticDraw);
            // Vertex Shader
            var shaderSource = File.ReadAllText("../../../shaders/screen_vs.glsl");
            var vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, shaderSource);
            GL.CompileShader(vertexShader);
            GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out var status);
            if (status != (int)All.True)
            {
                var log = GL.GetShaderInfoLog(vertexShader);
                throw new Exception($"Error occurred whilst compiling vertex shader ({vertexShader}):\n{log}");
            }

            // Fragment Shader
            shaderSource = File.ReadAllText("../../../shaders/screen_fs.glsl");
            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, shaderSource);
            GL.CompileShader(fragmentShader);
            GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out status);
            if (status != (int)All.True)
            {
                var log = GL.GetShaderInfoLog(fragmentShader);
                throw new Exception($"Error occurred whilst compiling fragment shader ({fragmentShader}):\n{log}");
            }

            // Program: a set of shaders to be used together in a pipeline
            ProgramId = GL.CreateProgram();
            GL.AttachShader(ProgramId, vertexShader);
            GL.AttachShader(ProgramId, fragmentShader);
            GL.LinkProgram(ProgramId);
            GL.GetProgram(ProgramId, GetProgramParameterName.LinkStatus, out status);
            if (status != (int)All.True)
            {
                var log = GL.GetProgramInfoLog(ProgramId);
                throw new Exception($"Error occurred whilst linking program ({ProgramId}):\n{log}");
            }

            // the program contains the compiled shaders, we can delete the source
            GL.DetachShader(ProgramId, vertexShader);
            GL.DetachShader(ProgramId, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
            // send all the following draw calls through this pipeline
            GL.UseProgram(ProgramId);
            // tell the VAO which part of the VBO data should go to each shader input
            var location = GL.GetAttribLocation(ProgramId, "vPosition");
            GL.EnableVertexAttribArray(location);
            GL.VertexAttribPointer(location, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            location = GL.GetAttribLocation(ProgramId, "vUV");
            GL.EnableVertexAttribArray(location);
            GL.VertexAttribPointer(location, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float),
                3 * sizeof(float));
            // connect the texture to the shader uniform variable
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _screenId);
            GL.Uniform1(GL.GetUniformLocation(ProgramId, "pixels"), 0);
        }

        _app.Init();
    }

    protected override void OnUnload()
    {
        base.OnUnload();
        // called upon app close
        GL.DeleteTextures(1, ref _screenId);
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        // called upon window resize. Note: does not change the size of the pixel buffer.
        GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
        if (AllowPrehistoricOpenGl)
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(-1.0, 1.0, -1.0, 1.0, 0.0, 4.0);
        }
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);
        // called once per frame; app logic
        if (KeyboardState[Keys.Escape]) _terminated = true;
        // if pressed, keyboard keys: w, a, s, d, passed as boolean to update() function
        bool wPressed = KeyboardState[Keys.W];
        bool aPressed = KeyboardState[Keys.A];
        bool sPressed = KeyboardState[Keys.S];
        bool dPressed = KeyboardState[Keys.D];
        _app?.Update(wPressed, aPressed, sPressed, dPressed);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);
        // called once per frame; render
        _app?.Tick();
        if (_terminated)
        {
            Close();
            return;
        }

        // convert MyApplication.screen to OpenGL texture
        if (_app != null)
        {
            GL.BindTexture(TextureTarget.Texture2D, _screenId);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                _app.Screen.Width, _app.Screen.Height, 0,
                PixelFormat.Bgra,
                PixelType.UnsignedByte, _app.Screen.Pixels
            );
            // draw screen filling quad
            if (AllowPrehistoricOpenGl)
            {
                GL.Begin(PrimitiveType.Quads);
                GL.TexCoord2(0.0f, 1.0f);
                GL.Vertex2(-1.0f, -1.0f);
                GL.TexCoord2(1.0f, 1.0f);
                GL.Vertex2(1.0f, -1.0f);
                GL.TexCoord2(1.0f, 0.0f);
                GL.Vertex2(1.0f, 1.0f);
                GL.TexCoord2(0.0f, 0.0f);
                GL.Vertex2(-1.0f, 1.0f);
                GL.End();
            }
            else
            {
                GL.BindVertexArray(VertexArrayObject);
                GL.UseProgram(ProgramId);
                GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
            }
        }

        // tell OpenTK we're done rendering
        SwapBuffers();
    }

    public static void Main()
    {
        // entry point
        using OpenTKApp app = new();
        app.RenderFrequency = 30.0;
        app.Run();
    }
}