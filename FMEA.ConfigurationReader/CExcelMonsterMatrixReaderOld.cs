using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Excel;
using FMEA.Common;
using FMEA.Configuration;
using NLog;
using Niceasoft.ExtensionMethods;

namespace FMEA.ConfigurationReader
{
    public class CExcelMonsterMatrixReaderOld
    {
        private static readonly Dictionary<string, ETableSections> dictstretablesectionMasterMatrix
            = new Dictionary<string, ETableSections>()
                  {
                      {"ET", ETableSections.ElementType},
                      {"EIT", ETableSections.ElementJunctionType},
                      {"Op stress", ETableSections.OperationalStress},
                      {"Env stress", ETableSections.EnvironmentalStress},
                      {"IsInitial conditions", ETableSections.InitialCondition},
                      {"IsResulting operational stress", ETableSections.ResultingStress},
                      {"IsResulting conditions (CR)", ETableSections.ResultingCondition},
                      {"Failure Modes", ETableSections.FailureModes},
                  };
        private int nNUMBER_OF_SECTIONS = dictstretablesectionMasterMatrix.Count;
        private const int nSECTION_ROW_INDEX = 0;
        private const int nCOLUMN_NAME_ROW_INDEX = 2;
        private const int nOTHER_JUNCTION_NAME_ROW_INDEX = 1;
        private const int nFIRST_DATA_ROW_INDEX = 3;
        private const int nFIRST_DATA_COLUMN_INDEX = 2;
        private const int nPROCESS_COLUMN_INDEX = 1;

        public CExcelMonsterMatrixReaderOld(string strXlsFilePath)
        {
            if (strXlsFilePath == null) throw new ArgumentNullException("strXlsFilePath");
            FileStream stream = File.Open(strXlsFilePath, FileMode.Open, FileAccess.Read);
            IExcelDataReader excelReader = null;
            string strExtension = Path.GetExtension(strXlsFilePath);
            if (strExtension!=null)
            {
                strExtension = strExtension.ToLower().Trim('.');
            }
            if (strExtension!=null && strExtension == "xls")
            {
                //1. Reading from a binary Excel file ('97-2003 format; *.xls)
                excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
            }
            else if (strExtension != null && strExtension == "xlsx")
            {
                //2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
                excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            }
            else
            {
                throw new Exception(string.Format("Invalid Excel file. FilePath: {0}", strXlsFilePath));
            }
            //3. DataSet - The result of each spreadsheet will be created in the result.Tables
            DataSet datasetFull = excelReader.AsDataSet();
            stream.Close();
            try
            {
                datatableMasterMatrix = datasetFull.Tables["master matrix"];
            }
            catch (Exception)
            {
                throw new Exception(string.Format("Could not find master matrix in Excel file {0}", strXlsFilePath));
            }
            ProcessMatrix();
        }

        //private void AddToList<T> (List<T> lstT, T oT) where T: CE
        //{
            
        //}

        private void ProcessMatrix()
        {
            // Alows to locate each entry for a given dataset column index and detects in which section of the dataset the entry is located
            CColumnEntryInDataSet[] aoColumnEntryInDataSet = new CColumnEntryInDataSet[datatableMasterMatrix.Columns.Count];
            ETableSections etablesectionCurrent = ETableSections.None;

            #region Create ElementTypes, StressTypes, ConditionTypes, FailureModeTypes
//            Dictionary<int, string> dictnstrCurrent = null;

            // Needs to be initialized to empty list in case we have no ElementType but ElementJunctionTypes are defined
            List<string> lstExpectedElementJunctionTypeNames = new List<string>();

            for (int nColIndex = 0; nColIndex < datatableMasterMatrix.Columns.Count; nColIndex++)
            {
                string strSectionName = datatableMasterMatrix.Rows[nSECTION_ROW_INDEX][nColIndex].ToString();
                if (dictstretablesectionMasterMatrix.ContainsKey(strSectionName.Trim()))
                {
                    etablesectionCurrent = dictstretablesectionMasterMatrix[strSectionName.Trim()];

                    //WARNING: This works because no new ElementType are created once we reach the ElementJunctionType
                    //         section.
                    if(etablesectionCurrent==ETableSections.ElementJunctionType)
                    {
                        lstExpectedElementJunctionTypeNames =
                            CElementJunctionType.CreateExpectedElementJunctionTypeNameList(CElementType.InstanceNameList);
                    }
                }
                aoColumnEntryInDataSet[nColIndex] = null;
                if(etablesectionCurrent != ETableSections.None)
                {
                    string strCurrentEntryName;
                    if (etablesectionCurrent != ETableSections.ElementJunctionType)
                    {
                        strCurrentEntryName = datatableMasterMatrix.Rows[nCOLUMN_NAME_ROW_INDEX][nColIndex].ToString().Trim();
                    }
                    else
                    {
                        strCurrentEntryName =
                            CElementJunctionType.CreateElementJunctionTypeName(
                                datatableMasterMatrix.Rows[nOTHER_JUNCTION_NAME_ROW_INDEX][nColIndex].ToString().Trim(),
                                datatableMasterMatrix.Rows[nCOLUMN_NAME_ROW_INDEX][nColIndex].ToString().Trim());
                    }
                    try
                    {

                        bool bItemAlreadyExists = false;
                        long lDuplicateId = -1;
                        CEntry oEntry = null;
                        //Create configuration objects
                        switch (etablesectionCurrent)
                        {
                            case ETableSections.ElementType:
                                CElementType oElementType;
                                //oEntry = oElementType = CElementType.Create(nColIndex, strCurrentEntryName, "");
                                oEntry = oElementType = m_oConfiguration.CreateElementType(nColIndex, strCurrentEntryName, "");
                                bItemAlreadyExists = (oElementType == null);
                                if (bItemAlreadyExists)
                                {
                                    //lDuplicateId = CElementTypeBase.GetByName(strCurrentEntryName).Id;
                                    lDuplicateId = m_oConfiguration.GetElementTypeByName(strCurrentEntryName).Id;
                                }
                                break;
                            case ETableSections.ElementJunctionType:
                                CElementJunctionType oElementJunctionType;
                                oEntry = oElementJunctionType = m_oConfiguration.AddElementJunctionType(nColIndex, strCurrentEntryName, "");
                                bItemAlreadyExists = (oElementJunctionType == null);
                                if (bItemAlreadyExists)
                                {
                                    lDuplicateId = m_oConfiguration.GetElementJunctionTypeByName(strCurrentEntryName).Id;
                                }

                                // Check if the ElementJunctionType name is valid based on the ElementType names that have previously been defined
                                if (!lstExpectedElementJunctionTypeNames.Contains(strCurrentEntryName))
                                {
                                    ms_oLogger.Log(LogLevel.Warn,
                                            "<col. {2,3}_{3}> | INVALID ELEMENT JUNCTION TYPE NAME: {0} \"{1}\" is not composed of two existing ElementType names",
                                            etablesectionCurrent.ToString(), strCurrentEntryName, nColIndex,
                                            ExcelColumnFromNumber(nColIndex));
                                }
                                else
                                {
                                    // Valid names are remove so that missing ElementJunctionType definitions can be flagged
                                    lstExpectedElementJunctionTypeNames.Remove(strCurrentEntryName);
                                }
                                break;
                            case ETableSections.EnvironmentalStress:
                                CStressType oStressType;
                                oEntry = oStressType = m_oConfiguration.CreateStressType(nColIndex, strCurrentEntryName, "",
                                                                             EStressCategory.Environmental, true, false);
                                bItemAlreadyExists = (oStressType == null);
                                if (bItemAlreadyExists)
                                {
                                    lDuplicateId = m_oConfiguration.GetStressTypeByName(strCurrentEntryName).Id;
                                }
                                break;
                            case ETableSections.OperationalStress:
                                oEntry = oStressType = m_oConfiguration.CreateStressType(nColIndex, strCurrentEntryName, "",
                                                                             EStressCategory.Operational, true, false);
                                bItemAlreadyExists = (oStressType == null);
                                if (bItemAlreadyExists)
                                {
                                    lDuplicateId = m_oConfiguration.GetStressTypeByName(strCurrentEntryName).Id;
                                }
                                break;
                            case ETableSections.InitialCondition:
                                CConditionType oConditionType;
                                //VBB_TODO: Replace the last two booleans (AppliesToJunctions, AppliesToElements) with what is read from the matrix once it is has been added
                                oEntry = oConditionType = m_oConfiguration.CreateConditionType(nColIndex, strCurrentEntryName, "", true, false, true, true);
                                bItemAlreadyExists = (oConditionType == null);
                                if (bItemAlreadyExists)
                                {
                                    lDuplicateId = m_oConfiguration.GetConditionTypeByName(strCurrentEntryName).Id;
                                }
                                break;
                            case ETableSections.ResultingStress:
                                //We expect a null because all resulting StressTypes should be part of the initial stress
                                //types to have an effect on the physical analysis
                                oEntry = oStressType = m_oConfiguration.CreateStressType(nColIndex, strCurrentEntryName, "",
                                                                             EStressCategory.Operational, false, true);
                                if (oStressType != null)
                                {
                                    ms_oLogger.Log(LogLevel.Warn,
                                                   "<col. {1,3}_{2}> | RESULTING STRESS NOT IN INITIAL STRESSES: IsResulting Stress \"{0}\" is not part of the initial stresses. It will therefore have no effect during the physical analysis.",
                                                   strCurrentEntryName, nColIndex, ExcelColumnFromNumber(nColIndex));
                                }
                                else
                                {
                                    oEntry = oStressType = (CStressType) m_oConfiguration.GetStressTypeByName(strCurrentEntryName);
                                    oStressType.IsResulting = true;
                                }
                                break;
                            case ETableSections.ResultingCondition:
                                //We expect a null because all resulting ConditionTypes should be part of the initial condition
                                //types to have an effect on the physical analysis
                                //VBB_TODO: Replace the last two booleans (AppliesToJunctions, AppliesToElements) with what is read from the matrix once it is has been added
                                oEntry = oConditionType = m_oConfiguration.CreateConditionType(nColIndex, strCurrentEntryName, "",
                                                                       false, true, true, true);
                                if (oConditionType != null)
                                {
                                    ms_oLogger.Log(LogLevel.Warn,
                                                   "<col. {1,3}_{2:2}> | RESULTING CONDITION NOT IN INITIAL CONDITIONS: IsResulting Condition \"{0}\" is not part of the initial conditions. It will therefore have no effect during the physical analysis.",
                                                   strCurrentEntryName, nColIndex, ExcelColumnFromNumber(nColIndex));
                                }
                                else
                                {
                                    oEntry = oConditionType = (CConditionType) m_oConfiguration.GetConditionTypeByName(strCurrentEntryName);
                                    oConditionType.IsResulting = true;
                                }
                                break;
                            case ETableSections.FailureModes:
                                CFailureModeType oFailureModeType;
                                //VBB_TODO: Replace the two booleans (AppliesToJunctions, AppliesToElements) with what is read from the matrix once it is has been added
                                oEntry = oFailureModeType = m_oConfiguration.CreateFailureModeType(nColIndex, strCurrentEntryName, "", true, true);
                                bItemAlreadyExists = (oFailureModeType == null);
                                if (bItemAlreadyExists)
                                {
                                    lDuplicateId = CElementTypeBase.GetByName(strCurrentEntryName).Id;
                                }
                                break;
                        }

                        if (oEntry!=null)
                        {
                            aoColumnEntryInDataSet[nColIndex] = new CColumnEntryInDataSet(oEntry, etablesectionCurrent,
                                                                                          datatableMasterMatrix.Columns[
                                                                                              nColIndex].ColumnName);
                        }
                        if (bItemAlreadyExists)
                        {
                            ms_oLogger.Log(LogLevel.Warn,
                                           "<col. {2,3}_{3}> | DUPLICATE ENTRY: {0} \"{1}\" is already defined in <col. {4}_{5}>. This column will be ignored.",
                                           etablesectionCurrent.ToString(), strCurrentEntryName, nColIndex,
                                           ExcelColumnFromNumber(nColIndex),
                                           lDuplicateId, ExcelColumnFromNumber(lDuplicateId));
                        }
                    }
                    catch (Exception oException)
                    {
                        ms_oLogger.Log(LogLevel.Error,
                                           string.Format("Exception while processing Matrix.\nException:\n{0}\n{1}", oException.Message, oException.StackTrace));
                    }
                }
            }
            foreach (string strMissingElementJunctionTypeName in lstExpectedElementJunctionTypeNames)
            {
                ms_oLogger.Log(LogLevel.Warn,
                               "MISSING ELEMENT JUNCTION TYPE: Definition of ElementJunctionType named \"{0}\" is missing",
                               strMissingElementJunctionTypeName);
            }
            #endregion //Add columns to DataTables

            #region Add ProcessTypes, ProcessMapper
            for (int nRowindex = nFIRST_DATA_ROW_INDEX; nRowindex < datatableMasterMatrix.Rows.Count; nRowindex++)
            {
                DataRow datarowSource = datatableMasterMatrix.Rows[nRowindex];
                string strRowProcessName = datarowSource[nPROCESS_COLUMN_INDEX].ToString().Trim();

                if (strRowProcessName != "")
                {
                    //The next 3 lists are used to build a matrix line of the process mapper.
                    List<long> lstlElementTypeBaseIndex = new List<long>();
                    List<long> lstlInitialStressIndex = new List<long>();
                    List<long> lstlInitialConditionIndex = new List<long>();

                    //This list is used to add the allowable failure modes to each element type
                    List<CElementTypeBase> lstoElementTypeBase = new List<CElementTypeBase>();
                    List<CFailureModeType> lstoFailureModeType = new List<CFailureModeType>();

                    CProcessType oProcessType;
                    oProcessType = m_oConfiguration.CreateProcessType(nRowindex+10000, strRowProcessName, "");
 
                    for (int nColIndex = nFIRST_DATA_COLUMN_INDEX;
                         nColIndex < datatableMasterMatrix.Columns.Count;
                         nColIndex++)
                    {
                        if (aoColumnEntryInDataSet[nColIndex]!=null && aoColumnEntryInDataSet[nColIndex].Entry!=null && datarowSource[nColIndex].ToString().Trim() != "")
                        {
                            switch (aoColumnEntryInDataSet[nColIndex].TableSection)
                            {
                                case ETableSections.ElementType:
                                case ETableSections.ElementJunctionType:
                                    lstlElementTypeBaseIndex.Add(aoColumnEntryInDataSet[nColIndex].Entry.Id);
                                    lstoElementTypeBase.Add((CElementTypeBase)aoColumnEntryInDataSet[nColIndex].Entry);
                                    break;
                                case ETableSections.OperationalStress:
                                case ETableSections.EnvironmentalStress:
                                    lstlInitialStressIndex.Add(aoColumnEntryInDataSet[nColIndex].Entry.Id);
                                    break;
                                case ETableSections.InitialCondition:
                                    lstlInitialConditionIndex.Add(aoColumnEntryInDataSet[nColIndex].Entry.Id);
                                    break;
                                case ETableSections.ResultingStress:
                                    oProcessType.AddResultingStressType((CStressType)aoColumnEntryInDataSet[nColIndex].Entry);
                                    break;
                                case ETableSections.ResultingCondition:
                                    oProcessType.AddResultingConditionType((CConditionType)aoColumnEntryInDataSet[nColIndex].Entry);
                                    break;
                                case ETableSections.FailureModes:
                                    CFailureModeType oFailureModeType =
                                        (CFailureModeType) aoColumnEntryInDataSet[nColIndex].Entry;
                                    oProcessType.AddCausedFailureModeType(oFailureModeType);
                                    oFailureModeType.AddCausingProcessType(oProcessType);
                                    foreach (CElementTypeBase oElementTypeBase in lstoElementTypeBase)
                                    {
                                        oElementTypeBase.AddFailureModeType(oFailureModeType);
                                    }
                                    break;
                            }
                        }
                        ////Check and process section change
                        //string strSectionName = datatableMasterMatrix.Rows[nSECTION_ROW_INDEX][nColIndex].ToString();
                        //if (dictstretablesectionMasterMatrix.ContainsKey(strSectionName.Trim()))
                        //{
                        //    etablesectionCurrent = dictstretablesectionMasterMatrix[strSectionName.Trim()];
                        //    if (datatableCurrent != null)
                        //    //etablesectionCurrent != ETableSections.ElementType)
                        //    {
                        //        datatableCurrent.Rows.Add(datarowCurrent);
                        //    }
                        //    datatableCurrent = datasetReprocessed.Tables[etablesectionCurrent.ToString()];
                        //    datarowCurrent = datatableCurrent.NewRow();
                        //    datarowCurrent[0] = strRowProcessName;
                        //    nCurrentRowIndex = 1;
                        //}
                        //if (datarowCurrent != null && etablesectionCurrent != ETableSections.None)
                        //{
                        //    datarowCurrent[nCurrentRowIndex++] =
                        //        datatableMasterMatrix.Rows[nRowindex][nColIndex].ToString().Trim() != "";
                        //}
                    }
                    //m_oConfiguration.AddProcessToMapper(oProcessType, lstlElementTypeBaseIndex.ToArray(),
                    //                                    lstlInitialConditionIndex.ToArray(),
                    //                                    lstlInitialStressIndex.ToArray());
                    m_oConfiguration.AddProcessToMapper(oProcessType, lstoElementTypeBase.ToArray(),
                                                        lstlInitialConditionIndex.ToArray(),
                                                        lstlInitialStressIndex.ToArray());
                    //if (datatableCurrent != null)
                    //    datatableCurrent.Rows.Add(datarowCurrent);
                }
            }
            #endregion //Add Data to DataTables
        }

        private static string ExcelColumnFromNumber(long column)
        {
            string columnString = "";
            decimal columnNumber = column;
            while (columnNumber > 0)
            {
                decimal currentLetterNumber = (columnNumber - 1) % 26;
                char currentLetter = (char)(currentLetterNumber + 65);
                columnString = currentLetter + columnString;
                columnNumber = (columnNumber - (currentLetterNumber + 1)) / 26;
            }
            int nLength = 2;
            return (columnString ?? "").Length > nLength ? (columnString ?? "").Substring(0, nLength) : (columnString ?? "").PadRight(nLength);
        }


        public CProcessMapper ProcessMapper
        {
            get { return m_oProcessMapper; }
        }

        //public Configuration.Configuration Configuration
        //{
        //    get { return m_oConfiguration; }
        //}

        public IConfiguration Configuration
        {
            get { return m_oConfiguration; }
        }

        //public Configuration GetConfiguration()
        //{
        //    return new Configuration();
        //    //List<CElementType> lstoElementType, List<CElementJunctionType> lstoElementJunctionType, List<CStressType> lstoStressType, List<CConditionType> lstoConditionType, CProcessMapper oProcessMapper)
        //}

        private readonly DataTable datatableMasterMatrix;

        private List<CElementType> m_lstoElementType = new List<CElementType>();
        private List<CElementJunctionType> m_lstoElementJunctionType = new List<CElementJunctionType>();
        private List<CStressType> m_lstoStressType = new List<CStressType>();
        private List<CConditionType> m_lstoConditionType = new List<CConditionType>();
        private List<CStressType> m_lststresstypeResulting = new List<CStressType>();
        private List<CConditionType> m_lstconditiontypeResulting = new List<CConditionType>();
        private List<CFailureModeType> m_lstoFailureModeType = new List<CFailureModeType>();
        private List<CProcessType> m_lstoProcessType = new List<CProcessType>();
        private CProcessMapper m_oProcessMapper = new CProcessMapper();


        private Configuration.Configuration m_oConfiguration = new Configuration.Configuration();

        //private Configuration m_oConfiguration;

        private static Logger ms_oLogger = LogManager.GetCurrentClassLogger();

        private class CColumnEntryInDataSet
        {
            public CColumnEntryInDataSet(CEntry oEntry, ETableSections eTableSection, string strMasterMatrixColumnName)
            {
                m_oEntry = oEntry;
                m_eTableSection = eTableSection;
                m_strMasterMatrixColumnName = strMasterMatrixColumnName;
            }

            public CEntry Entry
            {
                get { return m_oEntry; }
                //set { m_oEntry = value; }
            }

            public ETableSections TableSection
            {
                get { return m_eTableSection; }
                //set { m_eTableSection = value; }
            }

            public string StrMasterMatrixColumnName
            {
                get { return m_strMasterMatrixColumnName; }
            }

            private CEntry m_oEntry;
            private ETableSections m_eTableSection;
            private string m_strMasterMatrixColumnName;
        }
    }
}
