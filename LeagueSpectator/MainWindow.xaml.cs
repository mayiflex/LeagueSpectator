using mayLCU;
using Newtonsoft.Json;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Management;

namespace LeagueSpectator {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private LCU lcu = null;

        private async void Button_Spectate_Click(object sender, RoutedEventArgs e) {
            if (lcu == null) lcu = LCU.HookLeagueClient();
            string name = "", tag = "";
            name = TextBox_Name.Text;
            tag = TextBox_Tag.Text;
            dynamic profile;
            try {
                profile = await lcu.RequestDynamicAsync($"/lol-summoner/v1/summoners?name={name}%23{tag}");
            } catch {
                MessageBox.Show("Unable to fetch summoner.");
                return;
            }
            string puuid = profile.puuid;
            if(puuid == null) {
                MessageBox.Show("Summoner not found.");
                return;
            }
            var spectate = await lcu.RequestAsync(RequestMethod.POST, "lol-spectator/v1/spectate/launch", $"{{\"allowObserveMode\": \"ALL\", \"dropInSpectateGameId\": \"\", \"gameQueueType\": \"\",  \"puuid\": \"{profile.puuid}\"}}");
        }

        private void TextBox_Name_TextChanged(object sender, TextChangedEventArgs e) {
            if (!TextBox_Name.Text.Contains('#')) return;
            var hashtagIndex = TextBox_Name.Text.IndexOf('#');
            var name = TextBox_Name.Text.Substring(0, hashtagIndex);
            var tag = TextBox_Name.Text.Substring(hashtagIndex + 1);
            TextBox_Name.Text = name;
            TextBox_Tag.Text = tag;
            TextBox_Tag.Focus();
            TextBox_Tag.Select(TextBox_Tag.Text.Length,0);
        }
    }
}