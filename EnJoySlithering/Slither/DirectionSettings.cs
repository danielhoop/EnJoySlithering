namespace Slither;
class DirectionSetting {
    private double SecondsDiffOriginal;
    public double SecondsDiff { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public DirectionSetting(double secondsDiff, double x, double y) {
        SecondsDiff = secondsDiff;
        SecondsDiffOriginal = secondsDiff;
        X = x;
        Y = y;
    }
    public void resetValues() {
        SecondsDiff = SecondsDiffOriginal;
    }
}
