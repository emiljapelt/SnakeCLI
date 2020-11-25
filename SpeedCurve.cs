using System.Windows;
using System;

using System.Diagnostics;

namespace SnakeCLI
{
    public class SpeedCurve
    {
        VariableDouble a;
        VariableDouble c;
        VariableDouble d;
        VariableDouble k;
        double definition;


        public SpeedCurve(float definition, int Width, int Height)
        {
            this.definition = definition;
            a = new VariableDouble(){ Value = -177.5};
            c = new VariableDouble(){ Value = -19.5};
            d = new VariableDouble(){ Value = 307.5};
            k = new VariableDouble(){ Value = 0.1};
        }

        public double CalculateY(double x)
        {
            double power = (-k.Value*(x + c.Value));
            return (a.Value/(1 + Math.Pow(Math.E, power)))+d.Value;
        }
    }
}