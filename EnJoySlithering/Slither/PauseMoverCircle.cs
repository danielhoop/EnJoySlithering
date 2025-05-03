using ControllerToMouseMapper.Input;
using XInputium.XInput;

namespace EnJoySlithering.Slither;

public class PauseMoverCircle
{
    private readonly XGamepad Gamepad;
    private readonly Screen Screen;
    private bool PauseModeOn = false;
    // When the frame frequency is 60 fps, i.e., approx 15. ms,
    // the setting below will lead to an update each second frame.
    private readonly double MsMaxBeforeNextStep = 20;
    private double Alpha = 0; // Winkel Alpha im Rechtwinkligen Kreis-Dreieck
    private double AlphaIncrPerStepOriginal = 0.005;
    private double AlphaDirectionMultiplier = 1;
    private double AlphaIncrPerStep;
    private readonly double AlphaAcceleratorFactor = 1.0007; // Was 1.001
    private float X = 0f;
    private float Y = 0f;
    private DateTime LastDirectionChange = DateTime.Now; // Will be overwritten when button is pressed.

    public PauseMoverCircle(Screen screen, XGamepad gamepad)
    {
        Screen = screen;
        Gamepad = gamepad;
        // It is necessary to set this here, otherwise the originalspeed will be set to 0 if the user
        // increases/decreases the speed before he/she entered into pause mode for the first time.
        AlphaIncrPerStep = AlphaIncrPerStepOriginal;
    }

    public void StartPauseMode()
    {
        PauseModeOn = true;
        Alpha = DetermineStartAlpha();
        CalcNewXY();
        //Console.WriteLine("Alpha is: " + Alpha + "\nX is: " + Gamepad.LeftJoystick.X + "\nY is: " + Gamepad.LeftJoystick.Y);
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
            // It is only allowed to go manually bwyond the speed of 0.044
            if (AlphaIncrPerStep < 0.044)
            {
                AlphaIncrPerStep = Math.Min(0.044, AlphaIncrPerStep * AlphaAcceleratorFactor);
            }
            LastDirectionChange = DateTime.Now;
            Alpha -= AlphaIncrPerStep * AlphaDirectionMultiplier;
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
            X = 1f;
            Y = 0f;
            return;
        }
        double H = 1;
        double radians = Alpha; //= Alpha * (Math.PI/180);
        double G = Math.Sin(radians) * H;
        double A = G / Math.Tan(radians);
        //Console.WriteLine("Alpha = " + Alpha + "\nG = " + G + "\A = " + A);
        X = (float)A;
        Y = (float)G;
    }
    private static double CalcAlphaFromXY(double x, double y)
    {
        /*
        The goal to achieve is:
        y =  0, x =  1 -> 0
        y = -1, x =  0 -> -1.664 = Math.PI/2
        y =  0, x = -1 -> -3.224 = Math.PI
        y =  1, x =  0 -> -4.701 = Math.PI*1.5
        */
        double xr = Math.Round(10 * x);
        double yr = Math.Round(10 * y);
        if (x > 0 && y < 0)
            return -1 * Math.Acos(y) + Math.PI / 2;
        if (x < 0 && y < 0)
            return Math.Asin(x) - Math.PI / 2;
        if (x < 0 && y > 0)
            return -1 * Math.Acos(-x) - Math.PI;
        if (x > 0 && y > 0)
            return -1 * Math.Asin(x) - Math.PI * 1.5;
        return -Math.PI * 1.5;
    }
    private double DetermineStartAlpha()
    {
        if (Gamepad.LeftJoystick.X != 0 || Gamepad.LeftJoystick.Y != 0)
        {
            return CalcAlphaFromXY(Gamepad.LeftJoystick.X, Gamepad.LeftJoystick.Y);
        }
        if (Gamepad.RightJoystick.X != 0 || Gamepad.RightJoystick.Y != 0)
        {
            return CalcAlphaFromXY(Gamepad.RightJoystick.X, Gamepad.RightJoystick.Y);
        }
        return 0;
    }
    public void IncreaseSpeed()
    {
        AlphaIncrPerStep *= 1.1;
        AlphaIncrPerStepOriginal = AlphaIncrPerStep;
        // Console.WriteLine("Speed is now: " + AlphaIncrPerStep);
    }
    public void DecreaseSpeed()
    {
        AlphaIncrPerStep /= 1.1;
        AlphaIncrPerStepOriginal = AlphaIncrPerStep;
        // Console.WriteLine("Speed is now: " + AlphaIncrPerStep);
    }
    public void SetDirectionClockwise()
    {
        AlphaDirectionMultiplier = 1;
    }
    public void SetDirectionAntiClockwise()
    {
        AlphaDirectionMultiplier = -1;
    }
}
