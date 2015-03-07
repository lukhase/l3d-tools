using L3dStrJahreszeiten.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace L3dStrJahreszeiten
{
    class ReplacementViewModel
    {
        private ICollectionView replacementView_;
        private List<ReplacementEntry> replacements_;
        private bool showInvisible_;

        private ReplacementSet set_;
        private Ersetzungsprojekt proj_;

        public ReplacementViewModel(Ersetzungsprojekt proj, ReplacementSet set)
        {
            set_ = set;
            proj_ = proj;

            var csvFilepath = System.IO.Path.Combine(proj_.ProjectDirectory, set_.CsvFilename);
            replacements_ = ReadFromCsv(csvFilepath).ToList();

            replacementView_ = CollectionViewSource.GetDefaultView(replacements_);
            replacementView_.Filter = EntryFilter;


            ReadObjectFilesCmd = new RelayCommand(obj =>
            {
                var currentEntries = ReplacementsList.ToDictionary(en => en.Original);
                IEnumerable<L3dFilePath> newFiles = Enumerable.Empty<L3dFilePath>();
                foreach (var f in set.Strecken)
                {
                    newFiles = newFiles.Union(ReadFromStrFile(f.OriginalName));
                }
                foreach (var f in newFiles)
                {
                    if (!currentEntries.ContainsKey(f))
                    {
                        currentEntries[f] = new ReplacementEntry { Original = f, DateAdded = DateTime.Now, IsVisible = true,
                            Replacement = new L3dFilePath(ReplacementUtilities.GetAutoReplacement(set_.Ersetzungen, f.AbsolutePath)) };
                    }
                }
                ReplacementsList.Clear();
                ReplacementsList.AddRange(currentEntries.Values);
                NotifyListChanged();
            },
            obj =>
            {
                return set.Strecken.Count > 0;
            });

            ApplyHiddenEntries = new RelayCommand(obj =>
                {
                    var dlg = new ChooseSetDlg(proj_.Sets.Where(s => s != set_));
                    if (dlg.ShowDialog().GetValueOrDefault())
                    {
                        var selected = dlg.SelectedSet;
                        var items = ReadFromCsv(System.IO.Path.Combine(proj_.ProjectDirectory, selected.CsvFilename));
                        foreach (var it in items)
                        {
                            var match = replacements_.Where(en => en.Original == it.Original).FirstOrDefault();
                            if (match != null)
                            {
                                match.IsVisible = it.IsVisible;
                            }
                        }
                        NotifyListChanged();
                    }
                },
                obj =>
                {
                    return proj_.Sets.Count > 1;
                });
        }


        private IEnumerable<ReplacementEntry> ReadFromCsv(string file)
        {
            try
            {
                return CsvFileIO.ReadFromCsv(file);
            }
            catch (Exception)
            {
                //MessageBox.Show("Fehler beim Lesen von CSV-Datei " + file + "\n" + ex.ToString());
            }
            return Enumerable.Empty<ReplacementEntry>();
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

        private bool EntryFilter(object item)
        {
            if (showInvisible_)
            {
                return true;
            }
            ReplacementEntry en = item as ReplacementEntry;
            return en == null || en.IsVisible;
        }

        public IEnumerable<Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogFilter> FileDlgFilters 
        { 
            get { return FileExtensions.ChangeableFiles; } 
        }

        public List<ReplacementEntry> ReplacementsList { get { return replacements_; } }

        public ReplacementSet CurrentSet
        {
            get { return set_; }
        }

        public void NotifyListChanged()
        {
            replacementView_.Refresh();
        }


        public ICollectionView Replacements
        {
            get { return replacementView_; }
        }

        public bool ShowInvisible
        {
            get { return showInvisible_;  }
            set
            {
                if (value != showInvisible_)
                {
                    showInvisible_ = value;
                    replacementView_.Refresh();
                }
            }
        }

        public RelayCommand ReadObjectFilesCmd { get; private set; }

        public RelayCommand ApplyHiddenEntries { get; private set; }
    }
}
