using System.Runtime.InteropServices;
using ControllerToMouseMapper.Input;

namespace EnJoySlithering.Slither;

public class Screen
{
    public double Radius { get; set; } = 65536 / 4;
    public double XCenter { get; set; } = 65536 / 2;
    public double YCenter { get; set; } = 65536 / 2;
    public double ScreenWidth { get; set; }
    public double ScreenHeight { get; set; }
    public double XAdaption;

    [StructLayout(LayoutKind.Sequential)]
    struct DEVMODE
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
        public string dmDeviceName;
        public short dmSpecVersion;
        public short dmDriverVersion;
        public short dmSize;
        public short dmDriverExtra;
        public int dmFields;
        public int dmPositionX;
        public int dmPositionY;
        public int dmDisplayOrientation;
        public int dmDisplayFixedOutput;
        public short dmColor;
        public short dmDuplex;
        public short dmYResolution;
        public short dmTTOption;
        public short dmCollate;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
        public string dmFormName;
        public short dmLogPixels;
        public int dmBitsPerPel;
        public int dmPelsWidth;
        public int dmPelsHeight;
        public int dmDisplayFlags;
        public int dmDisplayFrequency;
        public int dmICMMethod;
        public int dmICMIntent;
        public int dmMediaType;
        public int dmDitherType;
        public int dmReserved1;
        public int dmReserved2;
        public int dmPanningWidth;
        public int dmPanningHeight;
    }

    public Screen()
    {
        [DllImport("user32.dll")]
        static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);
        const int ENUM_CURRENT_SETTINGS = -1;

        DEVMODE devMode = default;
        devMode.dmSize = (short)Marshal.SizeOf(devMode);
        EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref devMode);
        ScreenWidth = (double)devMode.dmPelsWidth;
        ScreenHeight = (double)devMode.dmPelsHeight;
        Console.WriteLine("Detected screen width:  {0}", ScreenWidth);
        Console.WriteLine("Detected screen height: {0}", ScreenHeight);

        if (ScreenWidth == 0)
            ScreenWidth = 1920;
        if (ScreenHeight == 0)
            ScreenHeight = 1200;

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