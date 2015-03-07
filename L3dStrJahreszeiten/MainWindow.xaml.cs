using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Xml.Serialization;

namespace L3dStrJahreszeiten
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ReplacementViewModel replacementViewModel_;
        private ProjectsViewModel projViewModel_;
        private ReplacementSetsViewModel setsViewModel_;
        private FilesViewModel filesViewModel_;
        private WorkViewModel workViewModel_;

        private Ersetzungsprojekt curProj_;
        private ReplacementSet curSet_;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //SettingsWindow sw = new SettingsWindow();
            //sw.ShowDialog();
            L3dFilePath.LoksimDirectory = new L3dFilePath(@"D:\LoksimDevelopment\Loksim-Data");

            projViewModel_ = new ProjectsViewModel();
            tabProject.DataContext = projViewModel_;
            projViewModel_.Projekte = AppSettings.DefaultSettings.RecentProjekte;

            projViewModel_.LoadProjEvent += projViewModel__LoadProjEvent;

            /*
            Dictionary<string, string> reps = null;
            try
            {
                string inPath = @"D:\LoksimDevelopment\Loksim-Data\Strecken\DevStrecken\SimpleStrecke.l3dstr";
                //string outPath = @"D:\LoksimDevelopment\Loksim-Data\Strecken\DevStrecken\SimpleStrecke_tooltest.l3dstr";
                var files = L3dFileIO.ReadFromStrFile(inPath).ToList();
                reps = new Dictionary<string, string>();
                reps[files[0]] = files[0] + "xyz";
                reps[files[1]] = files[1] + "xyzdf";
                //L3dFileIO.ReplaceFilesInStr(inPath, outPath, reps);

                viewModel_ = new ReplacementViewModel(reps.Select(en => new ReplacementEntry
                { 
                    Original = en.Key, 
                    Replacement = en.Value, 
                    DateAdded = DateTime.Now 
                }).ToList());
                DataContext = viewModel_;

                CsvFileIO.WriteToCsv(viewModel_.Replacements.Cast<ReplacementEntry>(), @"D:\LoksimDevelopment\Loksim-Data\Strecken\DevStrecken\out.csv");
            } 
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
             */
        }

        void projViewModel__LoadProjEvent(Ersetzungsprojekt proj)
        {
            curProj_ = proj;
            setsViewModel_ = new ReplacementSetsViewModel(proj);
            tabSets.DataContext = setsViewModel_;
            setsViewModel_.ChangeSetEvent += setsViewModel__ChangeSetEvent;
            L3dFilePath.LoksimDirectory = new L3dFilePath(proj.LoksimDirectory);
        }

        void setsViewModel__ChangeSetEvent(ReplacementSet set)
        {
            SaveAll();
            
            curSet_ = set;
            filesViewModel_ = new FilesViewModel(set);
            tabFiles.DataContext = filesViewModel_;

            replacementViewModel_ = new ReplacementViewModel(curProj_, set);
            tabErsetzungen.DataContext = replacementViewModel_;

            workViewModel_ = new WorkViewModel(set, replacementViewModel_.ReplacementsList);
            tabWork.DataContext = workViewModel_;
        }

        private string GetCurrentCsvFilepath()
        {
            return System.IO.Path.Combine(curProj_.ProjectDirectory, curSet_.CsvFilename);
        }

        private void SaveAll()
        {
            try
            {
                AppSettings.DefaultSettings.RecentProjekte = new List<Ersetzungsprojekt>(projViewModel_.Projekte);
                AppSettings.DefaultSettings.SaveToDefaultXml();

                if (curProj_ != null && !string.IsNullOrWhiteSpace(curProj_.ProjectDirectory))
                {
                    XmlSerializer serializer = new XmlSerializer(curProj_.GetType());
                    string savePath = System.IO.Path.Combine(curProj_.ProjectDirectory, "l3dstrjahreszeitenproj.xml");
                    using (StreamWriter sw = new StreamWriter(savePath))
                    {
                        serializer.Serialize(sw, curProj_);
                    }
                }
                if (replacementViewModel_ != null)
                {
                    SaveToCsv(GetCurrentCsvFilepath(), replacementViewModel_.ReplacementsList);
                }
            } 
            catch(Exception ex)
            {
                MessageBox.Show("Fehler beim Speichern der Daten: " + ex);
            }
        }

        private IEnumerable<L3dFilePath> ReadFromStrFile(string file)
        {
            try
            {
                return L3dFileIO.ReadFromStrFile(file);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Lesen der Streckendatei " + file + "\n" + ex.ToString());
            }
            return Enumerable.Empty<L3dFilePath>();
        }

        /*
        private void AddObj_Click(object sender, RoutedEventArgs e)
        {
            var currentEntries = viewModel_.ReplacementsList.ToDictionary(en => en.Original);
            OpenFileDialog opf = new OpenFileDialog();
            opf.Multiselect = true;
            opf.Filter = "Streckendateien (*.l3dstr) | *.l3dstr";
            if (opf.ShowDialog(this).GetValueOrDefault(false))
            {
                IEnumerable<L3dFilePath> newFiles = Enumerable.Empty<L3dFilePath>();
                foreach (var f in opf.FileNames)
                {
                    newFiles = newFiles.Union(ReadFromStrFile(f));
                }
                foreach (var f in newFiles)
                {
                    if (!currentEntries.ContainsKey(f))
                    {
                        currentEntries[f] = new ReplacementEntry { Original = f, DateAdded = DateTime.Now, IsVisible = true };
                    }
                }
                viewModel_.ReplacementsList.Clear();
                viewModel_.ReplacementsList.AddRange(currentEntries.Values);
                viewModel_.NotifyListChanged();
            }
        }
        */

        private void SaveToCsv(string filename, IEnumerable<ReplacementEntry> replacements)
        {
            try
            {
                CsvFileIO.WriteToCsv(replacements, filename);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Schreiben in die CSV-Datei " + filename + "\n" + ex.ToString());
            }
        }

        /*
        private void SaveToCsv_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog svf = new SaveFileDialog();
            svf.DefaultExt = ".csv";
            svf.Filter = "CSV-Dateien (*.csv) | *.csv";
            if (svf.ShowDialog().GetValueOrDefault(false))
            {
                SaveToCsv(svf.FileName);
            }
        }

        private IEnumerable<ReplacementEntry> ReadFromCsv(string file)
        {
            try
            {
                return CsvFileIO.ReadFromCsv(file);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Lesen von CSV-Datei " + file + "\n" + ex.ToString());
            }
            return Enumerable.Empty<ReplacementEntry>();
        }
         */

        /*
        private void LoadFromCsv(string csvFilename)
        {
            OpenFileDialog opf = new OpenFileDialog();
            opf.Multiselect = false;
            opf.Filter = "CSV-Dateien (*.csv) | *.csv";
            if (opf.ShowDialog(this).GetValueOrDefault(false))
            {
                var newEntries = ReadFromCsv(opf.FileName);
                var newList = viewModel_.ReplacementsList.Union(newEntries, new ReplacementEntryComparer()).ToList();

                viewModel_.ReplacementsList.Clear();
                viewModel_.ReplacementsList.AddRange(newList);
                viewModel_.NotifyListChanged();
            }
        }
         */

        /*
        private void ReplaceInStrFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog();
            opf.Title = "Basisstreckendatei auswählen";
            opf.Multiselect = false;
            opf.Filter = "Streckendatei (*.l3dstr) | *.l3dstr";
            if (!opf.ShowDialog(this).GetValueOrDefault(false))
            {
                return;
            }
            var srcFile = opf.FileName;
            SaveFileDialog svf = new SaveFileDialog();
            svf.Title = "Neue Strecke speichern in";
            svf.Filter = "Streckendatei (*.l3dstr) | *.l3dstr";
            svf.DefaultExt = ".l3dstr";
            if (svf.ShowDialog().GetValueOrDefault(false))
            {
                try
                {
                    L3dFileIO.ReplaceFilesInStr(srcFile, svf.FileName, 
                        viewModel_.ReplacementsList.Where(en => 
                            en.Replacement != null && !string.IsNullOrWhiteSpace(en.Replacement.AbsolutePath)).
                            ToDictionary(en => en.Original, en => en.Replacement));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler beim Durchführen der Ersetzungen\n" + ex.ToString());
                }
            }
        }
        */

        private void mainWnd_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveAll();
        }

        private void MenuSave_Click(object sender, RoutedEventArgs e)
        {
            SaveAll();
        }
    }
}
