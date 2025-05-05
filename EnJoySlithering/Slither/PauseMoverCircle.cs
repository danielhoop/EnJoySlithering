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
    private double TargetSpeedLength = 500;
    private double AlphaIncrPerStepOriginal;
    private double AlphaDirectionMultiplier = 1;
    private double AlphaIncrPerStep;
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
        AlphaIncrPerStepOriginal = CalcAlphaIncrForTargetLength(TargetSpeedLength);
        AlphaIncrPerStep = AlphaIncrPerStepOriginal;
    }
    public bool IsPauseModeOn()
    {
        return PauseModeOn;
    }
    public void StopPauseMode()
    {
        PauseModeOn = false;
        // This has to be done so that the increase/decrease of speed in the respective functions can be done without an if/else statement.
        AlphaIncrPerStep = AlphaIncrPerStepOriginal;
    }
    public void AdaptMousePosition()
    {
        if (!PauseModeOn)
            return;
        double msDiff = (DateTime.Now - LastDirectionChange).TotalMilliseconds;
        if (msDiff > MsMaxBeforeNextStep)
        {
            // It is only allowed to go manually beyond the speed of 0.044, but not automatically
            if (AlphaIncrPerStep < 0.044)
            {
                // The line below works well for different sizes.
                // The factor 0.1609 has to be changed if the speed adaption is done more than 30 times per second.
                // It was determined from the following two data points:
                //  * AlphaIncrPerStep = 0.02358,  8.9 secs for a full circle -> n = 8.9*30 -> 0.1588 = x = 1 / (n * AlphaIncrPerStep)
                //  * AlphaIncrPerStep = 0.031384, 6.6 secs for a full circle -> n = 6.6*30 -> 0.1609 = x = 1 / (n * AlphaIncrPerStep)
                // For 60 fps, x (i.e., 0.1609) probably has to be doubled.
                AlphaIncrPerStep = Math.Min(0.044, AlphaIncrPerStep * Math.Pow(1 / 0.82, AlphaIncrPerStep * 0.1609));

                // This line was ok only for a certain size.
                // double AlphaAcceleratorFactor = 1.0007; // Was 1.001
                // AlphaIncrPerStep = Math.Min(0.044, AlphaIncrPerStep * AlphaAcceleratorFactor);
            }
            //Console.WriteLine("AlphaIncrPerStep: {0}", AlphaIncrPerStep);
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
        if (PauseModeOn)
        {
            AlphaIncrPerStep *= 1.075;
            AlphaIncrPerStepOriginal = AlphaIncrPerStep;
        }
        else
        {
            TargetSpeedLength = Math.Max(TargetSpeedLength - 500, 500);
            AlphaIncrPerStepOriginal = CalcAlphaIncrForTargetLength(TargetSpeedLength);
        }
        //Console.WriteLine("Speed is now: " + AlphaIncrPerStepOriginal);
        //Console.WriteLine("TargetSpeedLength: " + TargetSpeedLength);
    }
    public void DecreaseSpeed()
    {
        if (PauseModeOn)
        {
            AlphaIncrPerStep /= 1.075;
            AlphaIncrPerStepOriginal = AlphaIncrPerStep;
        }
        else
        {
            TargetSpeedLength += 500;
            AlphaIncrPerStepOriginal = CalcAlphaIncrForTargetLength(TargetSpeedLength);
        }
    }
    public static double CalcAlphaIncrForTargetLength(double len)
    {
        // From these measurements:
        // Length -> Optimal AlphaIncrPerStep
        //  500 -> 0.03797498
        // 1007 -> 0.025357206952925777
        // 1522 -> 0.01898749
        // 2067 -> 0.01528418076395789
        // 2533 -> 0.014217842571123619
        // 3868 -> 0.010646328049534463
        // 4618 -> 0.009903560976311128
        // 5200 -> 0.009212614861684771

        // This formula results: 
        double alphaIncr = 1.6931 * Math.Pow(len, -0.612);

        // Because this formula was calculated without making the circle continuously smaller,
        // We can increase the size a little bit by decreasing the speed.
        return alphaIncr * 0.95;
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
