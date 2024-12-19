namespace AlexHelms.NINA.Prometheusexporter;

public static class Util
{
    public static double ReplaceNan(double value, double defaultValue = 0) 
        => double.IsNaN(value) ? defaultValue : value;
}
