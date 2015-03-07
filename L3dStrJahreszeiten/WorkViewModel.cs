using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace L3dStrJahreszeiten
{
    public class WorkViewModel : BaseModel
    {
        public enum TaskType
        {
            TaskFpl, TaskKbs, TaskStr
        }

        public class TaskItem
        {
            public string OriginalFile { get; set; }
            public string NewFile { get; set; }
            public bool ProcessTask { get; set; }
            public TaskType Type { get; set; }

            public override string ToString()
            {
                return System.IO.Path.GetFileName(OriginalFile) + " => " +
                    System.IO.Path.GetFileName(NewFile);
            }

            public string Description
            {
                get
                {
                    return string.Format("{0}: {1} => {2}",
                        Type == TaskType.TaskFpl ? "Fahrplan" : (Type == TaskType.TaskKbs) ? "Kursbuchstrecke" : "Strecke",
                        System.IO.Path.GetFileName(OriginalFile),
                        System.IO.Path.GetFileName(NewFile));
                }
            }
        }

        private ReplacementSet set_;
        private IEnumerable<ReplacementEntry> replacements_;


        public WorkViewModel(ReplacementSet set, IEnumerable<ReplacementEntry> replacements)
        {
            set_ = set;
            replacements_ = replacements;
            Tasks = new ObservableCollection<TaskItem>();
            Errors = new ObservableCollection<string>();

            AddTasks(set_.Fahrplaene, TaskType.TaskFpl);
            AddTasks(set_.Kursbuchstrecken, TaskType.TaskKbs);
            AddTasks(set_.Strecken, TaskType.TaskStr);

            DoWorkCmd = new RelayCommand(obj =>
            {
                CreateFplAndKbsFiles();
                CreateStrFiles();
            },
            obj =>
            {
                return Tasks.Any(en => en.ProcessTask);
            });
        }

        private void AddTasks(IEnumerable<ReplacementSet.Replacement> replacements, TaskType type)
        {
            foreach (var r in replacements)
            {
                if (r.NewName != r.OriginalName && !string.IsNullOrWhiteSpace(r.NewName))
                {
                    TaskItem ni = new TaskItem { Type = type, OriginalFile = r.OriginalName, NewFile = r.NewName };
                    ni.ProcessTask = type == TaskType.TaskStr || !System.IO.File.Exists(r.NewName);
                    Tasks.Add(ni);
                }
            }
        }

        private void CreateStrFiles()
        {
            var reps  = replacements_.Where(en =>
                            en.Replacement != null && en.IsVisible && !string.IsNullOrWhiteSpace(en.Replacement.AbsolutePath)).
                            ToDictionary(en => en.Original, en => en.Replacement);
            foreach (var t in Tasks.Where(en => en.ProcessTask && en.Type == TaskType.TaskStr))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(t.NewFile));
                    L3dFileIO.ReplaceFilesInStr(t.OriginalFile, t.NewFile, reps);
                }
                catch (Exception ex)
                {
                    Errors.Add("Fehler beim Bearbeiten des Tasks " + t + "\n" + ex.Message);
                }
            }
        }

        private void CreateFplAndKbsFiles()
        {
            foreach (var t in Tasks)
            {
                try
                {
                    if (t.ProcessTask)
                    {
                        if (t.Type == TaskType.TaskFpl)
                        {
                            XDocument xDoc = XDocument.Load(t.OriginalFile);
                            var root = xDoc.Root;
                            var kbs = root.Elements("Props").Attributes("KBS").Select(a => a.Value).FirstOrDefault();
                            kbs = L3dFilePath.CreateRelativeToFile(kbs, new L3dFilePath(t.OriginalFile)).AbsolutePath;
                            var k = set_.Kursbuchstrecken.Where(en => en.OriginalName == kbs).FirstOrDefault();
                            if (k != null)
                            {
                                var newKbsFile = new L3dFilePath(k.NewName);
                                root.Elements("Props").Attributes("KBS").FirstOrDefault().SetValue(newKbsFile.GetPathRelativeToFile(new L3dFilePath(t.NewFile)));
                            }
                            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(t.NewFile));
                            xDoc.Save(t.NewFile);
                        }
                        else if (t.Type == TaskType.TaskKbs)
                        {
                            XDocument xDoc = XDocument.Load(t.OriginalFile);
                            var root = xDoc.Root;
                            var listStr = root.Elements("Strecken").Elements("Strecke").Elements("Props").Attributes("Name");
                            foreach (var attr in listStr)
                            {
                                var oldFileVal = L3dFilePath.CreateRelativeToFile(attr.Value, new L3dFilePath(t.OriginalFile));
                                var rep = set_.Strecken.Where(en => en.OriginalName == oldFileVal.AbsolutePath).FirstOrDefault();
                                if (rep != null)
                                {
                                    var newStrFile = new L3dFilePath(rep.NewName);
                                    attr.SetValue(newStrFile.GetPathRelativeToFile(new L3dFilePath(t.NewFile)));
                                }
                            }

                            var listVerb = root.Elements("Verbindung").Elements("Name").Elements("Props").Attributes("Strecke1").Union(
                                root.Elements("Verbindung").Elements("Name").Elements("Props").Attributes("Strecke2"));
                            foreach (var attr in listVerb)
                            {
                                var oldFileVal = L3dFilePath.CreateRelativeToFile(attr.Value, new L3dFilePath(t.OriginalFile));
                                var rep = set_.Strecken.Where(en => en.OriginalName == oldFileVal.AbsolutePath).FirstOrDefault();
                                if (rep != null)
                                {
                                    var newStrFile = new L3dFilePath(rep.NewName);
                                    attr.SetValue(newStrFile.GetPathRelativeToFile(new L3dFilePath(t.NewFile)));
                                }
                            }
                            
                            
                            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(t.NewFile));
                            xDoc.Save(t.NewFile);
                        }
                    }
                } catch (Exception ex)
                {
                    Errors.Add("Fehler beim Bearbeiten des Tasks " + t + "\n" + ex.Message);
                }
            }
        }

        public ReplacementSet Set
        {
            get
            {
                return set_;
            }
        }
        public ObservableCollection<TaskItem> Tasks { get; private set; }
        public ObservableCollection<string> Errors { get; private set; }

        public RelayCommand DoWorkCmd { get; private set; }
    }
}
