using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Corning.GenSys.Logger;
using FMEA.Configuration;
using FMEA.SystemHierarchy;

namespace FMEA.Engine
{
    public class CEngine
    {
        private static ILogger ms_iLogger = CLoggerFactory.CreateLog(MethodBase.GetCurrentMethod().DeclaringType.Name);

        public static List<CElementAnalysed> PhysicalAnalysis(List<IElementBase> lstiElementBase, IConfiguration iConfiguration)
        {
            // Create objects which will store the analysis results
            Dictionary<IElementBase, CElementAnalysed> dictoElementAnalysedByiElementBase =
                new Dictionary<IElementBase, CElementAnalysed>(lstiElementBase.Count);
            List<CElementAnalysed> lstoElementAnalysedAll;
            List<CElementAnalysed> lstoElementAnalysedJunctions = new List<CElementAnalysed>();
            List<CElementAnalysed> lstoElementAnalysedElements = new List<CElementAnalysed>();
            ms_iLogger.Log(ELogLevel.Info, ">>>>>>>>>>>>>>>>>>>>>  Start of PhysicalAnalysis  <<<<<<<<<<<<<<<<<<<<<");
            ms_iLogger.Log(ELogLevel.Info, "Creation of Analysis result data containers");

            for (int nElementIndex = 0; nElementIndex < lstiElementBase.Count; nElementIndex++)
            {
                CElementAnalysed oElementAnalysed = new CElementAnalysed(lstiElementBase[nElementIndex]);
                dictoElementAnalysedByiElementBase.Add(lstiElementBase[nElementIndex], oElementAnalysed);
                if (oElementAnalysed.ElementBaseFromSystem.IsJunction != null && (bool) oElementAnalysed.ElementBaseFromSystem.IsJunction)
                {
                    lstoElementAnalysedJunctions.Add(oElementAnalysed);
                }
                else
                {
                    lstoElementAnalysedElements.Add(oElementAnalysed);
                }
            }
            lstoElementAnalysedAll = new List<CElementAnalysed>(lstoElementAnalysedElements);
            lstoElementAnalysedAll.AddRange(lstoElementAnalysedJunctions);

            ms_iLogger.Log(ELogLevel.Info, "Creation of neighbor connections");
            // Sets the neighbor connections for each ElementAnalysed object
            foreach (CElementAnalysed oElementAnalysed in lstoElementAnalysedAll)
            {
                List<CElementAnalysed> lstelementanalysedNeighbor = new List<CElementAnalysed>();
                foreach (IElementBase iElementBaseNeighbor in oElementAnalysed.ElementBaseFromSystem.NeighborList)
                {
                    lstelementanalysedNeighbor.Add(dictoElementAnalysedByiElementBase[iElementBaseNeighbor]);
                }
                oElementAnalysed.SetNeighbor(lstelementanalysedNeighbor);
            }

            // Start of iterrative Physical analysis
            // Index 0 is reserved for Initial stresses and conditions
            int nIterrationIndex_1 = 1;

            // All element base are flagged for the first iterration
            List<CElementAnalysed> lstoElementAnalysedElementBaseFlagged = new List<CElementAnalysed>(lstoElementAnalysedAll);
            ms_iLogger.Log(ELogLevel.Info, "START OF ITERRATIVE ANALYSIS");

            do
            {
                if (lstoElementAnalysedElementBaseFlagged.Count > 0)
                {
                    ms_iLogger.Log(ELogLevel.Info, string.Format("Iterration {0}: Generation", nIterrationIndex_1 + 1));
                    StressAndConditionGeneration(nIterrationIndex_1, lstoElementAnalysedElementBaseFlagged, iConfiguration);
                    ms_iLogger.Log(ELogLevel.Info, string.Format("Iterration {0}: Propagation", nIterrationIndex_1 + 1));
                    StressAndConditionPropagation(nIterrationIndex_1, lstoElementAnalysedElementBaseFlagged);
                    lstoElementAnalysedElementBaseFlagged =
                        CreateListOfFlaggedElementAndResetFlag(lstoElementAnalysedElementBaseFlagged);
                }
                nIterrationIndex_1++;
            } while (lstoElementAnalysedElementBaseFlagged.Count != 0);

            ms_iLogger.Log(ELogLevel.Info, "START OF POST PROCESSING");
            int nNumElementProcessed = 1;
            foreach (CElementAnalysed oElementAnalysed in lstoElementAnalysedAll)
            {
                ms_iLogger.Log(ELogLevel.Info, string.Format("Postprocessing elements {0}/{1}", nNumElementProcessed++, lstoElementAnalysedAll.Count));
                oElementAnalysed.FinalProcessing();
            }
            ms_iLogger.Log(ELogLevel.Info, "---------------------  End of PhysicalAnalysis  ---------------------");

            return lstoElementAnalysedAll;


            // Old requirement: treating Element and ElementJunction in separate loop
            StressAndConditionPropagation(nIterrationIndex_1++, lstoElementAnalysedJunctions);
            List<CElementAnalysed> lstoElementAnalysedJunctionsFlagged = new List<CElementAnalysed>(lstoElementAnalysedJunctions);
            List<CElementAnalysed> lstoElementAnalysedElementsFlagged = new List<CElementAnalysed>(lstoElementAnalysedElements);

            do
            {
                if (lstoElementAnalysedElementsFlagged.Count > 0)
                {
                    StressAndConditionGeneration(nIterrationIndex_1, lstoElementAnalysedElementsFlagged, iConfiguration);
                    StressAndConditionPropagation(nIterrationIndex_1, lstoElementAnalysedJunctions);
                    lstoElementAnalysedJunctionsFlagged =
                        CreateListOfFlaggedElementAndResetFlag(lstoElementAnalysedJunctions);
                }
                StressAndConditionGeneration(nIterrationIndex_1, lstoElementAnalysedJunctionsFlagged, iConfiguration);
                StressAndConditionPropagation(nIterrationIndex_1, lstoElementAnalysedElements);
                lstoElementAnalysedElementsFlagged = CreateListOfFlaggedElementAndResetFlag(lstoElementAnalysedElements);
                if (lstoElementAnalysedElementsFlagged.Count == 0)
                {
                    lstoElementAnalysedJunctionsFlagged =
                        CreateListOfFlaggedElementAndResetFlag(lstoElementAnalysedJunctions);
                }
                nIterrationIndex_1++;
            } while (lstoElementAnalysedElementsFlagged.Count != 0 || lstoElementAnalysedJunctionsFlagged.Count != 0);
            return lstoElementAnalysedAll;
        }

        private static List<CElementAnalysed> CreateListOfFlaggedElementAndResetFlag(List<CElementAnalysed> lstoElementAnalysed)
        {
            List<CElementAnalysed> lstoElementAnalysedFlagged = new List<CElementAnalysed>();
            foreach (CElementAnalysed oElementAnalysed in lstoElementAnalysed)
            {
                if (oElementAnalysed.NewStressOrConditionAdded)
                {
                    lstoElementAnalysedFlagged.Add(oElementAnalysed);
                    oElementAnalysed.ResetFlag();
                }
            }
            return lstoElementAnalysedFlagged;
        }

        /// <summary>For one iterration, for each element, uses the stresses and conditions associated with that element and determines what processes will be launched. Stores the
        /// results in the associated ElementAnalysed object.</summary>
        /// <param name="nIterrationIndex_1"></param>
        /// <param name="lstoElementAnalysed"></param>
        /// <param name="iConfiguration"></param>
        public static void StressAndConditionGeneration(int nIterrationIndex_1, List<CElementAnalysed> lstoElementAnalysed,
                                                        IConfiguration iConfiguration)
        {
            foreach (CElementAnalysed oElementAnalysed in lstoElementAnalysed)
            {
                // Use the configuration Matrix to determine the list of generated processes.
                // Each generated process also contains a list of ALL the stress and conditions that triggered the process generation
                List<IResultingProcess> lstiprocesstypeResulting = iConfiguration.GetResultingProcesses(oElementAnalysed.ElementTypeBase,
                                                                                                        oElementAnalysed.CurrentStressTypeList,
                                                                                                        oElementAnalysed.CurrentConditionTypeList);
                // Stores the list of generated processes for the current iterration in the CElementAnalysed object
                // This method also determine the list of generated processes and conditions based on each process type which contains
                // a list of generated stress type and condition types that was stored in the configuration.
                oElementAnalysed.AddGeneratedProcessesStressesConditions(nIterrationIndex_1, lstiprocesstypeResulting);
            }
        }

        public static void StressAndConditionPropagation(int nIterrationIndex, List<CElementAnalysed> lstoElementAnalysed)
        {
/*
            lstoElementAnalysed.AsParallel().Select(oElementAnalysed =>
                                                    {
                                                        oElementAnalysed.GetStressesAndConditionsFromNeighborsStep1(nIterrationIndex);
                                                        return true;
                                                    });
//*/
//*
            foreach (CElementAnalysed oElementAnalysed in lstoElementAnalysed)
            {
                oElementAnalysed.GetStressesAndConditionsFromNeighborsStep1(nIterrationIndex);
                //oElementAnalysed.GetStressesAndConditionsFromNeighborsStep2(nIterrationIndex);
            }
            foreach (CElementAnalysed oElementAnalysed in lstoElementAnalysed)
            {
                oElementAnalysed.GetStressesAndConditionsFromNeighborsStep2(nIterrationIndex);
            }
//*/
/*
            BlockingCollection<CElementAnalysed> blocoloElementAnalyseds = new BlockingCollection<CElementAnalysed>();
            foreach (CElementAnalysed oElementAnalysed in lstoElementAnalysed)
            {
                blocoloElementAnalyseds.Add(oElementAnalysed);
            }
            blocoloElementAnalyseds.CompleteAdding();
            int nNumTasks = 8;
            Task[] aoTask = new Task[nNumTasks];
                            for (int nTaskIndex = 0; nTaskIndex < nNumTasks; nTaskIndex++)
                            {
                                aoTask[nTaskIndex] = Task.Factory.StartNew(() =>
                                    {
                                        foreach ( CElementAnalysed oElementAnalysed in
                                                    blocoloElementAnalyseds.GetConsumingEnumerable())
                                        {
                                            oElementAnalysed.GetStressesAndConditionsFromNeighborsStep1(nIterrationIndex);
                                        }
                                    }, TaskCreationOptions.None);
                            }
            Task.WaitAll(aoTask);
            foreach (CElementAnalysed oElementAnalysed in lstoElementAnalysed)
            {
                oElementAnalysed.GetStressesAndConditionsFromNeighborsStep2(nIterrationIndex);
            }

//*/
        }
    }
}