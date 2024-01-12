using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Corning.GenSys.Logger;
using FMEA.Common;
using FMEA.Configuration;
using FMEA.Constants;
using OfficeOpenXml;
using OfficeOpenXml.Extension;

namespace FMEA.ConfigurationReader
{
    internal enum ETableSections
    {
        ElementType = 0,
        ElementJunctionType = 1,
        InitialStress = 2,
        //EnvironmentalStress = 3,
        InitialCondition = 4,
        InitialConditionInterface = 5,
        ResultingStress = 6,
        ResultingCondition = 7,
        ResultingConditionInterface = 8,
        FailureModes = 9,
        None = 10
    }

    public class CExcelMonsterMatrixReaderT
    {
        private static readonly string[] astrFailureModePrefixes = new string[] { "High", "Low" };
        private static readonly Dictionary<string, ETableSections> dictstretablesectionMasterMatrix
            = new Dictionary<string, ETableSections>()
                  {
                      {"Element Type", ETableSections.ElementType},
                      {"Element Interface Type", ETableSections.ElementJunctionType},
                      {"Stress-initial", ETableSections.InitialStress},
                      //{"Env stress", ETableSections.EnvironmentalStress},
                      {"Conditions-initial", ETableSections.InitialCondition},
                      {"Conditions-initial-Interface", ETableSections.InitialCondition},
                      {"Stress-resulting", ETableSections.ResultingStress},
                      {"Condition-resulting", ETableSections.ResultingCondition},
                      {"Condition-resulting-Interface", ETableSections.ResultingConditionInterface},
                      {"Failure Modes", ETableSections.FailureModes},
                  };
        private int nNUMBER_OF_SECTIONS = dictstretablesectionMasterMatrix.Count;
        private const int nSECTION_COL_INDEX = 0;
        private const int nROW_SEL_RULE_COL_INDEX = 2;
        private const int nROW_NAME_COL_INDEX = 3;
        //private const int nOTHER_JUNCTION_NAME_COL_INDEX = 3;
        private const int nENTRY_DESCRIPTION_COL_INDEX = 4;
        //private const int nMAGNITUDE_COL_INDEX = 4;
        private const int nFIRST_AUTO_COND_COL_INDEX = 5;
        private const int nLAST_AUTO_COND_COL_INDEX = 6;
        private const int nAPLLIES_TO_JUNCTION_COL_INDEX = 7;
        private const int nFIRST_DATA_COL_INDEX = 8;
        private const int nFIRST_DATA_ROW_INDEX = 6;
        private const int nPROCESS_GROUP_ROW_INDEX = 1;
        private const int nPROCESS_AUTOCOND_ROW_INDEX = 2;
        private const int nPROCESS_DESCRIPTION_ROW_INDEX = 3;
        private const int nSTRESS_OP__ROW_INDEX = 4;
        private const int nCONDITION_OP__ROW_INDEX = 5;

        public CExcelMonsterMatrixReaderT(string strXlsFilePath, string strTableName)
        {
            if (strXlsFilePath == null) throw new ArgumentNullException("strXlsFilePath");
            if (Path.GetExtension(strXlsFilePath) != ".xlsx") throw new Exception(string.Format("Invalid Excel file. FilePath: {0}", strXlsFilePath)); //throw new Exception("only files with xlsx extension are supported");
            FileInfo oFileInfo = new FileInfo(strXlsFilePath);
            ExcelPackage oExcelPackage = new ExcelPackage(oFileInfo);
            ExcelWorksheet oExcelWorksheet = oExcelPackage.Workbook.Worksheets[strTableName];
            if (oExcelWorksheet == null)
            {
                throw new Exception(string.Format("Could not find master matrix in Excel file {0}", strXlsFilePath));
            }
            m_datatableMasterMatrix = oExcelWorksheet.ReadToDataTable(new ExcelCellAddress("A1"), new ExcelCellAddress("EN463"));
            oExcelPackage.Dispose();
            //FileStream stream = File.Open(strXlsFilePath, FileMode.Open, FileAccess.Read);
            //IExcelDataReader excelReader = null;
            //string strExtension = Path.GetExtension(strXlsFilePath);
            //if (strExtension != null)
            //{
            //    strExtension = strExtension.ToLower().Trim('.');
            //}
            //if (strExtension != null && strExtension == "xls")
            //{
            //    //1. Reading from a binary Excel file ('97-2003 format; *.xls)
            //    excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
            //}
            //else if (strExtension != null && strExtension == "xlsx")
            //{
            //    //2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
            //    excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            //}
            //else
            //{
            //    throw new Exception(string.Format("Invalid Excel file. FilePath: {0}", strXlsFilePath));
            //}
            //3. DataSet - The result of each spreadsheet will be created in the result.Tables
            //DataSet datasetFull = excelReader.AsDataSet();
            //stream.Close();
            //try
            //{
            //    m_datatableMasterMatrix = datasetFull.Tables[strTableName];
            //}
            //catch (Exception)
            //{
            //    throw new Exception(string.Format("Could not find master matrix in Excel file {0}", strXlsFilePath));
            //}
            ProcessMatrix();
        }

        //private void AddToList<T> (List<T> lstT, T oT) where T: CE
        //{
        //}

        private void ProcessMatrix()
        {
            // Alows to locate each entry for a given dataset column index and detects in which section of the dataset the entry is located
            CRowEntryInDataSet[] aoRowEntryInDataSet = new CRowEntryInDataSet[m_datatableMasterMatrix.Rows.Count];
            ETableSections etablesectionCurrent = ETableSections.None;


            #region Create ElementTypes, StressTypes, ConditionTypes, FailureModeTypes

            Dictionary<int, string> dictnstrCurrent = null;
            Dictionary<int, IConditionType> dictAutoConditionNamesByColIndex = null;
            // Needs to be initialized to empty list in case we have no ElementType but ElementJunctionTypes are defined
            List<string> lstExpectedElementJunctionTypeNames = new List<string>();
            string strPreviousSectionName = "%%%";
            for (int nRowIndex = 0; nRowIndex < m_datatableMasterMatrix.Rows.Count; nRowIndex++)
            {
                string strSectionName = m_datatableMasterMatrix.Rows[nRowIndex][nSECTION_COL_INDEX].ToString();
                if (strSectionName != strPreviousSectionName && dictstretablesectionMasterMatrix.ContainsKey(strSectionName.Trim()))
                {
                    // CODE EXECUTED AT CHANGE OF SECTION
                    strPreviousSectionName = strSectionName;
                    etablesectionCurrent = dictstretablesectionMasterMatrix[strSectionName.Trim()];

                    //WARNING: This works because no new ElementType are created once we reach the ElementJunctionType
                    //         section.
                    if (etablesectionCurrent == ETableSections.ElementJunctionType)
                    {
                        lstExpectedElementJunctionTypeNames =
                            CElementJunctionType.CreateExpectedElementJunctionTypeNameList(CElementType.InstanceNameList);
                    }
                    //WARNING: This works because all conditions are created before we reach the FailureModeType
                    //         section.
                    else if (etablesectionCurrent == ETableSections.FailureModes)
                    {
                        #region Build dictionary of auto conditions
                        dictAutoConditionNamesByColIndex = new Dictionary<int, IConditionType>();
                        for (int nColIndex = nFIRST_AUTO_COND_COL_INDEX; nColIndex <= nLAST_AUTO_COND_COL_INDEX; nColIndex++)
                        {
                            string strConditionTypeName = m_datatableMasterMatrix.Rows[nPROCESS_AUTOCOND_ROW_INDEX][nColIndex].ToString();
                            IConditionType iConditionType = m_oConfiguration.GetConditionTypeByName(strConditionTypeName, false);
                            if (iConditionType == null)
                            {
                                ms_iLogger.Log(ELogLevel.Warning, "Auto condition type {0} was not found in the list of initial or resulting conditions");
                            }
                            else
                            {
                                dictAutoConditionNamesByColIndex.Add(nColIndex, iConditionType);
                            }
                        }
                        #endregion
                    }
                }
                aoRowEntryInDataSet[nRowIndex] = null;
                if (etablesectionCurrent != ETableSections.None)
                {
                    string strCurrentEntryName;
                    string strEntryDescription =
                        m_datatableMasterMatrix.Rows[nRowIndex][nENTRY_DESCRIPTION_COL_INDEX].ToString().Trim();
                    bool bOnlyOneSelectable =
                        m_datatableMasterMatrix.Rows[nRowIndex][nROW_SEL_RULE_COL_INDEX].ToString().Trim().ToLower()=="o";
                    // Passes to Element from ElementJunction
                    int nPassesToElementProbability = 0;
                    if (etablesectionCurrent != ETableSections.ElementType && etablesectionCurrent != ETableSections.ElementJunctionType)
                    {
                        string strPassesToElementProbability =
                            m_datatableMasterMatrix.Rows[nRowIndex][nAPLLIES_TO_JUNCTION_COL_INDEX].ToString().Trim().ToUpper();
                        try
                        {
                            if (strPassesToElementProbability!="")
                                if (strPassesToElementProbability == "X" || strPassesToElementProbability == "B" || strPassesToElementProbability == "A")
                                {
                                    ms_iLogger.Log(ELogLevel.Warning,
                                                   string.Format("<row {1,3}> | PassesToElementProbability is \"{0}\" which is obsolete. Probability set to {2}",
                                                   strPassesToElementProbability,
                                                   nRowIndex, CConstants.mc_nNumberDefault));
                                    nPassesToElementProbability = CConstants.mc_nNumberDefault;
                                }
                                else
                                    nPassesToElementProbability = Convert.ToInt32(strPassesToElementProbability);
                        }
                        catch (Exception)
                        {
                            ms_iLogger.Log(ELogLevel.Error,
                                           string.Format("<row {1,3}> | INVALID PassesToElementProbability: \"{0}\" cannot be converted to an integer it has been set to 0",
                                           strPassesToElementProbability,
                                           nRowIndex));
                            nPassesToElementProbability = 0;
                        }
                    }

                    //int nMagnitude = CConstants.mc_nNumberDefault;
                    //if (etablesectionCurrent == ETableSections.InitialStress || etablesectionCurrent == ETableSections.InitialCondition)
                    //{
                    //    string strMagnitude =
                    //        m_datatableMasterMatrix.Rows[nRowIndex][nMAGNITUDE_COL_INDEX].ToString().Trim();
                    //    try
                    //    {
                    //        if (strMagnitude != "")
                    //            nMagnitude = Convert.ToInt32(strMagnitude);
                    //    }
                    //    catch (Exception)
                    //    {
                    //        ms_iLogger.Log(ELogLevel.Warning,
                    //                       string.Format("<row {1,3}> | INVALID PassesToElementProbability: \"{0}\" cannot be converted to an integer",
                    //                       strMagnitude,
                    //                       nRowIndex));
                    //        nMagnitude = CConstants.mc_nNumberDefault;
                    //    }
                    //}

                    if (etablesectionCurrent != ETableSections.ElementJunctionType)
                    {
                        //VBB_TODO: Remove ToLower
                        strCurrentEntryName = m_datatableMasterMatrix.Rows[nRowIndex][nROW_NAME_COL_INDEX].ToString().Trim().ToLower();
                    }
                    else
                    {
                        //VBB_TODO: Remove ToLower
                        string strElementJunctionMatrixName = m_datatableMasterMatrix.Rows[nRowIndex][nROW_NAME_COL_INDEX].ToString().Trim().ToLower();
                        if (!string.IsNullOrWhiteSpace(strElementJunctionMatrixName))
                        {
                            string[] astrElementTypeNames = strElementJunctionMatrixName.Split(new string[] { CElementJunctionType.strELEMENT_JUNCTION_TYPE_SEPARATOR },
                                                                                               StringSplitOptions.
                                                                                                   RemoveEmptyEntries);
                            if (astrElementTypeNames.Length != 2)
                            {
                                strCurrentEntryName = string.Format("■{0}", strElementJunctionMatrixName);
                            }
                            else
                            {
                                strCurrentEntryName =
                                    CElementJunctionType.CreateElementJunctionTypeName(
                                        astrElementTypeNames[0].Trim(),
                                        astrElementTypeNames[1].Trim());
                            }
                        }
                        else
                        {
                            strCurrentEntryName = "";
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(strCurrentEntryName))
                    {

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
                                    oEntry =
                                        oElementType =
                                        m_oConfiguration.CreateElementType(nRowIndex, strCurrentEntryName,
                                                                           strEntryDescription);
                                    bItemAlreadyExists = (oElementType == null);
                                    if (bItemAlreadyExists)
                                    {
                                        //lDuplicateId = CElementTypeBase.GetByName(strCurrentEntryName).Id;
                                        lDuplicateId = m_oConfiguration.GetElementTypeByName(strCurrentEntryName).Id;
                                    }
                                    break;
                                case ETableSections.ElementJunctionType:
                                    if (strCurrentEntryName.StartsWith("■"))
                                    {
                                        ms_iLogger.Log(ELogLevel.Warning,
                                                       string.Format("<row {2,3}> | INVALID ELEMENT JUNCTION TYPE NAME: {0} \"{1}\" is not properly constructed",
                                                       etablesectionCurrent.ToString(), strCurrentEntryName.Trim('■'),
                                                       nRowIndex,
                                                       ExcelColumnFromNumber(nRowIndex)));
                                        break;
                                    }
                                    CElementJunctionType oElementJunctionType;
                                    oEntry =
                                        oElementJunctionType =
                                        m_oConfiguration.AddElementJunctionType(nRowIndex, strCurrentEntryName,
                                                                                strEntryDescription);
                                    bItemAlreadyExists = (oElementJunctionType == null);
                                    if (bItemAlreadyExists)
                                    {
                                        lDuplicateId =
                                            m_oConfiguration.GetElementJunctionTypeByName(strCurrentEntryName).Id;
                                    }

                                    // Check if the ElementJunctionType name is valid based on the ElementType names that have previously been defined
                                    if (!lstExpectedElementJunctionTypeNames.Contains(strCurrentEntryName))
                                    {
                                        ms_iLogger.Log(ELogLevel.Warning,
                                                       string.Format("<row {2,3}> | INVALID ELEMENT JUNCTION TYPE NAME: {0} \"{1}\" is not composed of two existing ElementType names",
                                                       etablesectionCurrent.ToString(), strCurrentEntryName, nRowIndex,
                                                       ExcelColumnFromNumber(nRowIndex)));
                                    }
                                    else
                                    {
                                        // Valid names are remove so that missing ElementJunctionType definitions can be flagged
                                        lstExpectedElementJunctionTypeNames.Remove(strCurrentEntryName);
                                    }
                                    break;
                                //case ETableSections.EnvironmentalStress:
                                //    CStressType oStressType;
                                //    oEntry =
                                //        oStressType =
                                //        m_oConfiguration.CreateStressType(nRowIndex, strCurrentEntryName,
                                //                                          strEntryDescription,
                                //                                          EStressCategory.Environmental, true, false, nPassesToElementProbability, 5);
                                //    bItemAlreadyExists = (oStressType == null);
                                //    if (bItemAlreadyExists)
                                //    {
                                //        lDuplicateId = m_oConfiguration.GetStressTypeByName(strCurrentEntryName).Id;
                                //    }
                                //    break;
                                case ETableSections.InitialStress:
                                    CStressType oStressType;
                                    oEntry =
                                        oStressType =
                                        m_oConfiguration.CreateStressType(nRowIndex, strCurrentEntryName,
                                                                          strEntryDescription,
                                                                          true, false, nPassesToElementProbability, 5);
                                    bItemAlreadyExists = (oStressType == null);
                                    if (bItemAlreadyExists)
                                    {
                                        lDuplicateId = m_oConfiguration.GetStressTypeByName(strCurrentEntryName).Id;
                                    }
                                    break;
                                case ETableSections.InitialCondition:
                                case ETableSections.InitialConditionInterface:
                                    CConditionType oConditionType;
                                    oEntry =
                                        oConditionType =
                                        m_oConfiguration.CreateConditionType(nRowIndex, strCurrentEntryName,
                                                                             strEntryDescription, true, false,
                                                                             nPassesToElementProbability, 5,
                                                                             etablesectionCurrent == ETableSections.InitialConditionInterface,
                                                                             bOnlyOneSelectable);
                                    bItemAlreadyExists = (oConditionType == null);
                                    if (bItemAlreadyExists)
                                    {
                                        lDuplicateId = m_oConfiguration.GetConditionTypeByName(strCurrentEntryName).Id;
                                    }
                                    break;
                                case ETableSections.ResultingStress:
                                    //We expect a null because all resulting StressTypes should be part of the initial stress
                                    //types to have an effect on the physical analysis
                                    oEntry =
                                        oStressType =
                                        m_oConfiguration.CreateStressType(nRowIndex, strCurrentEntryName,
                                                                          strEntryDescription,
                                                                          false, true, nPassesToElementProbability, 5);
                                    if (oStressType != null)
                                    {
                                        ms_iLogger.Log(ELogLevel.Warning,
                                                       string.Format("<row {1,3}> | RESULTING STRESS NOT IN INITIAL STRESSES: IsResulting Stress \"{0}\" is not part of the initial stresses. It will therefore have no effect during the physical analysis.",
                                                       strCurrentEntryName, nRowIndex, ExcelColumnFromNumber(nRowIndex)));
                                    }
                                    else
                                    {
                                        oEntry =
                                            oStressType =
                                            (CStressType) m_oConfiguration.GetStressTypeByName(strCurrentEntryName);
                                        oStressType.IsResulting = true;
                                    }
                                    break;
                                case ETableSections.ResultingCondition:
                                case ETableSections.ResultingConditionInterface:
                                    //We expect a null because all resulting ConditionTypes should be part of the initial condition
                                    //types to have an effect on the physical analysis
                                    oEntry =
                                        oConditionType =
                                        m_oConfiguration.CreateConditionType(nRowIndex, strCurrentEntryName,
                                                                             strEntryDescription,
                                                                             false, true, nPassesToElementProbability, 5,
                                                                             etablesectionCurrent == ETableSections.ResultingConditionInterface,
                                                                             bOnlyOneSelectable);
                                    if (oConditionType != null)
                                    {
                                        ms_iLogger.Log(ELogLevel.Warning,
                                                       string.Format("<row {1,3}> | RESULTING CONDITION NOT IN INITIAL CONDITIONS: Resulting Condition \"{0}\" is not part of the initial conditions. It will therefore have no effect during the physical analysis.",
                                                       strCurrentEntryName, nRowIndex, ExcelColumnFromNumber(nRowIndex)));
                                    }
                                    else
                                    {
                                        oEntry =
                                            oConditionType =
                                            (CConditionType)
                                            m_oConfiguration.GetConditionTypeByName(strCurrentEntryName);
                                        oConditionType.IsResulting = true;
                                    }
                                    break;
                                case ETableSections.FailureModes:
                                    if (!CFailureModeType.IsNameProperlyConstructed(strCurrentEntryName))
                                    {
                                        ms_iLogger.Log(ELogLevel.Warning,
                                                       string.Format("<row {1,3}> | INVALID FAILURE MODE NAME: Name \"{0}\" is an invalid name for a failure mode.",
                                                       strCurrentEntryName, nRowIndex, ExcelColumnFromNumber(nRowIndex)));
                                    }
                                    else
                                    {
                                        CFailureModeType oFailureModeType;
                                        List<IEntryWithNumber<IConditionType>> lstAutoConditionTypeWithMagnitudes = new List<IEntryWithNumber<IConditionType>>();
                                        for (int nColIndex = nFIRST_AUTO_COND_COL_INDEX; nColIndex <= nLAST_AUTO_COND_COL_INDEX; nColIndex++)
                                        {
                                            if (dictAutoConditionNamesByColIndex.ContainsKey(nColIndex))
                                            {
                                                string strAutoConditionMagnitude =
                                                    m_datatableMasterMatrix.Rows[nRowIndex][nColIndex].ToString();
                                                if (!string.IsNullOrWhiteSpace(strAutoConditionMagnitude))
                                                {
                                                    IConditionType iConditionType = dictAutoConditionNamesByColIndex[nColIndex];
                                                    int nAutoConditionMagnitude;
                                                    if (int.TryParse(strAutoConditionMagnitude, out nAutoConditionMagnitude))
                                                    {
                                                        lstAutoConditionTypeWithMagnitudes.Add(new CEntryWithNumber<IConditionType>(iConditionType,
                                                                                                                              nAutoConditionMagnitude));
                                                    }
                                                    else
                                                    {
                                                        ms_iLogger.Log(ELogLevel.Warning,
                                                                       string.Format(
                                                                                     "For failure mode {0} at row {1} the following magnitude \"{2}\" was provided for auto-condition {3} which cannot be converted to an integer",
                                                                                     strCurrentEntryName, nRowIndex, strAutoConditionMagnitude,
                                                                                     iConditionType.Name));
                                                    }
                                                }
                                            }
                                        }
                                        //VBB_TODO: Replace the two booleans (PropagateToElement, PropagateToElementJunction) with what is read from the matrix once it is has been added
                                        oEntry =
                                            oFailureModeType =
                                            m_oConfiguration.CreateFailureModeType(nRowIndex, strCurrentEntryName,
                                                                                   strEntryDescription, true, true, lstAutoConditionTypeWithMagnitudes);
                                        bItemAlreadyExists = (oFailureModeType == null);
                                        if (bItemAlreadyExists)
                                        {
                                            lDuplicateId = CElementTypeBase.GetByName(strCurrentEntryName).Id;
                                        }
                                    }
                                    break;
                            }

                            if (oEntry != null)
                            {
                                aoRowEntryInDataSet[nRowIndex] = new CRowEntryInDataSet(oEntry, etablesectionCurrent,
                                                                                        nRowIndex);
                            }
                            if (bItemAlreadyExists)
                            {
                                ms_iLogger.Log(ELogLevel.Warning,
                                               string.Format("<row {2,3}> | DUPLICATE ENTRY: {0} \"{1}\" is already defined in <row {4,3}>. This row will be ignored.",
                                               etablesectionCurrent.ToString(), strCurrentEntryName, nRowIndex,
                                               ExcelColumnFromNumber(nRowIndex),
                                               lDuplicateId, ExcelColumnFromNumber(lDuplicateId)));
                            }
                        }
                        catch (Exception oException)
                        {
                            ms_iLogger.Log(ELogLevel.Warning,
                                           string.Format("Exception while processing Matrix.\nException:\n{0}\n{1}",
                                                         oException.Message, oException.StackTrace));
                        }
                    }
                }
            }
            foreach (string strMissingElementJunctionTypeName in lstExpectedElementJunctionTypeNames)
            {
                ms_iLogger.Log(ELogLevel.Warning,
                               string.Format("MISSING ELEMENT JUNCTION TYPE: Definition of ElementJunctionType named \"{0}\" is missing",
                               strMissingElementJunctionTypeName));
            }

            #endregion Create ElementTypes, StressTypes, ConditionTypes, FailureModeTypes

            #region Add ProcessTypes, ProcessMapper

            //The following list is used to check for duplicate process names
            List<string> lststrProcessName = new List<string>();
            for (int nColIndex = nFIRST_DATA_COL_INDEX; nColIndex < m_datatableMasterMatrix.Columns.Count; nColIndex++)
            {
                //DataRow datarowSource = m_datatableMasterMatrix.Rows[nColIndex];
                string strRowProcessGroup = m_datatableMasterMatrix.Rows[nPROCESS_GROUP_ROW_INDEX][nColIndex].ToString().Trim();
                string strRowProcessName = m_datatableMasterMatrix.Rows[nPROCESS_AUTOCOND_ROW_INDEX][nColIndex].ToString().Trim();
                string strRowProcessDescription = m_datatableMasterMatrix.Rows[nPROCESS_DESCRIPTION_ROW_INDEX][nColIndex].ToString().Trim();
                EOperation operationStress =
                    CProcessType.OperationFromString(
                        m_datatableMasterMatrix.Rows[nSTRESS_OP__ROW_INDEX][nColIndex].ToString());
                EOperation operationCondition =
                    CProcessType.OperationFromString(
                        m_datatableMasterMatrix.Rows[nCONDITION_OP__ROW_INDEX][nColIndex].ToString());

                if (strRowProcessName != "")
                {
                    if (lststrProcessName.Contains(strRowProcessName))
                    {
                        ms_iLogger.Log(ELogLevel.Warning,
                                       string.Format("<col. {1,3}_{2}> | DUPLICATE PROCESS ENTRY: \"{0}\" is already defined.",
                                       strRowProcessName, nColIndex, ExcelColumnFromNumber(nColIndex)));
                    }
                    //The next 3 lists are used to build a matrix line of the process mapper.
                    List<IElementTypeBase> lstlElementTypeBaseIndex = new List<IElementTypeBase>();
                    List<IStressType> lstlInitialStressIndex  = new List<IStressType>();
                    List<IConditionType> lstlInitialConditionIndex = new List<IConditionType>();

                    //This list is used to add the allowable failure modes to each element type
                    List<CElementTypeBase> lstoElementTypeBase = new List<CElementTypeBase>();
                    List<CFailureModeType> lstoFailureModeType = new List<CFailureModeType>();

                    CProcessType oProcessType;
                    oProcessType = m_oConfiguration.CreateProcessType(nColIndex + 10000, strRowProcessGroup, strRowProcessName, strRowProcessDescription, operationStress, operationCondition);

                    for (int nRowIndex = nFIRST_DATA_ROW_INDEX;
                         nRowIndex < m_datatableMasterMatrix.Rows.Count;
                         nRowIndex++)
                    {
                        if (aoRowEntryInDataSet[nRowIndex] != null && aoRowEntryInDataSet[nRowIndex].Entry != null)
                        {
                            string strCellData = m_datatableMasterMatrix.Rows[nRowIndex][nColIndex].ToString().Trim().ToUpper();
                            int nCellData = 0;
                            if (strCellData != "")
                            {
                                if (strCellData == "A" || strCellData == "B" || strCellData == "X")
                                {
                                    nCellData = CConstants.mc_nNumberDefault;
                                }
                                else
                                {
                                    try
                                    {
                                        nCellData = Convert.ToInt32(strCellData);
                                    }
                                    catch (Exception)
                                    {
                                        ms_iLogger.Log(ELogLevel.Warning,
                                                     string.Format(
                                                                   "<row {1,3}> <col {2,3} | INVALID CELL VALUE: \"{0}\" is not a number or A or B",
                                                                   strCellData,
                                                                   nRowIndex, ExcelColumnFromNumber(nColIndex)));
                                        nCellData = CConstants.mc_nNumberDefault;
                                    }
                                }
                            }
                            if (nCellData > 0)
                            {
                                switch (aoRowEntryInDataSet[nRowIndex].TableSection)
                                {
                                    case ETableSections.ElementType:
                                    case ETableSections.ElementJunctionType:
                                        lstlElementTypeBaseIndex.Add((IElementTypeBase) aoRowEntryInDataSet[nRowIndex].Entry);
                                        lstoElementTypeBase.Add((CElementTypeBase) aoRowEntryInDataSet[nRowIndex].Entry);
                                        if (aoRowEntryInDataSet[nRowIndex].TableSection == ETableSections.ElementType)
                                        {
                                            oProcessType.AddElementType((IElementType) aoRowEntryInDataSet[nRowIndex].Entry);
                                        }
                                        else
                                        {
                                            oProcessType.AddElementJunctionType((IElementJunctionType)aoRowEntryInDataSet[nRowIndex].Entry);
                                        }
                                        break;
                                    case ETableSections.InitialStress:
                                        lstlInitialStressIndex.Add((IStressType) aoRowEntryInDataSet[nRowIndex].Entry);
                                        oProcessType.AddInitialStressType((IStressType) aoRowEntryInDataSet[nRowIndex].Entry);
                                        break;
                                    case ETableSections.InitialCondition:
                                        lstlInitialConditionIndex.Add((IConditionType) aoRowEntryInDataSet[nRowIndex].Entry);
                                        oProcessType.AddInitialConditionType((IConditionType) aoRowEntryInDataSet[nRowIndex].Entry);
                                        break;
                                    case ETableSections.ResultingStress:
                                        oProcessType.AddResultingStressType((CStressType) aoRowEntryInDataSet[nRowIndex].Entry, nCellData);
                                        break;
                                    case ETableSections.ResultingCondition:
                                        oProcessType.AddResultingConditionType((CConditionType) aoRowEntryInDataSet[nRowIndex].Entry, nCellData);
                                        break;
                                    case ETableSections.FailureModes:
                                        CFailureModeType oFailureModeType =
                                            (CFailureModeType) aoRowEntryInDataSet[nRowIndex].Entry;
                                        oProcessType.AddCausedFailureModeType(oFailureModeType, nCellData);
                                        oFailureModeType.AddCausingProcessType(oProcessType);
                                        foreach (CElementTypeBase oElementTypeBase in lstoElementTypeBase)
                                        {
                                            oElementTypeBase.AddFailureModeType(oFailureModeType);
                                        }
                                        break;
                                }
                            }
                        }
                        ////Check and process section change
                        //string strSectionName = m_datatableMasterMatrix.Rows[nSECTION_COL_INDEX][nColIndex].ToString();
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
                        //        m_datatableMasterMatrix.Rows[nRowindex][nColIndex].ToString().Trim() != "";
                        //}
                    }
                    m_oConfiguration.AddProcessToMapper(oProcessType, lstlElementTypeBaseIndex.ToArray(),
                                                        lstlInitialStressIndex.ToArray(),
                                                        lstlInitialConditionIndex.ToArray());
                    //if (datatableCurrent != null)
                    //    datatableCurrent.Rows.Add(datarowCurrent);
                }
            }

            #endregion Add ProcessTypes, ProcessMapper
        }

        //private void LogLoadError(ELogLevel oLogLevel, string strMessage)
        //{
        //    ms_iLogger.Log(oLogLevel, strMessage);
        //    m_oStringBuilderLoadErrors.AppendLine(strMessage);
        //}

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

        public string LoadWarnings
        {
            get { return m_oStringBuilderLoadErrors.Length == 0 ? null : m_oStringBuilderLoadErrors.ToString(); }
        }

        //public Configuration GetConfiguration()
        //{
        //    return new Configuration();
        //    //List<CElementType> lstoElementType, List<CElementJunctionType> lstoElementJunctionType, List<CStressType> lstoStressType, List<CConditionType> lstoConditionType, CProcessMapper oProcessMapper)
        //}

        private readonly DataTable m_datatableMasterMatrix;

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

        private StringBuilder m_oStringBuilderLoadErrors = new StringBuilder();

        //private Configuration m_oConfiguration;


        private static ILogger ms_iLogger = CLoggerFactory.CreateLog(MethodBase.GetCurrentMethod().DeclaringType.Name);

        private class CRowEntryInDataSet
        {
            public CRowEntryInDataSet(CEntry oEntry, ETableSections eTableSection, int nMasterMatrixRowIndex)//string strMasterMatrixColumnName)
            {
                m_oEntry = oEntry;
                m_eTableSection = eTableSection;
                m_nMasterMatrixRowIndex = nMasterMatrixRowIndex;
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

            public int MasterMatrixRowIndex
            {
                get { return m_nMasterMatrixRowIndex; }
            }

            private CEntry m_oEntry;
            private ETableSections m_eTableSection;
            private int m_nMasterMatrixRowIndex;
        }
    }
}