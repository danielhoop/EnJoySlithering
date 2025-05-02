using ControllerToMouseMapper.Input;

namespace Slither;

public class Screen
{
    public double Radius { get; set; } = 65536 / 4;
    public double XCenter { get; set; } = 65536 / 2;
    public double YCenter { get; set; } = 65536 / 2;
    public double ScreenWidth { get; set; } = 1920;
    public double ScreenHeight { get; set; } = 1200;
    public double XAdaption;

    public Screen()
    {
        var primaryScreen = System.Windows.Forms.Screen.PrimaryScreen;
        if (primaryScreen != null)
        {
            ScreenWidth = primaryScreen.Bounds.Width;
            ScreenHeight = primaryScreen.Bounds.Height;
        }
        XAdaption = ScreenHeight / ScreenWidth;
    }

    public void SetMousePositionFromJoystickValue(float x, float y)
    {
        if (x == 0 && y == 0)
            return;
        Mouse.SetPosition(
            XCenter + x * Radius * XAdaption,
            YCenter - y * Radius);
    }
}