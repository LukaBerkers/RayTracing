using OpenTK.Mathematics;

namespace RayTracing;

public class MyApplication
{
    // member variables
    public Surface Screen;

    // constructor
    public MyApplication(Surface screen)
    {
        Screen = screen;
    }

    // initialize
    public void Init()
    {
        var camera = new Camera((0, 0, 0), (0, 0, 1), (0, 1, 0));
    }

    // tick: renders one frame
    public void Tick()
    {
        Screen.Clear(0);
        Screen.Print("hello world", 2, 2, 0xffffff);
        Screen.Line(2, 20, 160, 20, 0xff0000);
    }
}