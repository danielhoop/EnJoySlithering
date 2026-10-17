# EnJoySlithering - Use an XInput device to play slither.io on your Windows computer

This repository contains the source code for a console application that will map your XInput device inputs to movements in slither.io.

The name `EnJoySlithering` combines the words `enjoy`, `joystick` and `slithering`.

The application is based on a basic sample console application project that demonstrates how [XInputium](https://github.com/AderitoSilva/XInputium) can be used to emulate the mouse and keyboard.

```
           /^\/^\
         _|__|  O|
  \/     /~     \_/ \
   \____|__________/  \
          \_______      \
                  `\     \                 \
                    |     |                  \
                   /      /                    \
                  /     /        En Joy         \
                /      /      Slithering!        \ \
               /     /                            \  \
             /     /             _----_            \   \
            /     /           _-~      ~-_         |   |
           (      (        _-~    _--_    ~-_     _/   |
            \      ~-____-~    _-~    ~-_    ~-_-~    /
              ~-_           _-~          ~-_       _-~
                 ~--______-~                ~-___-~

------------------------------------------------------------
------------------------- Controls -------------------------
------------------------------------------------------------

- Left joystick:  Move mouse
- Right joystick: Move mouse
- Buttons ABRL:   Boost
- Button X:       Toggle slither & rejoin mode
  - Ctrl + Up/Down (on the keyboard):
                  Adjust slither & rejoin circle size
  - Ctrl + X (on the keyboard):
                  Toggle slither & rejoin mode.
- Button Y:       Toggle circle mode
  - D pad up:       Increase circle size
                    The initial slither length to circle
                    is set to 500. It can be increased
                    by steps of 500 to 1000, 1500, 2000, etc.
  - D pad down:     Decrease circle size
  - D pad right:    Set circle direction clockwise
  - D pad left:     Set circle direction counterclockwise
- Start:          F11 (toggle full screen mode)
- Back:
  - 1x:           Turn right joystick off
  - 2x:           Turn left joystick off
  - 3x:           Turn both joysticks off
  - 4x:           Turn both joysticks on
- Press Ctrl + Q to close the application.

------------------------------------------------------------
Author:  Daniel Hoop
         https://github.com/danielhoop
Credits: Aderito Silva
         https://github.com/AderitoSilva/XInputium
Icon:    Snake icon created by Freepik - Flaticon
------------------------------------------------------------
```
