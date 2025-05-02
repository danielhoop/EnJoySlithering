using ControllerToMouseMapper.Input;

namespace EnJoySlithering.Slither;

public class PauseMoverCircle
{
    private Screen Screen { get; set; }
    private bool PauseModeOn { get; set; }
    private double MsMaxBeforeNextStep { get; set; }
    private double Alpha { get; set; } = 0; // Winkel Alpha
    private double AlphaIncrPerStepOriginal { get; set; } = 0.005;
    private double AlphaIncrPerStep { get; set; }
    private double AlphaAcceleratorFactor { get; set; } = 1.0007; // Was 1.001
    private float X { get; set; } = 0f;
    private float Y { get; set; } = 0f;


    private DateTime LastDirectionChange { get; set; } = DateTime.Now; // Will be overwritten when button is pressed.
    public PauseMoverCircle(Screen screen)
    {
        Screen = screen;
    }
    public void StartPauseMode()
    {
        PauseModeOn = true;
        Alpha = 0;
        LastDirectionChange = DateTime.Now;
        AlphaIncrPerStep = AlphaIncrPerStepOriginal;
    }
    public bool IsPauseModeOn()
    {
        return PauseModeOn;
    }
    public void StopPauseMode()
    {
        PauseModeOn = false;
    }
    public void AdaptMousePosition()
    {
        if (!PauseModeOn)
            return;
        double msDiff = (DateTime.Now - LastDirectionChange).TotalMilliseconds;
        if (msDiff > MsMaxBeforeNextStep)
        {
            AlphaIncrPerStep *= AlphaAcceleratorFactor;
            if (AlphaIncrPerStep > 0.04)
                AlphaIncrPerStep = 0.04;
            Console.WriteLine(AlphaIncrPerStep);
            LastDirectionChange = DateTime.Now;
            Alpha = (Alpha - AlphaIncrPerStep) % 360;
            CalcNewXY();
        }
        Screen.SetMousePositionFromJoystickValue(X, Y);
    }
    private void CalcNewXY()
    {
        // Sinus(Alpha) = G / H;
        // Tangens(Alpha) = G / A;
        if (Math.Abs(Alpha) < 0.01)
        {
            X = 0f;
            Y = 1f;
            return;
        }
        double H = 1;
        double G = Math.Sin(Alpha) * H;
        double A = G / Math.Tan(Alpha);
        X = (float)A;
        Y = (float)G;
    }
    public void IncreaseSpeed()
    {
        AlphaIncrPerStep *= 1.1;
        Console.WriteLine("Speed is now: " + AlphaIncrPerStep);
    }
    public void DecreaseSpeed()
    {
        AlphaIncrPerStep /= 1.1;
        Console.WriteLine("Speed is now: " + AlphaIncrPerStep);
    }
}
