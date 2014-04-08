using System;
using System.IO;
using Flai.Unity.CopyDLL.Properties;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows;
using System.Windows.Controls;

namespace Flai.Unity.CopyDLL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            FlaiDllTextBlock.Text = Settings.Default["Flai"].ToString();
            FlaiDllEditorTextBlock.Text = Settings.Default["FlaiEditor"].ToString();
            FlaiDllTiledTextBlock.Text = Settings.Default["FlaiTiled"].ToString();
            FlaiUnityPath.Text = Settings.Default["FlaiUnity"].ToString();

            Application.Current.Exit += (sender, args) =>
            {
                Settings.Default["Flai"] = FlaiDllTextBlock.Text;
                Settings.Default["FlaiEditor"] = FlaiDllEditorTextBlock.Text;
                Settings.Default["FlaiTiled"] = FlaiDllTiledTextBlock.Text;
                Settings.Default["FlaiUnity"] = FlaiUnityPath.Text;
                Settings.Default.Save();
            };
        }

        private void OnChangePathButtonClicked(object sender, RoutedEventArgs e)
        {
            TextBox textBox = null;
            if (sender == FlaiDllChangeButton)
            {
                textBox = this.FlaiDllTextBlock;
            }
            else if (sender == FlaiDllEditorChangeButton)
            {
                textBox = FlaiDllEditorTextBlock;
            }
            else if (sender == FlaiDllTiledChangeButton)
            {
                textBox = FlaiDllTiledTextBlock;
            }
            else
            {
                return;
            }

            CommonFileDialog dialog = new CommonOpenFileDialog()
            {
                IsFolderPicker = true,
                DefaultDirectory = "C:/Koodaus/Unity/",
                InitialDirectory = "C:/Koodaus/Unity/",
                EnsurePathExists = true,
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                textBox.Text = dialog.FileName;
            }
            else
            {
                textBox.Text = "";
            }
        }

        private void OnCopyClicked(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(FlaiUnityPath.Text))
            {
                MessageBox.Show("Flai.Unity folder not set!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string unityFolder = FlaiUnityPath.Text;
            string flai = this.FlaiDllTextBlock.Text;
            string flaiEditor = this.FlaiDllEditorTextBlock.Text;
            string flaiTiled = this.FlaiDllTiledTextBlock.Text;

            if (Directory.Exists(flai))
            {
                this.Copy(unityFolder, flai, "Flai.Unity");
            }
            if (Directory.Exists(flaiEditor))
            {
                this.Copy(unityFolder, flaiEditor, "Flai.Unity.Editor");
            }
            if (Directory.Exists(flaiTiled))
            {
                this.Copy(unityFolder, flaiTiled, "Flai.Unity.Tiled");
            }
        }

        private void OnFlaiUnityChangeClicked(object sender, RoutedEventArgs e)
        {
            CommonFileDialog dialog = new CommonOpenFileDialog()
            {
                IsFolderPicker = true,
                DefaultDirectory = "C:/Koodaus/Unity/",
                InitialDirectory = "C:/Koodaus/Unity/",
                EnsurePathExists = true,
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                FlaiUnityPath.Text = dialog.FileName;
            }
        }

        private string GetSourceFileName(string flaiUnityFolderPath, string dllName)
        {
            return Path.Combine(flaiUnityFolderPath, dllName + "/bin/Release/" + dllName + ".dll");
        }

        private void Copy(string unityFolder, string destinationFolder, string dllName)
        {
            try
            {
                File.Copy(this.GetSourceFileName(unityFolder, dllName), Path.Combine(destinationFolder, dllName + ".dll"), true);
            }
            catch (Exception)
            {
                MessageBox.Show("Error!! Probably wrong unity folder!!");
            }
        }
    }
}
