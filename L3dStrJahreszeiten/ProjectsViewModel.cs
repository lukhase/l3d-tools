using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace L3dStrJahreszeiten
{
    public class ProjectsViewModel : BaseModel
    {
        private ObservableCollection<Ersetzungsprojekt> _projekte;
        private Ersetzungsprojekt selected_;

        public delegate void LoadProjEventDelegate(Ersetzungsprojekt proj);
        public event LoadProjEventDelegate LoadProjEvent;

        public ProjectsViewModel()
        {
            _projekte = new ObservableCollection<Ersetzungsprojekt>();
            
            CreateNewProjectCmd = new RelayCommand(obj =>
                {
                    CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                    dialog.IsFolderPicker = true;
                    dialog.Title = "Bitte einen leeren Projektordner auswählen";
                    CommonFileDialogResult result = dialog.ShowDialog();
                    if (result == CommonFileDialogResult.Ok)
                    {
                        var fn = dialog.FileName;
                        var newProj = new Ersetzungsprojekt();
                        newProj.ProjectDirectory = fn;
                        newProj.LoksimDirectory = RegistryAccess.GetLoksimDataDir();
                        InsertNewProject(newProj);
                        LoadProjectCmd.Execute(null);
                    }
                },
                obj =>
                {
                    return true;
                });

            LoadProjectCmd = new RelayCommand(obj =>
            {
                if (SelectedProject != null && SelectedProject != null)
                {
                    LoadProjEvent(SelectedProject);
                }
            },
            obj =>
            {
                return SelectedProject != null;
            });

            LoadFromFileCmd = new RelayCommand(obj =>
                {
                    CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                    dialog.IsFolderPicker = false;
                    dialog.Title = "Bitte eine Projektdatei auswählen";
                    dialog.Filters.Add(new CommonFileDialogFilter("Projektdateien", "*.xml"));
                    CommonFileDialogResult result = dialog.ShowDialog();
                    if (result == CommonFileDialogResult.Ok)
                    {
                        var fn = dialog.FileName;
                        Ersetzungsprojekt newProj = null;
                        XmlSerializer serializer = new XmlSerializer(typeof(Ersetzungsprojekt));
                        using (StreamReader sr = new StreamReader(fn))
                        {
                            newProj = (Ersetzungsprojekt)serializer.Deserialize(sr);
                            InsertNewProject(newProj);
                            LoadProjectCmd.Execute(null);
                        }
                    }
                },
                obj =>
                {
                    return true;
                });
        }


        private void InsertNewProject(Ersetzungsprojekt proj)
        {
            _projekte.Insert(0, proj);
            for (int i = 1; i < Projekte.Count; i++)
            {
                if (Projekte[i].ProjectDirectory == proj.ProjectDirectory)
                {
                    Projekte.RemoveAt(i);
                    i--;
                }
            }
            while (Projekte.Count > 10)
            {
                Projekte.RemoveAt(10);
            }
            SelectedProject = proj;
        }

        public Ersetzungsprojekt SelectedProject
        {
            get { return selected_; }
            set
            {
                if (value != selected_)
                {
                    selected_ = value;
                    NotifyPropertyChanged(() => SelectedProject);
                }
            }
        }

        public IList<Ersetzungsprojekt> Projekte
        {
            get
            {
                return _projekte;

            }

            set
            {
                _projekte = new ObservableCollection<Ersetzungsprojekt>(value);
                NotifyPropertyChanged(() => Projekte);
            }
        }

        public RelayCommand CreateNewProjectCmd { get; private set; }

        public RelayCommand LoadProjectCmd { get; private set; }

        public RelayCommand LoadFromFileCmd { get; private set; }
    }
}
