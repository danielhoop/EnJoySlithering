using System.Runtime.InteropServices;

namespace EnJoySlithering.Slither;

public class JoystickSwitch {
    int NumberOfTimesPressed = 0;
    public void Press() {
        NumberOfTimesPressed = (NumberOfTimesPressed + 1) % 4;
    }
    public bool IsLeftJoystickTurnedOn() {
        return NumberOfTimesPressed == 0 || NumberOfTimesPressed == 1;
    }
    public bool IsRightJoystickTurnedOn() {
        return NumberOfTimesPressed == 0 || NumberOfTimesPressed == 2;
    }
}