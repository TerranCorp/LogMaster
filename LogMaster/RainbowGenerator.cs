public class RainbowGenerator
{
    public static List<(int r, int g, int b)> GenerateRainbow(int steps = 50)
    {
        var colors = new List<(int r, int g, int b)>();

        for (int i = 0; i < steps; i++)
        {
            double hue = i * 360.0 / steps; // 0 -> 360 degrees
            colors.Add(HsvToRgb(hue, 1.0, 1.0));
        }

        return colors;
    }

    // Converts HSV to RGB
    public static (int r, int g, int b) HsvToRgb(double h, double s, double v)
    {
        double c = v * s;
        double x = c * (1 - Math.Abs((h / 60) % 2 - 1));
        double m = v - c;

        double r1 = 0, g1 = 0, b1 = 0;

        if (h >= 0 && h < 60) { r1 = c; g1 = x; b1 = 0; }
        else if (h >= 60 && h < 120) { r1 = x; g1 = c; b1 = 0; }
        else if (h >= 120 && h < 180) { r1 = 0; g1 = c; b1 = x; }
        else if (h >= 180 && h < 240) { r1 = 0; g1 = x; b1 = c; }
        else if (h >= 240 && h < 300) { r1 = x; g1 = 0; b1 = c; }
        else { r1 = c; g1 = 0; b1 = x; }

        int r = (int)((r1 + m) * 255);
        int g = (int)((g1 + m) * 255);
        int b = (int)((b1 + m) * 255);

        return (r, g, b);
    }
}