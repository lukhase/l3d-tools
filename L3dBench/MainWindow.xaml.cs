using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using WindowsInput;

namespace L3dBench
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Process l3dProc;
        private LoksimControl.LoksimAutoServer l3dOle;
        private System.Windows.Threading.DispatcherTimer dispatcherTimer;
        private SimState simState;
        private DateTime? abfahrtsZeit;
        private DateTime lastEventTime;
        private double metersToDrive = 1000;
        private BenchmarkList benchmarks;
        private int curBenchIndex;
        private FileSystemWatcher fsw;
        private string frapsResults;
        private double lastReportPos;
        private long memUsage;
        private DateTime startLoading;
        private TimeSpan loadingTime;
        private int targetSpeed;

        enum SimState
        {
            Started, BuegelSent, BuegelAuf, HsSent, HsAn, AfbAn, Abfahren, BremsenGeloest, TuerenSchliessen, Unterwegs, Beenden, NaheZiel
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            curBenchIndex = 0;

            if (Process.GetProcessesByName("Fraps").Length == 0)
            {
                if (MessageBox.Show("Fraps wurde nicht gefunden!\nLoksim trotzdem starten?", "L3dBench", MessageBoxButton.YesNo) !=
                    MessageBoxResult.Yes)
                {
                    // Cancel Start of Loksim
                    return;
                }
            }
            StartL3d(benchmarks.Benchmarks[curBenchIndex]);
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Vor dem Starten der Benchmarks ist folgendes zu beachten:\nFraps muss laufen und F3 muss den Fraps Benchmark starten\n" +
                "Die Einstellungen für Loksim3D_AutoBenchmark.exe müssen gesetzt sein (sämtliche Sicherheitsfahrschalten ausschalten)\n" +
                "Sollen diese Einstellungen jetzt gesetzt werden?", "Erster Start", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var bs = benchmarks.Benchmarks[0];
                string fexe = Path.Combine(bs.L3dDir, "Loksim3D_AutoBenchmark.exe");
                File.Copy(bs.L3dExeToTest, fexe, true);
                Process.Start(fexe);
            }
        }

        private void StartL3d(BenchmarkSettings set)
        {
            ProcessStartInfo pinf = new ProcessStartInfo();

            AppendStatus("Loksim: " + set.L3dExeToTest);
            AppendStatus("Benchmarkname: " + set.Name);

            try
            {
                string fexe = Path.Combine(set.L3dDir, "Loksim3D_AutoBenchmark.exe");
                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        File.Copy(set.L3dExeToTest, fexe, true);
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (i >= 9)
                        {
                            throw ex;
                        }
                        Thread.Sleep(500);
                    }
                }
                pinf.FileName = fexe;
                pinf.Arguments = string.Format("Fahrplan:{0}/Lok:{1}/StartBahnhof:{2}/StartIndex:{3}/WetterIndex:{1}/hidemsg", set.Fpl, set.Lok, set.StartBhf, set.StartIndex, set.WetterIndex);
                metersToDrive = set.MeterZuFahren;
                targetSpeed = set.TargetSpeed;

                if (fsw != null)
                {
                    fsw.EnableRaisingEvents = false;
                    fsw.Changed -= fsw_Changed;
                    fsw.Dispose();
                }
                frapsResults = null;
                fsw = new FileSystemWatcher(set.FrapsBenchmarkDir);
                fsw.Changed += fsw_Changed;
                fsw.EnableRaisingEvents = true;

                AppendStatus("Starte Loksim für neuen Benchmark");
                l3dProc = Process.Start(pinf);
                simState = SimState.Started;
                dispatcherTimer.Start();

                btnHelp.IsEnabled = false;
                btnStart.IsEnabled = false;
                startLoading = DateTime.Now;
            }
            catch (Exception ex)
            {
                AppendStatus(ex.ToString());
                AppendStatus("Fehler beim Starten des Benchmarks:");
            }
        }

        private delegate void DelgateAppendStatus(string status);

        void fsw_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.Name.Contains("minmaxavg"))
            {
                frapsResults = e.FullPath;
                DelgateAppendStatus delStatus = new DelgateAppendStatus(AppendStatus);
                tbStatus.Dispatcher.Invoke(delStatus, "Fraps Ergebnisdatei: " + e.Name);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            l3dOle = new LoksimControl.LoksimAutoServer();
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 5);

            if (App.Args.Length > 0)
            {
                try
                {
                    benchmarks = BenchmarkSettingsSerializer.ReadFromFile(App.Args[0]);
                }
                catch (Exception ex)
                {
                    AppendStatus("Fehler beim Lesen der Benchmarks: " + ex.ToString());
                }
            }
#if DEBUG
            BenchmarkList bl = new BenchmarkList();
            BenchmarkSettings bs = new BenchmarkSettings {
                Fpl = @"D:\LoksimDevelopment\Loksim-Data\Fahrplan\PSB100\ICE_Stein.l3dfpl",
                FrapsBenchmarkDir = @"D:\Fraps\Benchmarks",
                L3dDir = @"D:\LoksimDevelopment\Loksim3d\",
                L3dExeToTest = @"D:\LoksimDevelopment\Loksim3d\Loksim3D.exe",
                Lok = @"D:\LoksimDevelopment\Loksim-Data\Lok\Triebwagen\BR 406\UPeters\406_080.l3dlok",
                MeterZuFahren = 500,
                Name = "ICE_Stein 60.000m",
                StartBhf = "Trischen Hbf",
                StartIndex = 0,
                WetterIndex = 1
            };
            bl.Benchmarks = new List<BenchmarkSettings>();
            bl.Benchmarks.Add(bs);
            bs = new BenchmarkSettings
            {
                Fpl = @"D:\LoksimDevelopment\Loksim-Data\Fahrplan\Dev\SimpleFpl.l3dfpl",
                FrapsBenchmarkDir = @"D:\Fraps\Benchmarks",
                L3dDir = @"D:\LoksimDevelopment\Loksim3d\",
                L3dExeToTest = @"D:\LoksimDevelopment\Loksim3d\Loksim3D.exe",
                Lok = @"D:\LoksimDevelopment\Loksim-Data\Lok\Triebwagen\BR 406\UPeters\406_080.l3dlok",
                MeterZuFahren = 300,
                Name = "NB SimpleFpl",
                StartBhf = "h0 \"ui\"",
                StartIndex = 1,
                WetterIndex = 1
            };
            bl.Benchmarks.Add(bs);
            bs = new BenchmarkSettings
            {
                Fpl = @"D:\LoksimDevelopment\Loksim-Data\Fahrplan\Dev\SimpleFpl2.l3dfpl",
                FrapsBenchmarkDir = @"D:\Fraps\Benchmarks",
                L3dDir = @"D:\LoksimDevelopment\Loksim3d\",
                L3dExeToTest = @"D:\LoksimDevelopment\Loksim3d\Loksim3D.exe",
                Lok = @"D:\LoksimDevelopment\Loksim-Data\Lok\Triebwagen\BR 406\UPeters\406_080.l3dlok",
                MeterZuFahren = 300,
                Name = "NB SimpleFpl2",
                StartBhf = "gl2_halt",
                StartIndex = 0,
                WetterIndex = 1
            };
            bl.Benchmarks.Add(bs);
            BenchmarkSettingsSerializer.WriteToFile(bl, "testout.xml");
            benchmarks = bl;
#endif
            if (benchmarks == null || benchmarks.Benchmarks == null || benchmarks.Benchmarks.Count == 0)
            {
                AppendStatus("Keine Benchmarks geladen");
                btnHelp.IsEnabled = false;
                btnStart.IsEnabled = false;
            }
            else
            {
                AppendStatus(benchmarks.Benchmarks.Count + " Benchmarks geladen");
            }
        }

        private void AnalyzeResults()
        {
            AppendStatus("Ergebnis auswerten");
            BenchmarkSettings bs = benchmarks.Benchmarks[curBenchIndex];
            if (string.IsNullOrEmpty(frapsResults))
            {
                AppendStatus("Keine Fraps Ergebnisdatei gefunden");
            }
            else
            {
                try
                {
                    using (StreamReader sr = new StreamReader(frapsResults))
                    {
                        sr.ReadLine();
                        var res = sr.ReadLine().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (!File.Exists("benchmark_results.csv"))
                        {
                            AppendStatus("Neue Ergebnisdatei erstellen");
                            using (StreamWriter sw = new StreamWriter("benchmark_results.csv"))
                            {
                                sw.WriteLine("Test Name; Loksim Version; Datum; Frames; Time (ms); Min; Max; Avg; Maximaler Speicherverbrauch;Startzeit");
                            }
                        }
                        using (StreamWriter sw = new StreamWriter("benchmark_results.csv", true))
                        {
                            //Frames, Time (ms), Min, Max, Avg
                            var versInfo = FileVersionInfo.GetVersionInfo(bs.L3dExeToTest);
                            sw.WriteLine("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9}", 
                                bs.Name, versInfo.ProductVersion, DateTime.Now.ToString(), res[0], res[1], res[2], res[3], res[4], memUsage, loadingTime);
                        }
                        AppendStatus("Avg Frames: " + res[4]);
                    }
                }
                catch (Exception ex)
                {
                    AppendStatus(ex.ToString());
                    AppendStatus("Fehler beim Auswerten der Ergebnisse:");
                }
            }
        }

        private void AppendStatus(string txt)
        {
            tbStatus.Text = txt + "\n" + tbStatus.Text;
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (l3dProc != null && l3dProc.HasExited)
            {
                l3dProc = null;
                dispatcherTimer.Stop();
                AnalyzeResults();
                ++curBenchIndex;

                btnHelp.IsEnabled = true;
                btnStart.IsEnabled = true;
                if (curBenchIndex < benchmarks.Benchmarks.Count)
                {
                    StartL3d(benchmarks.Benchmarks[curBenchIndex]);
                }
                else
                {
                    AppendStatus("Ergebnisse in benchmark_results.csv gespeichert");
                    AppendStatus("Alle Benchmarks abgeschlossen");
                }
            }
            else if (l3dOle.SimulationIsRunning())
            {
                l3dOle.StartWaitForCommands(500);
                string para = string.Empty;
                string ret;
                switch (simState)
                {
                    case SimState.Started:
                        loadingTime = DateTime.Now - startLoading;

                        l3dOle.SendKey(LoksimUtil.LoksimHelper.KEY_RICHTUNGSWALZE_V);
                        l3dOle.SendKey(LoksimUtil.LoksimHelper.KEY_BUEGEL);
                        simState = SimState.BuegelSent;
                        lastEventTime = DateTime.Now;
                        AppendStatus("Simulation gestartet");
                        break;
                    case SimState.BuegelSent:
                        LoksimUtil.LoksimHelper.AddParam(ref para, LoksimUtil.LoksimHelper.cLOK_ANZEIGE_BUEGELPANTO);
                        ret = l3dOle.GetSimData(para);
                        if (LoksimUtil.LoksimHelper.GetParamBool(ret, LoksimUtil.LoksimHelper.cLOK_ANZEIGE_BUEGELPANTO))
                        {
                            simState = SimState.BuegelAuf;
                        }
                        lastEventTime = DateTime.Now;
                        AppendStatus("Bügel heben");
                        break;
                    case SimState.BuegelAuf:
                        if ((DateTime.Now - lastEventTime).Seconds > 8)
                        {
                            l3dOle.SendKey(LoksimUtil.LoksimHelper.KEY_HAUPTSCHALTER);
                            simState = SimState.HsSent;
                            lastEventTime = DateTime.Now;
                            AppendStatus("HS ein");
                        }
                        break;
                    case SimState.HsSent:
                        if ((DateTime.Now - lastEventTime).Seconds >= 4)
                        {
                            LoksimUtil.LoksimHelper.AddParam(ref para, LoksimUtil.LoksimHelper.cLOK_ANZEIGE_HAUPTSCHALTER);
                            ret = l3dOle.GetSimData(para);
                            if (LoksimUtil.LoksimHelper.GetParamBool(ret, LoksimUtil.LoksimHelper.cLOK_ANZEIGE_HAUPTSCHALTER))
                            {
                                simState = SimState.HsAn;
                            }
                            lastEventTime = DateTime.Now;
                        }
                        break;
                    case SimState.HsAn:
                        for (int i = 0; i < 10; i++)
                        {
                            l3dOle.SendKey(LoksimUtil.LoksimHelper.KEY_BREMSEN7);
                        }
                        if ((DateTime.Now - lastEventTime).Seconds >= 4)
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                l3dOle.SendKey(LoksimUtil.LoksimHelper.KEY_BREMSEN7);
                            }
                            l3dOle.SendKey(LoksimUtil.LoksimHelper.KEY_AFB);  // AFB ein
                            l3dOle.SendKey(LoksimUtil.LoksimHelper.KEY_LZB); // LZB aus
                            simState = SimState.AfbAn;
                            AppendStatus("AFB ein, Bremsen lösen");

                            LoksimUtil.LoksimHelper.AddParam(ref para, LoksimUtil.LoksimHelper.cSIM_COMMON_FAHRPLAN);
                            ret = l3dOle.GetSimData(para);
                            ret = LoksimUtil.LoksimHelper.GetParamString(ret, LoksimUtil.LoksimHelper.cSIM_COMMON_FAHRPLAN);
                            abfahrtsZeit = GetAbfahrtZeitFromFpl(ret);
                            lastEventTime = DateTime.Now;
                        }
                        break;
                    case SimState.AfbAn:
                        for (int i = 0; i < 10; i++)
                        {
                            l3dOle.SendKey(LoksimUtil.LoksimHelper.KEY_BREMSEN7);
                        }

                        for (int i = 0; i < 20; i++)
                        {
                            l3dOle.SendKey(LoksimUtil.LoksimHelper.KEY_STUP);                        
                        }
                        AppendStatus("Zugkraft wählen");
                        simState = SimState.BremsenGeloest;
                        lastEventTime = DateTime.Now;
                        break;
                    case SimState.BremsenGeloest:
                        l3dOle.SendKey(LoksimUtil.LoksimHelper.KEY_BREMSEN7);
                        InputSimulator.SimulateKeyPress(VirtualKeyCode.F11);
                        if (abfahrtsZeit.HasValue)
                        {
                            LoksimUtil.LoksimHelper.AddParam(ref para, LoksimUtil.LoksimHelper.cSIM_COMMON_ACTTIME);
                            ret = l3dOle.GetSimData(para);
                            ret = LoksimUtil.LoksimHelper.GetParamString(ret, LoksimUtil.LoksimHelper.cSIM_COMMON_ACTTIME);
                            var actTime = ParseTime(ret);
                            if (actTime >= abfahrtsZeit)
                            {
                                simState = SimState.TuerenSchliessen;
                                l3dOle.SendKey(LoksimUtil.LoksimHelper.KEY_TUEREN);
                            }
                        }
                        else
                        {
                            simState = SimState.TuerenSchliessen;
                            l3dOle.SendKey(LoksimUtil.LoksimHelper.KEY_TUEREN);
                        }
                        lastEventTime = DateTime.Now;
                        break;
                    case SimState.TuerenSchliessen:
                        AppendStatus("Türen schließen");
                        l3dOle.SendKey(LoksimUtil.LoksimHelper.KEY_BREMSEN7);
                        simState = SimState.Abfahren;
                        lastEventTime = DateTime.Now;
                        break;
                    case SimState.Abfahren:
                        l3dOle.SendKey(LoksimUtil.LoksimHelper.KEY_BREMSEN7);
                        if ((DateTime.Now - lastEventTime).Seconds > 8)
                        {
                            AppendStatus("Abfahrt!");
                            double vsoll = 0;
                            while (vsoll < targetSpeed - 2)
                            {
                                l3dOle.SendKey(LoksimUtil.LoksimHelper.KEY_STRIGHT);
                                LoksimUtil.LoksimHelper.AddParam(ref para, LoksimUtil.LoksimHelper.cLOK_INSTRUMENT_VSOLLSTELLER);
                                ret = l3dOle.GetSimData(para);
                                vsoll = LoksimUtil.LoksimHelper.GetParamFloat(ret, LoksimUtil.LoksimHelper.cLOK_INSTRUMENT_VSOLLSTELLER);
                            }
                            AppendStatus("Fraps Benchmark starten");
                            InputSimulator.SimulateKeyPress(VirtualKeyCode.F3);
                            simState = SimState.Unterwegs;
                            lastEventTime = DateTime.Now;
                            lastReportPos = 0;
                        }
                        break;
                    case SimState.Unterwegs:
                    case SimState.NaheZiel:
                        {
                            double actPos;
                            LoksimUtil.LoksimHelper.AddParam(ref para, LoksimUtil.LoksimHelper.cSIM_COMMON_ACTPOS);
                            ret = l3dOle.GetSimData(para);
                            actPos = LoksimUtil.LoksimHelper.GetParamFloat(ret, LoksimUtil.LoksimHelper.cSIM_COMMON_ACTPOS);
                            if (actPos - lastReportPos > 1000 || lastReportPos < 0.5)
                            {
                                lastReportPos = actPos;
                                AppendStatus("Noch " + (metersToDrive - actPos) + " Meter zu fahren");
                            }
                            if (actPos > metersToDrive)
                            {
                                AppendStatus("Benchmark beenden");
                                InputSimulator.SimulateKeyPress(VirtualKeyCode.F3);
                                simState = SimState.Beenden;
                            }
                            else if (actPos + 500 > metersToDrive && simState != SimState.NaheZiel)
                            {
                                dispatcherTimer.Interval = TimeSpan.FromSeconds(0.5);
                                simState = SimState.NaheZiel;
                            }
                        }
                        break;
                    case SimState.Beenden:
                        AppendStatus("Speicherverbrauch ermitteln");
                        memUsage = l3dProc.PeakWorkingSet64 / 1024 / 1024;
                        AppendStatus("Loksim schließen");
                        l3dProc.CloseMainWindow();
                        break;
                }
                l3dOle.StopWaitForCommands();
            }
        }

        private DateTime? ParseTime(string t)
        {
            try
            {
                var times = t.Split(':').ToList();
                if (times.Count == 2)
                {
                    return new DateTime(2013, 1, 1, int.Parse(times[0]), int.Parse(times[1]), 0);
                }
                if (times.Count > 2)
                {
                    return new DateTime(2013, 1, 1, int.Parse(times[0]), int.Parse(times[1]), int.Parse(times[2]));
                }
            }
            catch (Exception)
            {
            }

            return null;
        }

        private DateTime? GetAbfahrtZeitFromFpl(string ret)
        {
            var line1 = ret.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault().Trim();
            if (!string.IsNullOrEmpty(line1))
            {
                for (int i = line1.Length - 1; i >= 0; i--)
                {
                    if (char.IsWhiteSpace(line1[i]))
                    {
                        line1 = line1.Substring(i + 1);
                        break;
                    }
                }
                return ParseTime(line1);
            }
            return null;
        }
    }
}
