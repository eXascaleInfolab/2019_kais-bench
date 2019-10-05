﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TestingFramework.Algorithms;
using TestingFramework.Testing;

namespace TestingFramework
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            string[] codes = null;
            string[] codesLimited = null;

            List<string> ignoreList = new List<string>();

            Dictionary<string, string> config =
                args.Length == 0
                    ? Utils.ReadConfigFile()
                    : Utils.ReadConfigFile(args[0]);

            bool disableTrmf = false;
            bool useBatchCd = false;

            foreach (KeyValuePair<string,string> entry in config)
            {
                switch (entry.Key)
                {
                    case "PlotsFolder":
                        DataWorks.FolderPlotsRemoteBase = entry.Value.Trim().Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.Personal));
                        DataWorks.FolderPlotsRemote = DataWorks.FolderPlotsRemoteBase;
                        break;
                        
                    case "AlgorithmsLocation":
                        AlgoPack.GlobalAlgorithmsLocation = entry.Value.Trim().Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.Personal));
                        break;
                    
                    case "Datasets":
                        codes = entry.Value.Split(',').Select(c => c.Trim()).ToArray();
                        codesLimited = codes
                            .Where(c => DataWorks.CountMatrixColumns($"{c}/{c}_normal.txt") > 4)
                            .ToArray();
                        break;
                    
                    case "EnableStreaming":
                        EnumMethods.EnableStreaming = Convert.ToBoolean(entry.Value);
                        break;
                    
                    case "EnableContinuous":
                        EnumMethods.EnableContinuous = Convert.ToBoolean(entry.Value);
                        break;
                    
                    case "EnableMulticolumn":
                        EnumMethods.EnableMulticolumn = Convert.ToBoolean(entry.Value);
                        break;
                    
                    case "DisableTrmf":
                        disableTrmf = Convert.ToBoolean(entry.Value);
                        break;
                        
                    case "UseBatchCd":
                        useBatchCd = Convert.ToBoolean(entry.Value);
                        break;

                    case "IgnoreAlgorithms":
                        ignoreList.AddRange(entry.Value.Split(',').Select(x => x.Trim().ToLower()));
                        break;
                    
                    default:
                        Utils.DelayedWarnings.Enqueue($"Warning: unknown config entry with the key {entry.Key}");
                        break;
                }
            }
            
            // verificaiton that all necessary entries are provided
            if (DataWorks.FolderPlotsRemoteBase == null)
            {
                throw new InvalidProgramException("Invalid config file: plots folder has to be supplied (PlotsFolder=)");
            }
            
            if (AlgoPack.GlobalAlgorithmsLocation == null)
            {
                throw new InvalidProgramException("Invalid config file: algorithms folder has to be supplied (AlgorithmsLocation=)");
            }
            
            if (codes == null || codes.Length == 0)
            {
                throw new InvalidProgramException("Invalid config file: datasets are not supplied (Datasets=) or the list is empty");
            }

            Algorithm cdVersion = useBatchCd ? AlgoPack.Cd : AlgoPack.InCd;

            ignoreList = useBatchCd
                ? ignoreList.Select(x => x == "cdrec" ? "cd" : x).ToList()
                : ignoreList.Select(x => x == "cd" ? "cdrec" : x).ToList();

            var listStd = new List<Algorithm> {cdVersion};

            listStd.AddRange(new[] {AlgoPack.Stmvl, AlgoPack.Tkcm, AlgoPack.Spirit, AlgoPack.Nnmf, AlgoPack.Grouse});
            listStd.AddRange(new[] {AlgoPack.ArImp, AlgoPack.Ssa, AlgoPack.Mrnn, AlgoPack.DynaMMo, AlgoPack.MdIsvd, AlgoPack.SvdImp});// very new batch

            if (!disableTrmf)
            {
                listStd.Add(AlgoPack.Trmf);
            }

            listStd = listStd.Where(x => !ignoreList.Contains(x.AlgCode.ToLower())).ToList();
            
            AlgoPack.ListAlgorithms = listStd.ToArray();

            listStd.Remove(AlgoPack.Tkcm);
            listStd.Remove(AlgoPack.Spirit);
            listStd.Remove(AlgoPack.Ssa);
            AlgoPack.ListAlgorithmsMulticolumn = listStd.ToArray();

            AlgoPack.ListAlgorithmsStreaming = AlgoPack.ListAlgorithmsStreaming.Where(x => !ignoreList.Contains(x.AlgCode.ToLower())).ToArray();
            
            AlgoPack.CleanUncollectedResults();
            AlgoPack.EnsureFolderStructure();
            
            void FullPrecision()
            {
                if (EnumMethods.EnableContinuous)
                {
                    codes.ForEach(c => TestRoutines.PrecisionTest(ExperimentType.Continuous, ExperimentScenario.Missing, c, 1000));
                    codes.ForEach(c => TestRoutines.PrecisionTest(ExperimentType.Continuous, ExperimentScenario.Length, c, 2000));
                    codesLimited.ForEach(c => TestRoutines.PrecisionTest(ExperimentType.Continuous, ExperimentScenario.Columns, c, 1000));
                    codesLimited.ForEach(c => TestRoutines.PrecisionTest(ExperimentType.Continuous, ExperimentScenario.Fullrow, c, 1000));
                }

                codes.ForEach(c => TestRoutines.PrecisionTest(ExperimentType.Recovery, ExperimentScenario.Missing, c, 1000));
                codes.ForEach(c => TestRoutines.PrecisionTest(ExperimentType.Recovery, ExperimentScenario.Length, c, 2000));
                codesLimited.ForEach(c => TestRoutines.PrecisionTest(ExperimentType.Recovery, ExperimentScenario.Columns, c, 1000));
                codesLimited.ForEach(c => TestRoutines.PrecisionTest(ExperimentType.Recovery, ExperimentScenario.Fullrow, c, 1000));
                codes.ForEach(c => TestRoutines.PrecisionTest(ExperimentType.Recovery, ExperimentScenario.BlockSlide, c, 1000));
                codes.ForEach(c => TestRoutines.PrecisionTest(ExperimentType.Recovery, ExperimentScenario.McarElement, c, 1000));
                codes.ForEach(c => TestRoutines.PrecisionTest(ExperimentType.Recovery, ExperimentScenario.McarBlock, c, 1000));
            }
            void FullRuntime()
            {
                if (EnumMethods.EnableContinuous)
                {
                    codes.ForEach(c => TestRoutines.RuntimeTest(ExperimentType.Continuous, ExperimentScenario.Missing, c, 1000));
                    codes.ForEach(c => TestRoutines.RuntimeTest(ExperimentType.Continuous, ExperimentScenario.Length, c, 2000));
                    codesLimited.ForEach(c => TestRoutines.RuntimeTest(ExperimentType.Continuous, ExperimentScenario.Columns, c, 1000));
                    codesLimited.ForEach(c => TestRoutines.RuntimeTest(ExperimentType.Continuous, ExperimentScenario.Fullrow, c, 1000));
                }

                codes.ForEach(c => TestRoutines.RuntimeTest(ExperimentType.Recovery, ExperimentScenario.Missing, c, 1000));
                codes.ForEach(c => TestRoutines.RuntimeTest(ExperimentType.Recovery, ExperimentScenario.Length, c, 2000));
                codesLimited.ForEach(c => TestRoutines.RuntimeTest(ExperimentType.Recovery, ExperimentScenario.Columns, c, 1000));
                codesLimited.ForEach(c => TestRoutines.RuntimeTest(ExperimentType.Recovery, ExperimentScenario.Fullrow, c, 1000));
                codes.ForEach(c => TestRoutines.RuntimeTest(ExperimentType.Recovery, ExperimentScenario.BlockSlide, c, 1000));
                codes.ForEach(c => TestRoutines.RuntimeTest(ExperimentType.Recovery, ExperimentScenario.McarElement, c, 1000));
                codes.ForEach(c => TestRoutines.RuntimeTest(ExperimentType.Recovery, ExperimentScenario.McarBlock, c, 1000));
            }
            void FullRuntimeReplot() //service method
            {
                if (EnumMethods.EnableContinuous)
                {
                    codes.ForEach(c => TestRoutines.RuntimeTestReplot(ExperimentType.Continuous, ExperimentScenario.Missing, c, 1000));
                    codes.ForEach(c => TestRoutines.RuntimeTestReplot(ExperimentType.Continuous, ExperimentScenario.Length, c, 2000));
                    codesLimited.ForEach(c => TestRoutines.RuntimeTestReplot(ExperimentType.Continuous, ExperimentScenario.Columns, c, 1000));
                    codesLimited.ForEach(c => TestRoutines.RuntimeTestReplot(ExperimentType.Continuous, ExperimentScenario.Fullrow, c, 1000));
                }

                codes.ForEach(c => TestRoutines.RuntimeTestReplot(ExperimentType.Recovery, ExperimentScenario.Missing, c, 1000));
                codes.ForEach(c => TestRoutines.RuntimeTestReplot(ExperimentType.Recovery, ExperimentScenario.Length, c, 2000));
                codesLimited.ForEach(c => TestRoutines.RuntimeTestReplot(ExperimentType.Recovery, ExperimentScenario.Columns, c, 1000));
                codesLimited.ForEach(c => TestRoutines.RuntimeTestReplot(ExperimentType.Recovery, ExperimentScenario.Fullrow, c, 1000));
                codes.ForEach(c => TestRoutines.RuntimeTestReplot(ExperimentType.Recovery, ExperimentScenario.BlockSlide, c, 1000));
            }
            void FullStreaming()
            {
                codes.ForEach(c => TestRoutines.RuntimeTest(ExperimentType.Streaming, ExperimentScenario.Missing, c, 1000));
                codes.ForEach(c => TestRoutines.RuntimeTest(ExperimentType.Streaming, ExperimentScenario.Length, c, 2000));
                codesLimited.ForEach(c => TestRoutines.RuntimeTest(ExperimentType.Streaming, ExperimentScenario.Columns, c, 1000));
            }
            void FullMulticolumn()
            {
                codes.ForEach(c => TestRoutines.PrecisionTest(ExperimentType.Recovery, ExperimentScenario.MultiColumnDisjoint, c, 1000));
                codes.ForEach(c => TestRoutines.PrecisionTest(ExperimentType.Recovery, ExperimentScenario.MultiColumnOverlap, c, 1000));
                
                codes.ForEach(c => TestRoutines.RuntimeTest(ExperimentType.Recovery, ExperimentScenario.MultiColumnDisjoint, c, 1000));
                codes.ForEach(c => TestRoutines.RuntimeTest(ExperimentType.Recovery, ExperimentScenario.MultiColumnOverlap, c, 1000));
            }
            
            if (EnumMethods.EnableStreaming) FullStreaming();
            
            FullPrecision();
            
            FullRuntime();
            
            if (EnumMethods.EnableMulticolumn) FullMulticolumn();
            
            //codes.ForEach(c => TestRoutines.RuntimeTest(ExperimentType.Recovery, ExperimentScenario.MissingMultiColumn, c, 1000));
            //codes.ForEach(c => TestRoutines.RuntimeTest(ExperimentType.Continuous, ExperimentScenario.MissingMultiColumn, c, 2000));
            
            //codes.ForEach(c => TestRoutines.PrecisionTest(ExperimentType.Recovery, ExperimentScenario.MissingMultiColumn, c, 1000));
            //codes.ForEach(c => TestRoutines.PrecisionTest(ExperimentType.Continuous, ExperimentScenario.MissingMultiColumn, c, 1000));
            
            //
            // multi-run for 1...N runtime tests and averaging the results from them
            //

            #if false
            {
                for (int i = 1; i <= 5; i++)
                {
                    DataWorks.FolderPlotsRemote = DataWorks.FolderPlotsRemoteBase + i + "/";
                    if (!Directory.Exists(DataWorks.FolderPlotsRemote))
                    {
                        Directory.CreateDirectory(DataWorks.FolderPlotsRemote);
                        AlgoPack.EnsureFolderStructure();
                    }

                    FullRuntime();
                    //FullStreaming();
                }
                DataWorks.FolderPlotsRemote = DataWorks.FolderPlotsRemoteBase;
            }
    
            //SingularExperiments.AverageRTRuns(codes, codesLimited, 5);
            #endif

            //SingularExperiments.MsePerformanceReport(codes, codesLimited);
            //SingularExperiments.RMSE("~/MVR/CD-RMV/incCD/_data/out/cdmvr200_k3.txt", "~/Downloads/bafu_int.csv", (0, 50, 100));

            #if false
            {
                string cmptype = "meaninit_vert";
                
                var sw_prec = new StreamWriter(File.Open($"prec_report_{cmptype}.txt", FileMode.Create));
                var sw_rt = new StreamWriter(File.Open($"rt_report_{cmptype}.txt", FileMode.Create));
                
                SingularExperiments.MsePerformanceReport(codes, codesLimited, sw_prec.WriteLine, cmptype);
                SingularExperiments.SSVIterPerformanceReport(codes, codesLimited, sw_rt.WriteLine, cmptype);
                
                sw_prec.Close();
                sw_rt.Close();
            }
            #endif  
            //
            // time series
            //
            
            #if false
            {
                //var data = DataWorks.TimeSeries("BAFU", "*.asc", 3, Utils.Specific.ParseWasserstand, new DateTime(2005, 1, 1), true);
                //DataWorks.TimeSeriesMerge(data, "BAFU_total.txt");
            }
            #endif

            FinalSequence();
        }

        private static void FinalSequence()
        {
            if (Utils.DelayedWarnings.Count > 0)
            {
                var writer = new Utils.ContinuousWriter();

                writer.WriteLine($"====={Environment.NewLine}The following delayed warnings were recorded:" +
                                 $"{Environment.NewLine}===={Environment.NewLine}");

                writer.Indent();

                while (Utils.DelayedWarnings.Count > 0)
                {
                    writer.WriteLine(Utils.DelayedWarnings.Dequeue());
                }
            }
            Console.WriteLine("--- END ---");
        }
    }
}
