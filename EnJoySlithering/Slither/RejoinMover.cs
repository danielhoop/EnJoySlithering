using XInputium.XInput;

namespace EnJoySlithering.Slither;

public class RejoinMover
{
    private readonly XGamepad Gamepad;
    private readonly Screen Screen;
    private bool PauseModeOn = false;
    private readonly double MsMaxBeforeNextStep = 20;
    private const float MinCircleFactor = 0.025f;
    private const float MaxCircleFactor = 1f;
    private const float CircleFactorStep = 0.025f;
    private float CircleFactor = 0.6f;
    private const float TopCrossingRatio = 0.95f;
    private const double StartAngleOffset = 0.1;
    private static readonly TimeSpan BoostPressLength = TimeSpan.FromMilliseconds(100);
    private double Alpha = 0;
    private readonly double AlphaIncrPerStep = PauseMoverCircle.CalcAlphaIncrForTargetLength(500);
    private float X = 0f;
    private float Y = 0f;
    private float XBefore = 0f;
    private DateTime LastDirectionChange = DateTime.Now;
    private DateTime BoostReleaseTime = DateTime.MinValue;
    private bool BoostPressed;

    public RejoinMover(Screen screen, XGamepad gamepad)
    {
        Screen = screen;
        Gamepad = gamepad;
    }

    public void StartPauseMode()
    {
        PauseModeOn = true;
        Alpha = Math.PI / 2 + StartAngleOffset;
        CalcNewXY();
        XBefore = X;
        LastDirectionChange = DateTime.Now;
        ReleaseBoost();
    }

    public bool IsPauseModeOn()
    {
        return PauseModeOn;
    }

    public float AdjustCircleFactor(int direction)
    {
        CircleFactor = Math.Clamp(CircleFactor + direction * CircleFactorStep, MinCircleFactor, MaxCircleFactor);
        return CircleFactor;
    }

    public void PreviewCircleSize()
    {
        if (PauseModeOn)
            return;

        Screen.SetMousePositionFromJoystickValue(0f, CircleFactor, JoystickType.LEFT);
    }

    public void StopPauseMode()
    {
        PauseModeOn = false;
        ReleaseBoost();
    }

    public void AdaptMousePosition()
    {
        if (!PauseModeOn)
            return;

        DateTime now = DateTime.Now;
        if (BoostPressed && now >= BoostReleaseTime)
        {
            ReleaseBoost();
        }

        double msDiff = (now - LastDirectionChange).TotalMilliseconds;
        if (msDiff > MsMaxBeforeNextStep)
        {
            XBefore = X;
            Alpha -= AlphaIncrPerStep;
            CalcNewXY();
            LastDirectionChange = now;

            if (XBefore <= 0f && X > 0f && Y >= CircleFactor * TopCrossingRatio)
            {
                PressBoost(now);
            }
        }

        Screen.SetMousePositionFromJoystickValue(X, Y, JoystickType.LEFT);
    }

    private void CalcNewXY()
    {
        X = (float)Math.Cos(Alpha) * CircleFactor;
        Y = (float)Math.Sin(Alpha) * CircleFactor;
    }

    private static double CalcAlphaFromXY(double x, double y)
    {
        return Math.Atan2(y, x);
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

    private void PressBoost(DateTime now)
    {
        if (!BoostPressed)
        {
            Screen.Boost();
            BoostPressed = true;
        }

        BoostReleaseTime = now + BoostPressLength;
    }

    private void ReleaseBoost()
    {
        if (!BoostPressed)
            return;

        Screen.StopBoosting();
        BoostPressed = false;
        BoostReleaseTime = DateTime.MinValue;
    }

}
