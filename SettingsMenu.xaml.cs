using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
namespace PocketLLM
{
    public partial class SettingsMenu : System.Windows.Controls.UserControl
    {
        public Config.ConfigClass config { get; set; }
        public Popup ParentPopup { get; set; }


        public SettingsMenu()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            config.ApiKey = ApiKeyTextBox.Text.Trim();
            config.Model = ModelTextBox.Text.Trim();
            config.LLMType = LLMTypeTextBox.Text.Trim();
            config.isAutoStart = (bool)AutoRunCheckBox.IsChecked;
            Config.ConfigService.Save(config);

            Config.ConfigService.SetAutoStart(config.isAutoStart);

            if (ParentPopup != null)
                ParentPopup.IsOpen = false;
        }

    }
}
