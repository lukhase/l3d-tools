using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.Win32;
using System.Windows;
using System.IO;

namespace L3dStrJahreszeiten.Views
{
    /// <summary>
    /// Helper für FileOpen und FileSave
    /// </summary>
    public static class DialogHelpers
    {

        /// <summary>
        /// Zeigt Datei-Öffnen Dialog an
        /// </summary>
        /// <param name="selectedFile">Zurzeit gewählte Datei (kann null sein)</param>
        /// <param name="fileExtensions">Dateiendungen</param>
        /// <param name="owner">ParentWindow (kann null sein)</param>
        /// <param name="guid">GUID für FileDialog (Vista+ relevant; OS merkt sich pro GUID wo der Dialog das letzte Mal geöffnet war)</param>
        /// <returns>Gewählte Datei oder null</returns>
        public static string OpenLoksimFile(L3dFilePath selectedFile, IEnumerable<CommonFileDialogFilter> fileExtensions, Window owner, Guid guid = default(Guid))
        {
            return OpenLoksimFile(selectedFile != null ? selectedFile.AbsolutePath : null, fileExtensions, owner, guid);
        }
        /// <summary>
        /// Zeigt Datei-Öffnen Dialog an
        /// </summary>
        /// <param name="selectedFile">Zurzeit gewählte Datei (kann null sein)</param>
        /// <param name="fileExtensions">Dateiendungen</param>
        /// <param name="owner">ParentWindow (kann null sein)</param>
        /// <param name="guid">GUID für FileDialog (Vista+ relevant; OS merkt sich pro GUID wo der Dialog das letzte Mal geöffnet war)</param>
        /// <returns>Gewählte Datei oder null</returns>
        public static string OpenLoksimFile(string selectedFile, IEnumerable<CommonFileDialogFilter> fileExtensions, Window owner, Guid guid = default(Guid))
        {
            if (CommonOpenFileDialog.IsPlatformSupported)
            {
                using (CommonOpenFileDialog dlg = new CommonOpenFileDialog("Loksim3D"))
                {
                    if (!string.IsNullOrEmpty(selectedFile))
                    {
                        dlg.InitialDirectory = Path.GetDirectoryName(selectedFile);
                        dlg.DefaultFileName = Path.GetFileName(selectedFile);
                    }
                     
                    if (Directory.Exists(L3dFilePath.LoksimDirectory.AbsolutePath))
                    {
                        dlg.DefaultDirectory = L3dFilePath.LoksimDirectory.AbsolutePath;
                        dlg.AddPlace(L3dFilePath.LoksimDirectory.AbsolutePath, Microsoft.WindowsAPICodePack.Shell.FileDialogAddPlaceLocation.Bottom);
                        dlg.ShowPlacesList = true;
                    }
                    dlg.CookieIdentifier = guid;
                    if (fileExtensions != null)
                    {
                        foreach (CommonFileDialogFilter f in fileExtensions)
                        {
                            dlg.Filters.Add(f);
                        }
                    }

                    if (owner != null)
                    {
                        if (dlg.ShowDialog(owner) == CommonFileDialogResult.Ok)
                        {
                            return dlg.FileName;
                        }
                    }
                    else
                    {
                        if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
                        {
                            return dlg.FileName;
                        }
                    }
                }
            }
            else
            {
                OpenFileDialog dlg = new OpenFileDialog();
                if (!string.IsNullOrEmpty(selectedFile))
                {
                    dlg.FileName = selectedFile;
                }
                dlg.Title = "Loksim3D";
                if (!string.IsNullOrWhiteSpace(selectedFile))
                {
                    dlg.InitialDirectory = Path.GetDirectoryName(selectedFile);
                }
                if (fileExtensions != null)
                {
                    string f = FileExtensions.CommonDlgFilterToClassicFilter(fileExtensions);
                    dlg.Filter = FileExtensions.CommonDlgFilterToClassicFilter(fileExtensions);
                }
                if (dlg.ShowDialog().GetValueOrDefault(false))
                {
                    return dlg.FileName;
                }
            }
            return null;
        }

        /// <summary>
        /// Zeigt Datei-Speichern Dialog an
        /// </summary>
        /// <param name="selectedFile">Zurzeit gewählte Datei (kann null sein)</param>
        /// <param name="fileExtensions">Dateiendungen</param>
        /// <param name="guid">GUID für FileDialog (Vista+ relevant; OS merkt sich pro GUID wo der Dialog das letzte Mal geöffnet war)</param>
        /// <returns>Gewählte Datei oder null</returns>
        public static L3dFilePath SaveLoksimFile(L3dFilePath selectedFile, IEnumerable<CommonFileDialogFilter> fileExtensions, Guid guid = default(Guid))
        {
            if (CommonSaveFileDialog.IsPlatformSupported)
            {
                using (CommonSaveFileDialog dlg = new CommonSaveFileDialog("Loksim3D"))
                {
                    if (selectedFile != null)
                    {
                        dlg.InitialDirectory = selectedFile.Directory;
                        dlg.DefaultFileName = selectedFile.Filename;
                    }
                    if (Directory.Exists(L3dFilePath.LoksimDirectory.AbsolutePath))
                    {
                        dlg.AddPlace(L3dFilePath.LoksimDirectory.AbsolutePath, Microsoft.WindowsAPICodePack.Shell.FileDialogAddPlaceLocation.Bottom);
                        dlg.ShowPlacesList = true;
                    }
                    dlg.CookieIdentifier = guid;
                    if (fileExtensions != null)
                    {
                        foreach (CommonFileDialogFilter f in fileExtensions)
                        {
                            dlg.Filters.Add(f);
                        }
                        dlg.DefaultExtension = "." + dlg.Filters[0].Extensions[0];
                    }
                    if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        return new L3dFilePath(dlg.FileName);
                    }
                }
            }
            else
            {
                SaveFileDialog dlg = new SaveFileDialog();
                if (selectedFile != null)
                {
                    dlg.FileName = selectedFile.Filename;
                }
                dlg.Title = "Loksim3D";
                if (selectedFile != null)
                {
                    dlg.InitialDirectory = selectedFile.Directory;
                }
                if (fileExtensions != null)
                {
                    dlg.Filter = FileExtensions.CommonDlgFilterToClassicFilter(fileExtensions);
                    dlg.DefaultExt = "." + fileExtensions.First().Extensions[0];
                }
                if (dlg.ShowDialog().GetValueOrDefault(false))
                {
                    return new L3dFilePath(dlg.FileName);
                }
            }
            return null;
        }
    }


    /// <summary>
    /// Statische Properties für verschiedenste Dateiendungen (und Kombinationen)
    /// </summary>
    public static class FileExtensions
    {
        private static IEnumerable<CommonFileDialogFilter> _changeableFiles =
            new List<CommonFileDialogFilter> { new CommonFileDialogFilter("Loksim Dateien", "*.l3dobj;*.l3dgrp;*.bmp;*.png;*.tga") };

        public static IEnumerable<CommonFileDialogFilter> ChangeableFiles
        {
            get { return _changeableFiles; }
        }

        private static IEnumerable<CommonFileDialogFilter> _fahrplanFiles =
            new List<CommonFileDialogFilter> { new CommonFileDialogFilter("Loksim Fahrpläne", "*.l3dfpl") };

        public static IEnumerable<CommonFileDialogFilter> FahrplanFiles { get { return _fahrplanFiles; } }

        private static IEnumerable<CommonFileDialogFilter> _kursbuchstreckenFiles =
            new List<CommonFileDialogFilter> { new CommonFileDialogFilter("Loksim Kursbuchstrecken", "*.l3dkbs") };

        public static IEnumerable<CommonFileDialogFilter> KursbuchstreckenFiles { get { return _kursbuchstreckenFiles; } }

        private static IEnumerable<CommonFileDialogFilter> _streckenFiles =
            new List<CommonFileDialogFilter> { new CommonFileDialogFilter("Loksim Strecken", "*.l3dstr") };

        public static IEnumerable<CommonFileDialogFilter> StreckenFiles { get { return _streckenFiles; } }

        /// <summary>
        /// Erzeugt String aus CommonFileDialogFilter Collection
        /// </summary>
        /// <param name="l">CommonFileDialogFilter Collection</param>
        /// <returns>Klassischer Filter-String für alte Datei-Dialoge</returns>
        public static string CommonDlgFilterToClassicFilter(IEnumerable<CommonFileDialogFilter> l)
        {
            if (l != null)
            {
                return l.Aggregate("", (str, f) =>
                    str + (str.Length != 0 ? "|" : "") + f.DisplayName + "|" +
                    f.Extensions.Aggregate("", (strExt, ext) =>
                        strExt + (strExt.Length != 0 ? ";" : "") + "*." + ext));
            }
            return string.Empty;
        }
    }
}
