using System.Windows;
using System;

using System.Diagnostics;

namespace SnakeCLI
{
    public class SpeedCurve
    {
        double a;
        double c;
        double d;
        double k;
        double definition;


        public SpeedCurve(float definition, int Width, int Height)
        {
            this.definition = definition;
            a =  -177.5;
            c = -19.5;
            d = 307.5;
            k = 0.1;
        }

        public double CalculateY(double x)
        {
            double power = (-k*(x + c));
            return (a/(1 + Math.Pow(Math.E, power)))+d;
        }
    }
}