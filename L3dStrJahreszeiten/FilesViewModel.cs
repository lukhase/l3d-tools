using L3dStrJahreszeiten.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Xml.Linq;

namespace L3dStrJahreszeiten
{
    public class FilesViewModel : BaseModel
    {
        private ReplacementSet set_;

        private ICollectionView fahrplaeneView_;
        private ICollectionView kursbuchstreckenView_;
        private ICollectionView streckenView_;

        private ReplacementSet.Replacement selectedFahrplan_;
        private ReplacementSet.Replacement selectedKursbuchstrecke_;
        private ReplacementSet.Replacement selectedStrecke_;
    
        public FilesViewModel(ReplacementSet set)
        {
            set_ = set;

            fahrplaeneView_ = CollectionViewSource.GetDefaultView(set_.Fahrplaene);
            kursbuchstreckenView_ = CollectionViewSource.GetDefaultView(set_.Kursbuchstrecken);
            streckenView_ = CollectionViewSource.GetDefaultView(set_.Strecken);

            SearchForKbsAndStrCmd = new RelayCommand(obj =>
                {
                    AutoSearchForFiles();
                    kursbuchstreckenView_.Refresh();
                    streckenView_.Refresh();
                },
                obj =>
                {
                    return set_.Fahrplaene.Count > 0;
                });

            #region Fpl Commands
            AddFplCmd = new RelayCommand(obj =>
                {
                    var f = DialogHelpers.OpenLoksimFile((string)null, FileExtensions.FahrplanFiles, null);
                    if (!string.IsNullOrEmpty(f))
                    {
                        var autoRepl = GetAutoReplacement(f);
                        set_.Fahrplaene.Add(new ReplacementSet.Replacement { OriginalName = f, NewName = autoRepl });
                        fahrplaeneView_.Refresh();
                    }
                },
                obj =>
                {
                    return true;
                });

            DeleteFplCmd = new RelayCommand(obj =>
                {
                    if (selectedFahrplan_ != null)
                    {
                        set_.Fahrplaene.Remove(selectedFahrplan_);
                        fahrplaeneView_.Refresh();
                    }
                },
                obj =>
                {
                    return selectedFahrplan_ != null;
                });
            #endregion

            #region Kursbuchstrecken Commands
            AddKursbuchstreckeCmd = new RelayCommand(obj =>
            {
                var f = DialogHelpers.OpenLoksimFile((string)null, FileExtensions.KursbuchstreckenFiles, null);
                if (!string.IsNullOrEmpty(f))
                {
                    var autoRepl = GetAutoReplacement(f);
                    set_.Kursbuchstrecken.Add(new ReplacementSet.Replacement { OriginalName = f, NewName = autoRepl });
                    kursbuchstreckenView_.Refresh();
                }
            },
                obj =>
                {
                    return true;
                });

            DeleteKursbuchstreckeCmd = new RelayCommand(obj =>
            {
                if (selectedKursbuchstrecke_ != null)
                {
                    set_.Kursbuchstrecken.Remove(selectedKursbuchstrecke_);
                    kursbuchstreckenView_.Refresh();
                }
            },
                obj =>
                {
                    return selectedKursbuchstrecke_ != null;
                });
            #endregion

            #region Strecken Commands
            AddStreckeCmd = new RelayCommand(obj =>
            {
                var f = DialogHelpers.OpenLoksimFile((string)null, FileExtensions.StreckenFiles, null);
                if (!string.IsNullOrEmpty(f))
                {
                    var autoRepl = GetAutoReplacement(f);
                    set_.Strecken.Add(new ReplacementSet.Replacement { OriginalName = f, NewName = autoRepl });
                    streckenView_.Refresh();
                }
            },
                obj =>
                {
                    return true;
                });

            DeleteStreckeCmd = new RelayCommand(obj =>
            {
                if (selectedStrecke_ != null)
                {
                    set_.Strecken.Remove(selectedStrecke_);
                    streckenView_.Refresh();
                }
            },
                obj =>
                {
                    return selectedStrecke_ != null;
                });
            #endregion
        }

        private void AutoSearchForFiles()
        {
            foreach (var fpl in set_.Fahrplaene)
            {
                try
                {
                    XDocument xDoc = XDocument.Load(fpl.OriginalName);
                    var root = xDoc.Root;
                    var kbs = root.Elements("Props").Attributes("KBS").Select(a => a.Value).FirstOrDefault();
                    kbs = L3dFilePath.CreateRelativeToFile(kbs, new L3dFilePath(fpl.OriginalName)).AbsolutePath;
                    if (!string.IsNullOrWhiteSpace(kbs) && !ContainsOriginal(set_.Kursbuchstrecken, kbs))
                    {
                        set_.Kursbuchstrecken.Add(new ReplacementSet.Replacement { OriginalName = kbs, NewName = GetAutoReplacement(kbs) });
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
            }

            foreach (var kbs in set_.Kursbuchstrecken)
            {
                try
                {
                    XDocument xDoc = XDocument.Load(kbs.OriginalName);
                    var root = xDoc.Root;
                    var listStr = root.Elements("Strecken").Elements("Strecke").Elements("Props").Attributes("Name").Select(a => a.Value);
                    listStr = listStr.Select(s => L3dFilePath.CreateRelativeToFile(s, new L3dFilePath(kbs.OriginalName)).AbsolutePath);
                    foreach (var str in listStr)
                    {
                        if (!string.IsNullOrWhiteSpace(str) && !ContainsOriginal(set_.Strecken, str))
                        {
                            set_.Strecken.Add(new ReplacementSet.Replacement { OriginalName = str, NewName = GetAutoReplacement(str) });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
            }
        }

        private bool ContainsOriginal(IEnumerable<ReplacementSet.Replacement> entries, string originalName)
        {
            foreach (var e in entries)
            {
                if (string.Equals(e.OriginalName, originalName))
                {
                    return true;
                }
            }
            return false;
        }

        private string GetAutoReplacement(string str)
        {
            return ReplacementUtilities.GetAutoReplacement(set_.Ersetzungen, str);
        }

        public ReplacementSet.Replacement SelectedFahrplan
        {
            get { return selectedFahrplan_; }
            set
            {
                if (value != selectedFahrplan_)
                {
                    selectedFahrplan_ = value;
                    NotifyPropertyChanged(() => SelectedFahrplan);
                }
            }
        }
        public ReplacementSet.Replacement SelectedKursbuchstrecke
        {
            get { return selectedKursbuchstrecke_; }
            set
            {
                if (value != selectedKursbuchstrecke_)
                {
                    selectedKursbuchstrecke_ = value;
                    NotifyPropertyChanged(() => SelectedKursbuchstrecke);
                }
            }
        }
        public ReplacementSet.Replacement SelectedStrecke
        {
            get { return selectedStrecke_; }
            set
            {
                if (value != selectedStrecke_)
                {
                    selectedStrecke_ = value;
                    NotifyPropertyChanged(() => SelectedStrecke);
                }
            }
        }

        public ICollectionView Fahrplaene
        {
            get { return fahrplaeneView_; }
        }

        public ICollectionView Kursbuchstrecken
        {
            get { return kursbuchstreckenView_; }
        }

        public ICollectionView Strecken
        {
            get { return streckenView_; }
        }

        public RelayCommand AddFplCmd { get; private set; }
        public RelayCommand DeleteFplCmd { get; private set; }

        public RelayCommand AddKursbuchstreckeCmd { get; private set; }
        public RelayCommand DeleteKursbuchstreckeCmd { get; private set; }

        public RelayCommand AddStreckeCmd { get; private set; }
        public RelayCommand DeleteStreckeCmd { get; private set; }

        public RelayCommand SearchForKbsAndStrCmd { get; private set; }
    }
}
