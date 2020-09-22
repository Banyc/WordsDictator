using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WordsDictator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SpeechSynthesizer speech = new SpeechSynthesizer();
        private readonly Random random = new Random();
        private const string voiceIndexPath = "./voiceIndex.txt";
        private const string inputTextPath = "./input.txt";

        public MainWindow()
        {
            InitializeComponent();

            foreach (var voice in this.speech.GetInstalledVoices())
            {
                listSpeechSources.Items.Add(voice.VoiceInfo.Description);
            }
            LoadTextBoxInput();
            LoadVoiceIndex();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            string[] words = textBoxInput.Text.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length <= 0)
            {
                return;
            }
            int selectedIndex = random.Next(0, words.Length);
            string selectedWord = words[selectedIndex];
            textBoxOutput.Text = selectedWord;
            SpeakTextBoxOutputText();
            if (checkBoxIsDeleteAfterSpoken.IsChecked.Value)
            {
                string pattern = $"(?<=[\n\r ]|^){selectedWord}(?=[\n\r ]|$)";
                textBoxInput.Text = Regex.Replace(textBoxInput.Text, pattern, "");
            }
        }

        private void btnSpeak_Click(object sender, RoutedEventArgs e)
        {
            SpeakTextBoxOutputText();
        }

        private void SpeakTextBoxOutputText()
        {
            if (listSpeechSources.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a voice from the listbox", "Warming");
                return;
            }
            var selectedVoice = this.speech.GetInstalledVoices()[listSpeechSources.SelectedIndex];
            this.speech.SelectVoice(selectedVoice.VoiceInfo.Name);
            this.speech.SpeakAsync(textBoxOutput.Text);
        }

        private void btnSaveInput_Click(object sender, RoutedEventArgs e)
        {
            SaveTextBoxInput();
        }

        private void btnLoadInput_Click(object sender, RoutedEventArgs e)
        {
            LoadTextBoxInput();
        }

        private void listSpeechSources_SelectionChanged(object sender, RoutedEventArgs e)
        {
            SaveSelectedVoiceIndex();
        }

        private void SaveTextBoxInput()
        {
            File.WriteAllText(inputTextPath, textBoxInput.Text);
        }

        private void LoadTextBoxInput()
        {
            if (!File.Exists(inputTextPath))
            {
                return;
            }
            textBoxInput.Text = File.ReadAllText(inputTextPath);
        }

        private void SaveSelectedVoiceIndex()
        {
            File.WriteAllText(voiceIndexPath, listSpeechSources.SelectedIndex.ToString());
        }

        private void LoadVoiceIndex()
        {
            if (!File.Exists(voiceIndexPath))
            {
                return;
            }
            listSpeechSources.SelectedIndex = int.Parse(File.ReadAllText(voiceIndexPath));
        }
    }
}
