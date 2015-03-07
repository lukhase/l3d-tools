using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace L3dStrJahreszeiten
{
    public class ReplacementSetsViewModel : BaseModel
    {
        private ICollectionView setsView_;
        private ICollectionView ersetzungenView_;
        private ReplacementSet selectedSet_;

        private Ersetzungsprojekt proj_;

        public delegate void ChangeSetEventDelegate(ReplacementSet set);
        public event ChangeSetEventDelegate ChangeSetEvent;

        public ReplacementSetsViewModel(Ersetzungsprojekt proj)
        {
            this.proj_ = proj;
            setsView_ = CollectionViewSource.GetDefaultView(proj.Sets);

            AddSetCmd = new RelayCommand(obj =>
                {
                    proj.Sets.Add(new ReplacementSet { Name = "_name_" });
                    setsView_.Refresh();
                },
                obj =>
                {
                    return true;
                });

            DeleteSetCmd = new RelayCommand(obj =>
                {
                    proj.Sets.Remove(SelectedSet);
                    setsView_.Refresh();            
                },
                obj =>
                {
                    return SelectedSet != null;
                });

            AddErsetzungCmd = new RelayCommand(obj =>
                {
                    if (SelectedSet != null)
                    {
                        SelectedSet.Ersetzungen.Add(new ReplacementSet.Replacement { OriginalName = "alt", NewName = "neu" });
                        ersetzungenView_.Refresh();
                    }
                },
                obj =>
                {
                    return SelectedSet != null;
                });

            DeleteErsetzungCmd = new RelayCommand(obj =>
                {
                    if (SelectedSet != null && SelectedErsetzung != null)
                    {
                        SelectedSet.Ersetzungen.Remove(SelectedErsetzung);
                        ersetzungenView_.Refresh();
                    }
                },
                obj =>
                {
                    return SelectedErsetzung != null;
                });
        }

        public ICollectionView ReplacementSets
        {
            get { return setsView_; }
        }

        public ICollectionView Ersetzungen
        {
            get { return ersetzungenView_; }
            private set
            {
                if (value != ersetzungenView_)
                {
                    ersetzungenView_ = value;
                    NotifyPropertyChanged(() => Ersetzungen);
                }
            }
        }

        public ReplacementSet SelectedSet 
        {
            get { return selectedSet_; }

            set
            {
                if (value != selectedSet_)
                {
                    selectedSet_ = value;
                    if (selectedSet_ == null)
                    {
                        Ersetzungen = null;
                    }
                    else
                    {
                        Ersetzungen = CollectionViewSource.GetDefaultView(selectedSet_.Ersetzungen);
                    }
                    if (ChangeSetEvent != null)
                    {
                        ChangeSetEvent(selectedSet_);
                    }
                }
            }
        
        }

        public ReplacementSet.Replacement SelectedErsetzung { get; set; }

        public RelayCommand AddSetCmd { get; private set; }

        public RelayCommand DeleteSetCmd { get; private set; }

        public RelayCommand AddErsetzungCmd { get; private set; }

        public RelayCommand DeleteErsetzungCmd { get; private set; }
    }
}
