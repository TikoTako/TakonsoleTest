using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using takon = TikoTako.Takonsole;

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

        Action<string> ImGonnaPrintSomething = takon.Out;
        Action<string> ImGonnaPrintSomethingTwoTheRevenge = takon.Inf;
        Action<string> W = takon.Warn;
        Action<string> HoRyShItTo = takon.Err;

        private void ShowErrorDlg(string str) { MessageBox.Show(str, "Error.", MessageBoxButton.OK, MessageBoxImage.Error); }
        private void ShowInfDlg(string str) { MessageBox.Show(str, "Information.", MessageBoxButton.OK, MessageBoxImage.Information); }

        private void Alloc_Click(object sender, RoutedEventArgs e)
        {
            if (takon.Alloc("Testing console"))
            {
                Alloc.IsEnabled = false;
                Enumerable.Range(1, 8).ToList().ForEach(i => ((Button)this.FindName($"Test{i}")).IsEnabled = true);
            }
            else
            {
                ShowErrorDlg("Unable to allocate the console.");
            }
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
                takon.Write("#", null, rbColors[f], rbColors[b]);
            }
            takon.WriteLine();

            foreach (var color in rbColors)
            {
                for (int i = 0; i < sL; i++)
                {
                    UpDown(ref f, ref b, rL);
                    takon.Write("#", null, rbColors[f], color);
                }
                takon.WriteLine();
            }

            Color c = Color.Black;
        meIsLoop:
            foreach (var color in rbColors)
            {
                takon.Write($"{color.Name}", null, color, c);
                takon.Write($"{color.Name}", null, c, color);
            }
            takon.WriteLine();
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
            bool?[] tsF = { null, null, true, false };
            takon.TimeStampFormat.customFormat = "ddd dd MMMM yyyy HH:mm:ss zzz";
            foreach (var item in tsF)
            {
                takon.TimeStampFormat.dtl = item;
                ImGonnaPrintSomething("timestamped normal text");
                takon.Inf("timestamped information text");
                W("timestamped warning text");
                takon.Err("timestamped error text");
                takon.TimeStampFormat.customFormat = ""; // loopderp
            }
        }

        private void Test3_Click(object sender, RoutedEventArgs e)
        {
            string buff;
            var tmpColors = Tuple.Create(takon.NormalColor, takon.BackgroundColor, takon.InformationColor, takon.WarningColor, takon.ErrorColor);
            var arrayOfColors = (typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public));

            Enumerable.Range(0, 255).ToList().ForEach(i => takon.Write($"#", null, Color.FromArgb(i, 0, 0), null));
            Enumerable.Range(0, 255).Reverse().ToList().ForEach(i => takon.Write($"#", null, Color.FromArgb(0, i, 0), null));
            Enumerable.Range(0, 255).ToList().ForEach(i => takon.Write($"#", null, Color.FromArgb(0, 0, i), null));
            Enumerable.Range(0, 255).Reverse().ToList().ForEach(i => takon.Write($"#", null, Color.FromArgb(i, i, 0), null));
            Enumerable.Range(0, 255).ToList().ForEach(i => takon.Write($"#", null, Color.FromArgb(i, 0, i), null));
            Enumerable.Range(0, 255).Reverse().ToList().ForEach(i => takon.Write($"#", null, Color.FromArgb(0, i, i), null));
            Enumerable.Range(0, 255).ToList().ForEach(i => takon.Write($"#", null, Color.FromArgb(i, i, i), null));

            Console.WriteLine(Environment.NewLine);
            Console.WriteLine($"Array of colors (from System.Drawing.Color) = {arrayOfColors.Length}");
            foreach (var bColor in arrayOfColors)
            {
                buff = "";
                foreach (var fColor in arrayOfColors)
                {
                    buff += $"{takon.SetFontColor(Color.FromName(fColor.Name))}{takon.SetBackgroundColor(Color.FromName(bColor.Name))}#{((Console.CursorLeft == Console.WindowWidth) ? Environment.NewLine : "")}";
                }
                Console.Write(buff);
            }
            (takon.NormalColor, takon.BackgroundColor, takon.InformationColor, takon.WarningColor, takon.ErrorColor) = tmpColors;
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine($"done {takon.SetFontColor(Color.BlueViolet)}{takon.SetStyle(takon.Style.Reverse)}DONE{takon.UnSetStyle(takon.Style.Reverse)}{takon.SetFontColor(takon.NormalColor)} done");
        }

        int currentFont = 0;
        List<string> fontsInstalled = null;

        private void Test4_Click(object sender, RoutedEventArgs e)
        {
            if (fontsInstalled != null && fontsInstalled.Count > 0)
            {
                takon.SetFont(fontsInstalled[currentFont], (short)(currentFont + 14));
                takon.Inf($"The font is now \"{fontsInstalled[currentFont]}\", the size is {(currentFont + 14)}");
                currentFont += currentFont == fontsInstalled.Count - 1 ? -currentFont : 1;
            }
            else if (fontsInstalled == null)
            {
                fontsInstalled = new List<string>();
                string[] fonts = { "Consolas", "Courier New", "DejaVu Sans Mono", "Liberation Mono", "Lucida Console", "Source Code Pro Medium" };

                // Font reqire System.Drawing.Common
                fontsInstalled.AddRange(from font in fonts
                                        where new Font(font, 16).Name.Equals(font)
                                        select font);
                ShowInfDlg($"Found {fontsInstalled.Count} fonts:{Environment.NewLine}{String.Join(Environment.NewLine, fontsInstalled)}");
                if(fontsInstalled.Count > 0)
                {
                    Test4_Click(null, null);
                }
            }
            else
            {
                ShowErrorDlg("Can't find any font.");
            }
        }

        object oldColors = Tuple.Create(takon.NormalColor, takon.BackgroundColor, takon.InformationColor, takon.WarningColor, takon.ErrorColor);

        private void Test5_Click(object sender, RoutedEventArgs e)
        {
            object tmpColors = oldColors;
            if (((Button)sender).Name.Equals("Test5"))
            {
                // tmpColors = oldColors;
            }
            else if (((Button)sender).Name.Equals("Test6"))
            {
                tmpColors = Tuple.Create(Color.YellowGreen, Color.PapayaWhip, Color.CornflowerBlue, Color.RosyBrown, Color.OrangeRed);
            }
            else if (((Button)sender).Name.Equals("Test7"))
            {
                tmpColors = Tuple.Create(Color.Ivory, Color.DarkSlateBlue, Color.NavajoWhite, Color.Coral, Color.Tomato);
            }
            (takon.NormalColor, takon.BackgroundColor, takon.InformationColor, takon.WarningColor, takon.ErrorColor) = (Tuple<Color, Color, Color, Color, Color>)tmpColors;
            takon.CLS();
            Test2_Click(sender, e);
        }

        Color InvertColor(Color color)
        {
            return color.Name.Contains("ray") ? Color.Black : Color.FromArgb(255 - color.R, 255 - color.G, 255 - color.B);
        }

        private void Test8_Click(object sender, RoutedEventArgs e)
        {
            takon.CLS();
            takon.WriteLine();
            var arrayOfColors = (typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public));
            var str = "bebs";
            int
                i = 0,
                ii = 0,
                pass = 0,
                totC = arrayOfColors.Length - 1;
            Color
                fColor = Color.GreenYellow,
                bColor = Color.Red;

            for (; i <= totC && pass <= 2;)
            {
                if (pass == 0 || pass == 2)
                {
                    str = new String(' ', arrayOfColors[i].Name.Length + 2);
                    bColor = Color.FromName(arrayOfColors[i].Name);
                }
                else if (pass == 1)
                {
                    bColor = Color.FromName(arrayOfColors[i].Name);
                    str = $" {arrayOfColors[i].Name} ";
                    fColor = InvertColor(bColor);
                }

                if (Console.CursorLeft + str.Length + 2 > Console.WindowWidth)
                {
                    if (pass == 2)
                    {
                        takon.WriteLine();
                        ii = i;
                    }
                    i = ii;
                    takon.WriteLine();
                    pass += pass == 2 ? -2 : 1;
                }
                else if (i == totC)
                {
                    i = ii;
                    pass++;
                    takon.WriteLine();
                }
                else
                {
                    i++;
                    takon.Write(" ");
                    takon.Write(str, null, fColor, bColor);
                }
            }
        }
    }
}
