using System.Linq;
using Flai.Unity.CopyDLL.Properties;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Flai.Unity.CopyDLL
{
    public class FlaiPaths
    {
        public string Flai { get; set; }
        public string FlaiEditor { get; set; }
        public string FlaiTiled { get; set; }

        public void Write(TextWriter writer)
        {
            writer.WriteLine(this.Flai ?? "");
            writer.WriteLine(this.FlaiEditor ?? "");
            writer.WriteLine(this.FlaiTiled ?? "");
        }

        public static FlaiPaths Read(TextReader reader)
        {
            return new FlaiPaths { Flai = reader.ReadLine(), FlaiEditor = reader.ReadLine(), FlaiTiled = reader.ReadLine() };
        }
    }

    public class PresetCollection
    {
        private const string FilePath = "Presets.txt";
        private readonly List<string> _presets = new List<string>(); // top keep order
        private readonly Dictionary<string, FlaiPaths> _presetToPathsDictionary = new Dictionary<string, FlaiPaths>();

        public IReadOnlyCollection<string> Presets
        {
            get { return _presets.AsReadOnly(); }
        }

        public void Load()
        {
            _presets.Clear();
            _presetToPathsDictionary.Clear();
            if (File.Exists(FilePath))
            {
                using (var stream = File.Open(FilePath, FileMode.Open))
                {
                    using (TextReader reader = new StreamReader(stream))
                    {
                        int count = int.Parse(reader.ReadLine());
                        reader.ReadLine(); // empty line

                        for (int i = 0; i < count; i++)
                        {
                            string preset = reader.ReadLine();
                            _presets.Add(preset);
                            _presetToPathsDictionary.Add(preset, FlaiPaths.Read(reader));
                            reader.ReadLine(); // empty line
                        }
                    }
                }
            }
        }

        public void Save()
        {
            using (var stream = File.Open(FilePath, FileMode.Create))
            {
                using (TextWriter writer = new StreamWriter(stream))
                {
                    writer.WriteLine(_presets.Count);
                    writer.WriteLine(""); // empty line
                    foreach (string preset in _presets)
                    {
                        writer.WriteLine(preset);
                        _presetToPathsDictionary[preset].Write(writer);
                        writer.WriteLine(""); // empty line
                    }
                }
            }
        }

        public bool Contains(string name)
        {
            return _presetToPathsDictionary.ContainsKey(name);
        }

        public void Add(string name)
        {
            _presetToPathsDictionary.Add(name, new FlaiPaths());
            _presets.Add(name);
        }

        public FlaiPaths this[string preset]
        {
            get { return _presetToPathsDictionary[preset]; }
        }

        public void Remove(string preset)
        {
            _presets.Remove(preset);
            _presetToPathsDictionary.Remove(preset);
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly PresetCollection _presetCollection = new PresetCollection();
        public MainWindow()
        {
            InitializeComponent();

            _presetCollection.Load();
            foreach (string preset in _presetCollection.Presets.Reverse())
            {
                _presetComboBox.Items.Insert(1, preset);
            }

            FlaiUnityPath.Text = Settings.Default["Flai"].ToString();
            var defaultPreset = Settings.Default["Preset"].ToString();
            if (_presetCollection.Contains(defaultPreset))
            {
                this.LoadPreset(defaultPreset);
                _presetComboBox.SelectedItem = _presetComboBox.Items.OfType<string>().First(name => name == defaultPreset);
            }

            Application.Current.Exit += (sender, args) =>
            {
                Settings.Default["Flai"] = FlaiUnityPath.Text;
                Settings.Default.Save();
                _presetCollection.Save();
            };
        }

        private void LoadPreset(string preset)
        {
            var presetPaths = _presetCollection[preset];
            FlaiDllEditorTextBlock.Text = presetPaths.FlaiEditor;
            FlaiDllTiledTextBlock.Text = presetPaths.FlaiTiled;
            FlaiDllTextBlock.Text = presetPaths.Flai;
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
                DefaultDirectory = @"C:\Koodaus\Unity",
                InitialDirectory = @"C:\Koodaus\Unity",
                EnsurePathExists = true,
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                textBox.Text = dialog.FileName;
                this.SaveCurrentPresets();
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
                DefaultDirectory = @"C:\Koodaus\Unity",
                InitialDirectory = @"C:\Koodaus\Unity",
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

        private void OnPresetComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var item = e.AddedItems[0] as ComboBoxItem;
                if (item == null || item.Content == null)
                {
                    if (e.AddedItems[0] is string)
                    {
                        this.LoadPreset(e.AddedItems[0] as string);
                    }

                    return;
                }

                var contentName = item.Content.ToString();
                if (contentName == "Add New Item")
                {
                    AddNewItemDialog dialog = new AddNewItemDialog();
                    if (dialog.ShowDialog() == true)
                    {
                        var name = dialog.EnteredName;
                        if (_presetCollection.Contains(name) || string.IsNullOrWhiteSpace(name))
                        {
                            MessageBox.Show("Invalid name", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        {
                            _presetCollection.Add(name);
                            _presetComboBox.Items.Insert(_presetComboBox.Items.Count - 1, name);
                            _presetComboBox.SelectedIndex = _presetComboBox.Items.Count - 2;
                            return;
                        }
                    }

                    if (e.RemovedItems.Count > 0)
                    {
                        _presetComboBox.SelectedItem = e.RemovedItems[0];
                    }
                }
                else if (contentName == "")
                {
                    if (e.RemovedItems.Count > 0)
                    {
                        _presetComboBox.SelectedItem = e.RemovedItems[0];
                    }
                }
            }
        }

        private void OnSavePresetClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                this.SaveCurrentPresets();
            }
            catch (Exception)
            {
                MessageBox.Show("Something failed while saving");
            }
        }

        private void SaveCurrentPresets()
        {
            var selectedItem = _presetComboBox.SelectedItem as string;
            if (!string.IsNullOrWhiteSpace(selectedItem))
            {
                if (_presetCollection.Contains(selectedItem))
                {
                    var paths = _presetCollection[selectedItem];
                    paths.Flai = FlaiDllTextBlock.Text;
                    paths.FlaiEditor = FlaiDllEditorTextBlock.Text;
                    paths.FlaiTiled = FlaiDllTiledTextBlock.Text;
                    _presetCollection.Save();
                }
            }
        }

        private void OnRemovePresetClicked(object sender, RoutedEventArgs e)
        {
            _presetCollection.Remove(_presetComboBox.SelectedItem as string);
            _presetComboBox.Items.Remove(_presetComboBox.SelectedItem);
        }
    }
}
