using System;
using System.Drawing;
using System.Windows;

using TikoTako;

namespace TakonsoleTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        Takonsole con;
        Action<string> ImGonnaPrintSomething = Takonsole.Out;
        Action<string> ImGonnaPrintSomethingTwoTheRevenge = Takonsole.Inf;
        Action<string> W = Takonsole.Warn;
        Action<string> HoRyShItTo = Takonsole.Err;

        private void Alloc_Click(object sender, RoutedEventArgs e)
        {
            con = Takonsole.Alloc("Testing console");
            Takonsole.Out("Standard output with the static method.");
            Takonsole.Inf("Standard output with the static method.");
            Takonsole.Warn("Standard output with the static method.");
            Takonsole.Err("Standard output with the static method.");

            Alloc.IsEnabled = false;
            Test1.IsEnabled = Test2.IsEnabled = true;
        }

        void UpDown(ref int f, ref int b, int rL)
        {
            f -= f > 0 ? 1 : -(rL);
            b += b < rL ? 1 : -rL;
        }

        void Rainbow(string str)
        {
            // The colors of the rainbow in order.
            Color[] rbColors = { Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet };
            int rL = rbColors.Length - 1,
                sL = str.Length,
                f = rL,
                b = 0;            
            for (int i = 0; i < sL; i++)
            {
                UpDown(ref f, ref b, rL);
                con.RawPrintToConsole($"#", false, (i == sL - 1), false, rbColors[f], rbColors[b]);
            }
            foreach (var color in rbColors)
            {
                for (int i = 0; i < sL; i++)
                {
                    UpDown(ref f, ref b, rL);
                    con.RawPrintToConsole($"{str[i]}", false, (i == sL - 1), false, rbColors[f], color);
                }
            }
            Color c = Color.Black;
            meIsLoop:
            foreach (var color in rbColors)
            {
                con.RawPrintToConsole($"{color.Name}", false, false, false, color, c);
                con.RawPrintToConsole($"{color.Name}", false, false, false, c, color);
            }
            con.RawPrintToConsole(null, false, true, true, null, null);
            if (c.Equals(Color.Black))
            {
                c = Color.White;
                goto meIsLoop;
            }
        }

        private void Test1_Click(object sender, RoutedEventArgs e)
        {
            Rainbow("Output using the RawPrintToConsole method.");
        }

        private void Test2_Click(object sender, RoutedEventArgs e)
        {
            ImGonnaPrintSomething("Hello.");
            ImGonnaPrintSomethingTwoTheRevenge("Bye bye.");
            W("I'm gonna watch netflix soon.");
            HoRyShItTo("Like in ten minutes, just the time that i check if the code compile...");
        }
    }
}
