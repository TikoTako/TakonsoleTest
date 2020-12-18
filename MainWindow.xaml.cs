using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
// Why 2 Color ?
using SDColor = System.Drawing.Color;
using WMColor = System.Windows.Media.Color;

// Shorten Takonsole to Takon
using Takon = TikoTako.Takonsole;

namespace TakonsoleTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Button selectedBox = null;
        object currentConsoleScheme;
        List<string> fontsInstalled = null;
        readonly object originalConsoleColorScheme = Tuple.Create(Takon.TimestampColor, Takon.NormalColor, Takon.BackgroundColor, Takon.InformationColor, Takon.WarningColor, Takon.ErrorColor);
        object consoleCustomScheme1 = Tuple.Create(SDColor.Cyan, SDColor.YellowGreen, SDColor.PapayaWhip, SDColor.CornflowerBlue, SDColor.RosyBrown, SDColor.OrangeRed);
        object consoleCustomScheme2 = Tuple.Create(SDColor.DarkSalmon, SDColor.Ivory, SDColor.DarkSlateBlue, SDColor.NavajoWhite, SDColor.Coral, SDColor.Tomato);

        // 
        readonly Action<string> PrintNormal = Takon.Out;
        readonly Action<string> PrintInfo = Takon.Inf;
        readonly Action<string> PrintWarning = Takon.Warn;
        readonly Action<string> PrintError = Takon.Err;

        public MainWindow()
        {
            InitializeComponent();
            CheckFontsInstalled(false);
            fontsComboBox.ItemsSource = fontsInstalled;
            currentConsoleScheme = originalConsoleColorScheme;
            Title = $"Takonsole test program [{Assembly.GetExecutingAssembly().GetName().Version}]";
            SetBoxesColors();
        }

        public static WMColor ColorToMColor(SDColor color) => WMColor.FromRgb(color.R, color.G, color.B);
        public static SDColor MColorToColor(WMColor color) => SDColor.FromArgb(255, color.R, color.G, color.B);
        private void ShowErrorDlg(string str) => MessageBox.Show(str, "Error.", MessageBoxButton.OK, MessageBoxImage.Error);
        private void ShowWarnDlg(string str) => MessageBox.Show(str, "Attention.", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        private void ShowInfDlg(string str) => MessageBox.Show(str, "Information.", MessageBoxButton.OK, MessageBoxImage.Information);

        private void CheckFontsInstalled(bool loadOutOfSpecificationFonts)
        {
            fontsComboBox.ItemsSource = null;
            // Font reqire System.Drawing.Common
            if (!loadOutOfSpecificationFonts)
            {
                fontsInstalled = new List<string>();
                // Some of the fonts in the windows console properties
                string[] fonts = { "Consolas", "Courier New", "DejaVu Sans Mono", "Liberation Mono", "Lucida Console", "somefontthatdontexists" };
                // check if font exists in the system
                fontsInstalled.AddRange(from font in fonts
                                        where new Font(font, 16).Name.Equals(font)
                                        select font);
                if (fontsInstalled.Count < 0)
                {
                    ShowWarnDlg("Unable to find any usable fonts.");
                }
            }
            else
            {
                fontsInstalled = new List<string>();
                // Get all teh fonts
                Dictionary<string, bool> leFonts = new Dictionary<string, bool>();
                foreach (var item in Fonts.SystemTypefaces)
                {
                    if (!leFonts.ContainsKey(item.FontFamily.Source))
                    {
                        leFonts.Add(item.FontFamily.Source, true);
                    }
                }
                fontsInstalled.AddRange(leFonts.Keys);
            }
            fontsInstalled.Sort();
            fontsComboBox.ItemsSource = fontsInstalled;
        }

        static public void EnableDisableAllTheControls(Visual vis, bool isEnabled)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(vis); i++)
            {
                Visual gioacchino = (Visual)VisualTreeHelper.GetChild(vis, i);
                if (gioacchino is Button butan && !(butan.Name.Equals("AllocDeallocButton")))
                {
                    butan.IsEnabled = isEnabled;
                }
                else if (gioacchino is RadioButton radioButan)
                {
                    radioButan.IsEnabled = isEnabled;
                }
                else if (gioacchino is Slider slidy)
                {
                    slidy.IsEnabled = isEnabled;
                }
                else if (gioacchino is ComboBox comby)
                {
                    comby.IsEnabled = isEnabled;
                }
                else
                {
                    EnableDisableAllTheControls(gioacchino, isEnabled);
                }
            }
        }

        private void AllocDeallocButton_Click(object sender, RoutedEventArgs e)
        {
            bool doAlloc;
            if (doAlloc = AllocDeallocButton.Content.ToString().Equals("Alloc()"))
            {
                if (!Takon.Alloc($"Takonsole [{Assembly.GetAssembly(typeof(Takon)).GetName().Version}]"))
                {
                    ShowErrorDlg("Unable to allocate the console.");
                    return;
                }
                fontsSizeComboBox.SelectedIndex = 2;
                fontsComboBox.SelectedIndex = 0;
            }
            else
            {
                Takon.DeAlloc();
            }
            AllocDeallocButton.Content = doAlloc ? "Dealloc()" : "Alloc()";
            EnableDisableAllTheControls(this, doAlloc);
        }

        SDColor BoW(SDColor rob) { return GetLuminance(rob) > 127 ? SDColor.Black : SDColor.White; }

        /// <summary>
        /// Rainbow
        /// </summary>        
        private void Test1_Click(object sender, RoutedEventArgs e)
        {
            //Rainbow("Output using the RawPrintToConsole method.");
            // red, orange, yellow, green, blue, indigo and violet
            string _s, _ss;
            bool _l = true;
            List<SDColor> Rainbow = new List<SDColor> { SDColor.Red, SDColor.Orange, SDColor.Yellow, SDColor.Green, SDColor.Blue, SDColor.Indigo, SDColor.Violet };
        ThisIsBASICnotCSharp:
            _s = _ss = "";
            foreach (var _c in Rainbow)
            {
                _s += Takon.GenerateRawFromString($"{_c.Name}", null, BoW(_c), _c) + " ";
                _ss += Takon.GenerateRawFromString($"{_c.Name}", null, _c, BoW(_c)) + " ";
            }
            Takon.WriteLine($"{_s}{Environment.NewLine}{_ss}");
            if (_l)
            {
                _l = false;
                Rainbow.Reverse();
                goto ThisIsBASICnotCSharp;
            }
        }

        /// <summary>
        /// Out, Inf, Warn, Err
        /// </summary>
        private void Test2_Click(object sender, RoutedEventArgs e)
        {
            bool?[] tsF = { null, null, true, false };
            Takon.TimeStampFormat.customFormat = "ddd dd MMMM yyyy HH:mm:ss zzz";
            foreach (var item in tsF)
            {
                Takon.TimeStampFormat.dtl = item;
                PrintNormal("timestamped normal text");
                Takon.Inf("timestamped information text");
                PrintWarning("timestamped warning text");
                Takon.Err("timestamped error text");
                Takon.TimeStampFormat.customFormat = ""; // loopderp
            }
        }

        void SetConsoleColorScheme(object colorScheme)
        {
            // maybe is better to put a get/set theme directly in takonsole
            (Takon.TimestampColor, Takon.NormalColor, Takon.BackgroundColor, Takon.InformationColor, Takon.WarningColor, Takon.ErrorColor) =
                (Tuple<SDColor, SDColor, SDColor, SDColor, SDColor, SDColor>)colorScheme;
        }

        /// <summary>
        /// Lots of colors
        /// </summary>
        private void Test3_Click(object sender, RoutedEventArgs e)
        {
            string buff;
            var arrayOfColors = (typeof(SDColor).GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public));

            Enumerable.Range(0, 255).ToList().ForEach(i => Takon.Write($"#", null, SDColor.FromArgb(i, 0, 0), null));
            Enumerable.Range(0, 255).Reverse().ToList().ForEach(i => Takon.Write($"#", null, SDColor.FromArgb(0, i, 0), null));
            Enumerable.Range(0, 255).ToList().ForEach(i => Takon.Write($"#", null, SDColor.FromArgb(0, 0, i), null));
            Enumerable.Range(0, 255).Reverse().ToList().ForEach(i => Takon.Write($"#", null, SDColor.FromArgb(i, i, 0), null));
            Enumerable.Range(0, 255).ToList().ForEach(i => Takon.Write($"#", null, SDColor.FromArgb(i, 0, i), null));
            Enumerable.Range(0, 255).Reverse().ToList().ForEach(i => Takon.Write($"#", null, SDColor.FromArgb(0, i, i), null));
            Enumerable.Range(0, 255).ToList().ForEach(i => Takon.Write($"#", null, SDColor.FromArgb(i, i, i), null));

            Console.WriteLine(Environment.NewLine);
            Console.WriteLine($"Array of colors (from System.Drawing.Color) = {arrayOfColors.Length}");
            foreach (var bColor in arrayOfColors)
            {
                buff = "";
                foreach (var fColor in arrayOfColors)
                {
                    buff += $"{Takon.SetFontColor(SDColor.FromName(fColor.Name))}{Takon.SetBackgroundColor(SDColor.FromName(bColor.Name))}#{((Console.CursorLeft == Console.WindowWidth) ? Environment.NewLine : "")}";
                }
                Console.Write(buff);
            }
            SetConsoleColorScheme(originalConsoleColorScheme);
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine($"done {Takon.SetFontColor(SDColor.BlueViolet)}{Takon.SetStyle(Takon.Style.Reverse)}DONE{Takon.UnSetStyle(Takon.Style.Reverse)}{Takon.SetFontColor(Takon.NormalColor)} done");
        }

        void SetBoxesColors()
        {
            if (currentConsoleScheme != null)
            {
                // (TimestampColor, NormalColor, BackgroundColor, InformationColor, WarningColor, ErrorColor);
                var bob = (Tuple<SDColor, SDColor, SDColor, SDColor, SDColor, SDColor>)currentConsoleScheme;
                boxTimeStamp.Background = new SolidColorBrush(ColorToMColor(bob.Item1));
                boxNormal.Background = new SolidColorBrush(ColorToMColor(bob.Item2));
                boxBackground.Background = new SolidColorBrush(ColorToMColor(bob.Item3));
                boxInformation.Background = new SolidColorBrush(ColorToMColor(bob.Item4));
                boxWarning.Background = new SolidColorBrush(ColorToMColor(bob.Item5));
                boxError.Background = new SolidColorBrush(ColorToMColor(bob.Item6));
            }
        }

        object GetBoxesColors()
        {
            // (TimestampColor, NormalColor, BackgroundColor, InformationColor, WarningColor, ErrorColor);
            return Tuple.Create(
                MColorToColor(((SolidColorBrush)boxTimeStamp.Background).Color),
                MColorToColor(((SolidColorBrush)boxNormal.Background).Color),
                MColorToColor(((SolidColorBrush)boxBackground.Background).Color),
                MColorToColor(((SolidColorBrush)boxInformation.Background).Color),
                MColorToColor(((SolidColorBrush)boxWarning.Background).Color),
                MColorToColor(((SolidColorBrush)boxError.Background).Color)
                );
        }

        private void SchemeButans_Checked(object sender, RoutedEventArgs e)
        {
            if (this.IsInitialized && e.Source is RadioButton rB)
            {
                var _msg = $"You can change the colors of {Takon.SetStyle(Takon.Style.Reverse)}Custom color scheme {{0}}{Takon.UnSetStyle(Takon.Style.Reverse)} by clicking on the color boxes then moving the RGB sliders.{Environment.NewLine}";
                _msg += $"Press {Takon.SetStyle(Takon.Style.Reverse)}Apply{Takon.UnSetStyle(Takon.Style.Reverse)} to set this color scheme.";
                if (rB.Tag.Equals("1"))
                {
                    currentConsoleScheme = originalConsoleColorScheme;
                    sliderR.IsEnabled = sliderG.IsEnabled = sliderB.IsEnabled = false;
                    PrintInfo("Changing the default colors is disabled, press \"Apply\" to set this color scheme.");
                }
                else if (rB.Tag.Equals("2"))
                {
                    Takon.WriteLine(String.Format(_msg, 1));
                    currentConsoleScheme = consoleCustomScheme1;
                    sliderR.IsEnabled = sliderG.IsEnabled = sliderB.IsEnabled = true;
                }
                else if (rB.Tag.Equals("3"))
                {
                    Takon.WriteLine(String.Format(_msg, 2));
                    currentConsoleScheme = consoleCustomScheme2;
                    sliderR.IsEnabled = sliderG.IsEnabled = sliderB.IsEnabled = true;
                }
                SetBoxesColors();
            }
        }

        /// <summary>
        /// Change color scheme.
        /// </summary>
        private void Test7_Click(object sender, RoutedEventArgs e)
        {
            var mhyheadhurts = "Color scheme set to: ";
            currentConsoleScheme = GetBoxesColors();
            if (currentConsoleScheme.Equals(originalConsoleColorScheme))
            {
                mhyheadhurts += "Normal";
            }
            else if (!currentConsoleScheme.Equals(originalConsoleColorScheme) && schemeButan2.IsChecked == true)
            {
                mhyheadhurts += "Custom 1";
                consoleCustomScheme1 = currentConsoleScheme;
            }
            else if (!currentConsoleScheme.Equals(originalConsoleColorScheme) && schemeButan3.IsChecked == true)
            {
                mhyheadhurts += "Custom 2";
                consoleCustomScheme2 = currentConsoleScheme;
            }

            SetConsoleColorScheme(currentConsoleScheme);
            Takon.CLS();
            Takon.WriteLine(mhyheadhurts + Environment.NewLine, Takon.Style.Reverse, null, null);
            Test2_Click(null, null);
        }

        /// <summary>
        /// Calculate the luminance of a color.
        /// <para>https://en.wikipedia.org/wiki/Relative_luminance</para>
        /// </summary>
        /// <param name="color">Color in.</param>
        /// <returns>Luminance value</returns>
        byte GetLuminance(SDColor color)
        {
            return (byte)(
                    (0.2126 * color.R) +
                    (0.7152 * color.G) +
                    (0.0722 * color.B));
        }

        /// <summary>
        /// Pick all the color in Colors and print them out in a box.
        /// </summary>
        private void Test8_Click(object sender, RoutedEventArgs e)
        {
            Takon.CLS();
            Takon.WriteLine();

            var arrayOfColors = (typeof(SDColor).GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public));
            var str = "hosonno";
            int
                i = 0,
                ii = 0,
                pass = 0,
                totC = arrayOfColors.Length - 1;
            SDColor
                fColor = SDColor.GreenYellow,
                bColor = SDColor.Red;

            for (; i <= totC && pass <= 2;)
            {
                if (pass == 0 || pass == 2)
                {
                    str = new String(' ', arrayOfColors[i].Name.Length + 2);
                    bColor = SDColor.FromName(arrayOfColors[i].Name);
                }
                else if (pass == 1)
                {
                    bColor = SDColor.FromName(arrayOfColors[i].Name);
                    str = $" {arrayOfColors[i].Name} ";
                    fColor = GetLuminance(bColor) > 127 ? SDColor.Black : SDColor.White;
                }

                if (Console.CursorLeft + str.Length + 2 > Console.WindowWidth)
                {
                    if (pass == 2)
                    {
                        Takon.WriteLine();
                        ii = i;
                    }
                    i = ii;
                    Takon.WriteLine();
                    pass += pass == 2 ? -2 : 1;
                }
                else if (i == totC)
                {
                    i = ii;
                    pass++;
                    Takon.WriteLine();
                }
                else
                {
                    i++;
                    Takon.Write(" ");
                    Takon.Write(str, null, fColor, bColor);
                }
            }
        }

        private void FontsComboBoxes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (fontsComboBox.SelectedIndex >= 0 && fontsSizeComboBox.SelectedIndex >= 0)
            {
                var _f = (string)fontsComboBox.SelectedItem;
                var _s = short.Parse(((ComboBoxItem)fontsSizeComboBox.SelectedItem).Content.ToString());
                try
                {
                    Takon.SetFont(_f, _s);
                    PrintWarning($"The font is now: {Takon.SetStyle(Takon.Style.Reverse)}{_f}{Takon.UnSetStyle(Takon.Style.Reverse)} size {Takon.SetStyle(Takon.Style.Bold | Takon.Style.Reverse)}{_s}{Takon.UnSetStyle(Takon.Style.Bold | Takon.Style.Reverse)}");
                }
                catch (Exception ex)
                {
                    PrintError(ex.Message);
                    PrintError($"Cannot set the font to: {Takon.SetStyle(Takon.Style.Reverse)}{_f}{Takon.UnSetStyle(Takon.Style.Reverse)} size {Takon.SetStyle(Takon.Style.Bold | Takon.Style.Reverse)}{_s}{Takon.UnSetStyle(Takon.Style.Bold | Takon.Style.Reverse)}");
                }
            }
        }

        private void ColorBox_Click(object sender, RoutedEventArgs e)
        {
            selectedBox = (Button)sender;
            WMColor _backColor = ((SolidColorBrush)selectedBox.Background).Color;
            sliderR.Value = _backColor.R;
            sliderG.Value = _backColor.G;
            sliderB.Value = _backColor.B;
        }

        private void ColorBox_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender is Button leButan)
            {
                SDColor c;
                var _c = GetLuminance(MColorToColor(((SolidColorBrush)leButan.Background).Color)) > 127;
                var _c2 = GetLuminance(MColorToColor(((SolidColorBrush)this.Background).Color)) > 127;
                if (_c && _c2)
                {
                    c = SDColor.Black;
                }
                else if (!_c && !_c2)
                {
                    c = SDColor.White;
                }
                else
                {
                    c = SDColor.Red; // TODO pick some color lebutanback > & < windowback
                }
                leButan.BorderBrush = new SolidColorBrush(ColorToMColor(c));
            }
        }

        private void ColorBox_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!((Button)sender).Equals(selectedBox))
            {
                ((Button)sender).BorderBrush = new SolidColorBrush(ColorToMColor(SDColor.Transparent));
            }
        }

        private void Sliders_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (sender is Slider slider)
            {
                slider.Value += e.Delta < 0 ? -10 : 10;
            }
        }

        private void Sliders_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (selectedBox != null)
            {
                selectedBox.Background = new SolidColorBrush(WMColor.FromRgb((byte)sliderR.Value, (byte)sliderG.Value, (byte)sliderB.Value));
            }
        }

        private void LoadAllTheFonts_Click(object sender, RoutedEventArgs e)
        {
            CheckFontsInstalled(true);
        }
    }
}
