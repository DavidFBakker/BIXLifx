using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BIXLIFX;
using DiffPlex;
using LIFXDevices;
using Newtonsoft.Json;
using Util;

namespace BIXLifxBatchCreator
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly HttpClient Client = new HttpClient();

        private static ObservableCollection<BIXColor> _colors;
        private readonly bool _mainIsLoaded;
        private ObservableCollection<LightBulb> _lightBulbs;
        private ObservableCollection<LIFXCommand> _lifxCommands;

        public MainWindow()
        {
            InitializeComponent();
            _mainIsLoaded = true;
        }

        public static string BaseUrl { get; } = @"http://192.168.85.12:9105/?";


        public ObservableCollection<LightBulb> LightBulbs
        {
            get
            {
                if (_lightBulbs == null)
                {
                    var cmd = "Lights&Json";
                    var page = $"{BaseUrl}{cmd}";

                    var response = Client.GetAsync(page).Result;

                    var contents = response.Content.ReadAsStringAsync().Result;
                    _lightBulbs = JsonConvert.DeserializeObject<ObservableCollection<LightBulb>>(contents);
                    
                }
                return _lightBulbs;
            }
            set => _lightBulbs = value;
        }


        public static ObservableCollection<BIXColor> Colors
        {
            get
            {
                if (_colors == null)
                {
                    var cmd = "Colors&Json";
                    var page = $"{BaseUrl}{cmd}";

                    var response = Client.GetAsync(page).Result;

                    var contents = response.Content.ReadAsStringAsync().Result;
                    _colors = JsonConvert.DeserializeObject<ObservableCollection<BIXColor>>(contents);
                }
                return _colors;
            }
            set => _colors = value;
        }

        private void TxtDim_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            BuildCommand();
        }
      
        private void TxtLabel_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            BuildCommand();
        }

        private string GetMatch(IEnumerable<string> strings)
        {
            var match = "";
            foreach (var str in strings)
                if (string.IsNullOrEmpty(match))
                {
                    match = str;
                }
                else
                {
                    var d = new Differ();
                    var cc = d.CreateCharacterDiffs(match, str, false);
                    if ( match.Length > cc.DiffBlocks[0].InsertStartB)
                        match = match.Remove(cc.DiffBlocks[0].InsertStartB).Trim();
                }
            return match;
        }

        private LIFXCommand GetLIFXCommand(string label)
        {
            var color = "";
            var Dim = -1;
            var power = "";


            if (lstColor.SelectedItems.Count > 0)
            {
                var bc = (BIXColor) lstColor.SelectedItems[0];
                color = bc.Name;
            }

            if (!string.IsNullOrEmpty(txtDim.Text))
                int.TryParse(txtDim.Text, out Dim);

            var test = chkPower.IsChecked;
            if (test != null)
                power = (bool) chkPower.IsChecked ? "On" : "Off";

            var lifx = new LIFXCommand
            {
                Label = label
            };

            if (!string.IsNullOrEmpty(power))
                lifx.Power = power;

            if (Dim > 0)
                lifx.Dim = Dim;

            if (!string.IsNullOrEmpty(color))
                lifx.Color = color;

            return lifx;
        }

        public ObservableCollection<LIFXCommand> LIFXCommands
        {
            get
            {
                if (_lifxCommands == null )
                    _lifxCommands=new ObservableCollection<LIFXCommand>();

                return _lifxCommands;
            }
            
        }

        private List<LIFXCommand> _currentCommands;

        private void BuildCommand()
        {
            if (!_mainIsLoaded)
                return;

            _currentCommands = new List<LIFXCommand>();


            if (lstLights.SelectedItems.Count > 0)
            {
                List<LightBulb> lightbulbs = lstLights.SelectedItems.Cast<LightBulb>().ToList();

                var match = GetMatch(lightbulbs.Select(a => a.Label));
                txtMatch.Text = match;
                if ((bool) chkMatch.IsChecked)
                    _currentCommands.Add(GetLIFXCommand(match));
                else
                    _currentCommands.AddRange(lightbulbs.Select(light => GetLIFXCommand(light.Label)));
            }
            else if (!string.IsNullOrEmpty(txtLabel.Text))
            {
                _currentCommands.Add(GetLIFXCommand(txtLabel.Text));
            }
            if (!_currentCommands.Any())
                return;
            
           
            txtCommand.Text = JsonConvert.SerializeObject(_currentCommands);
        }

        private void LstLights_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BuildCommand();
        }

        private void ChkPower_OnClick(object sender, RoutedEventArgs e)
        {
            int Dim = -1;
            if (!string.IsNullOrEmpty(txtDim.Text))
                int.TryParse(txtDim.Text, out Dim);

            if (chkPower.IsChecked != null)
                if ((bool)chkPower.IsChecked && Dim < 1)
                {
                    txtDim.Text = "100";
                }
            BuildCommand();
        }

        private void LstColor_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BuildCommand();
        }

        private void TxtCommand_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(txtCommand.Text);
        }

        private void ChkMatch_OnClick(object sender, RoutedEventArgs e)
        {
            BuildCommand();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (var lc in _currentCommands)
                LIFXCommands.Add(lc);
            UpdateCommands();

            lstColor.SelectedItems.Clear();
            lstLights.SelectedItems.Clear();

            
        }

        private void Label_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!(lstCommands.SelectedItem is LIFXCommand)) return;

            var toremove = (LIFXCommand)lstCommands.SelectedItem;
            LIFXCommands.Remove(toremove);
            UpdateCommands();
        }

        private void UpdateCommands()
        {
            txtCommands.Text = JsonConvert.SerializeObject(LIFXCommands);
        }
         
        private void TxtCommands_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(txtCommands.Text);
        }

       

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            lstColor.SelectedItem = null;
        }

        private void TestButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(txtCommands.Text))
            {
                 RunTask(txtCommands.Text);
                return;
            }

            if (!String.IsNullOrWhiteSpace(txtCommand.Text))
            {
                 RunTask(txtCommand.Text);
                return;
                }
            }
        private  void RunTask(string arg)
        {
            var page = $"{txtBaseUrl.Text}CMDS={arg}";
            // Console.WriteLine($"Running: {page}");
            var response = Client.GetAsync(page).Result;
            //  var res = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            response.Content.ReadAsStringAsync().ConfigureAwait(false);
            // Console.WriteLine(res);
        }

    }
}