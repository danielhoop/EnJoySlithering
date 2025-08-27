using ControllerToMouseMapper.Input;

namespace EnJoySlithering.Slither;

public class PauseMoverRectangle
{
    private Screen Screen { get; set; }
    private bool PauseModeOn { get; set; }
    private double XReductionFactorEachRound { get; } = 0.9;
    private double MinSecondsDiff { get; } = 1.1;
    private DirectionSetting[] DirectionSettings { get; }
    private int CurrentSetting { get; set; } = 0;
    private DateTime LastDirectionChange { get; set; } = DateTime.Now; // Will be overwritten when button is pressed.
    public PauseMoverRectangle(Screen screen)
    {
        PauseModeOn = false;
        DirectionSettings = [
            new DirectionSetting(6.0,             1,  0), // for X seconds, go right
            new DirectionSetting(MinSecondsDiff,  0, -1), // for X seconds, go down
            new DirectionSetting(6.0,            -1,  0), // for X seconds, go left
            new DirectionSetting(MinSecondsDiff,  0,  1)]; // for X seconds, go up
        Screen = screen;
    }
    public void StartPauseMode()
    {
        PauseModeOn = true;
        CurrentSetting = 0;
        LastDirectionChange = DateTime.Now;
        foreach (var setting in DirectionSettings)
        {
            setting.ResetValues();
        }
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
        double secondsDiff = (DateTime.Now - LastDirectionChange).TotalSeconds;
        if (secondsDiff > DirectionSettings[CurrentSetting].SecondsDiff)
        {
            // When we're in the last setting, then we have to run a little less int he X direction because the tail of the snake will make a smaller curve.
            // Otherwise we would be vulnerable and could be caught by other snakes.
            if (DirectionSettings[CurrentSetting].Y != 0)
            {
                //if (currentSetting == directionSettings.Length - 1) {
                foreach (var setting in DirectionSettings)
                {
                    if (setting.X != 0)
                    {
                        setting.SecondsDiff *= XReductionFactorEachRound;
                        if (setting.SecondsDiff < MinSecondsDiff)
                        {
                            setting.SecondsDiff = MinSecondsDiff;
                        }
                    }
                }
            }
            CurrentSetting = (CurrentSetting + 1) % 4;
            LastDirectionChange = DateTime.Now;
        }
        Screen.SetMousePositionFromJoystickValue(DirectionSettings[CurrentSetting].X, DirectionSettings[CurrentSetting].Y, JoystickType.LEFT);
    }
}

class DirectionSetting
{
    private double SecondsDiffOriginal;
    public double SecondsDiff { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public DirectionSetting(double secondsDiff, float x, float y)
    {
        SecondsDiff = secondsDiff;
        SecondsDiffOriginal = secondsDiff;
        X = x;
        Y = y;
    }
    public void ResetValues()
    {
        SecondsDiff = SecondsDiffOriginal;
    }
}