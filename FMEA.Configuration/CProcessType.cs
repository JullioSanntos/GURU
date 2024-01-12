using System;
using System.Collections.Generic;
using System.Linq;
using FMEA.Common;

namespace FMEA.Configuration
{
    public class CProcessType : CEntry, IProcessType, IProcessTypeWrite
    {
        #region Constructors

        private CProcessType(long lId, string strName, string strDescription)
            : base(lId, strName, strDescription)
        {
        }

        public CProcessType(long lId, string strName, string strDescription, string strOperationStress,
                            string strOperationCondition)
            : base(lId, strName, strDescription)
        {
            m_operationStress = OperationFromString(strOperationStress);
            m_operationCondition = OperationFromString(strOperationCondition);
        }

        #endregion Constructors

        #region Methods

        private static CProcessType CreateInternal(long lId, string strName, string strDescription)
        {
            return new CProcessType(lId, strName, strDescription);
        }

        public static CProcessType Create(long? lId, string strName, string strDescription, object[] aobjParams)
        {
            CProcessType oProcessType = ms_oEntryHelper.CreateWithUniqueName(EUniquness.IdAndName, lId, strName, strDescription,
                                                        CreateInternal);
            oProcessType.m_strGroup = (string)aobjParams[0];
            oProcessType.m_operationStress = (EOperation)aobjParams[1];
            oProcessType.m_operationCondition = (EOperation)aobjParams[2];
            return oProcessType;
        }

        public static CProcessType GetByName(string strName)
        {
            return ms_oEntryHelper.GetByName(strName);
        }

        public static CProcessType GetById(long lId)
        {
            return ms_oEntryHelper.GetById(lId);
        }

        public List<string> GetNameList()
        {
            return ms_oEntryHelper.GetNameList();
        }

        public bool AddElementType(IElementType iElementType)
        {
            if (!m_dictiEelementTypeByName.ContainsKey(iElementType.Name))
            {
                m_dictiEelementTypeByName.Add(iElementType.Name, iElementType);
                return true;
            }
            return false;
        }

        public bool AddElementJunctionType(IElementJunctionType iElementJunctionType)
        {
            if (!m_dictiEelementJunctionTypeByName.ContainsKey(iElementJunctionType.Name))
            {
                m_dictiEelementJunctionTypeByName.Add(iElementJunctionType.Name, iElementJunctionType);
                return true;
            }
            return false;
        }

        public bool AddInitialStressType(IStressType iStressType)
        {
            if (!m_dictiStressInitialByName.ContainsKey(iStressType.Name))
            {
                m_dictiStressInitialByName.Add(iStressType.Name, iStressType);
                return true;
            }
            return false;
        }

        public bool AddInitialConditionType(IConditionType iConditionType)
        {
            if (!m_dictiConditionInitialByName.ContainsKey(iConditionType.Name))
            {
                m_dictiConditionInitialByName.Add(iConditionType.Name, iConditionType);
                return true;
            }
            return false;
        }

        public bool AddResultingStressType(IStressType iStressType, int nMagnitude)
        {
            CEntryWithNumber<IStressType> iEntryNumStressType = new CEntryWithNumber<IStressType>(iStressType, nMagnitude);
            if (!m_dictstressResulting.ContainsKey(iStressType))
            {
                m_dictstressResulting.Add(iStressType, iEntryNumStressType);
                m_dictiStressResultingByName.Add(iStressType.Name, iEntryNumStressType);
                return true;
            }
            else
            {
                if (nMagnitude != m_dictstressResulting[iStressType].Number)
                {
                    m_dictstressResulting[iStressType] = iEntryNumStressType;
                    m_dictiStressResultingByName[iStressType.Name] = iEntryNumStressType;
                    return true;
                }
            }
            return false;
        }

        public bool AddResultingConditionType(IConditionType iConditionType, int nMagnitude)
        {
            CEntryWithNumber<IConditionType> iEntryNumConditionType = new CEntryWithNumber<IConditionType>(iConditionType, nMagnitude);
            if (!m_dictconditionResulting.ContainsKey(iConditionType))
            {
                m_dictconditionResulting.Add(iConditionType, iEntryNumConditionType);
                m_dictiConditionResultingByName.Add(iConditionType.Name, iEntryNumConditionType);
                return true;
            }
            else
            {
                if (nMagnitude != m_dictconditionResulting[iConditionType].Number)
                {
                    m_dictconditionResulting[iConditionType] = iEntryNumConditionType;
                    m_dictiConditionResultingByName[iConditionType.Name] = iEntryNumConditionType;
                    return true;
                }
            }
            return false;
        }

        public bool AddCausedFailureModeType(IFailureModeType iFailureModeType, int nProbability)
        {
            CEntryWithNumber<IFailureModeType> iEntryNumFailureModeType = new CEntryWithNumber<IFailureModeType>(iFailureModeType, nProbability);
            if (!m_dictfailuremodetypeCaused.ContainsKey(iFailureModeType))
            {
                m_dictfailuremodetypeCaused.Add(iFailureModeType, iEntryNumFailureModeType);
                m_dictiFailureModeTypeCausedByName.Add(iFailureModeType.Name, iEntryNumFailureModeType);
                return true;
            }
            else
            {
                if (nProbability != m_dictfailuremodetypeCaused[iFailureModeType].Number)
                {
                    m_dictfailuremodetypeCaused[iFailureModeType] = iEntryNumFailureModeType;
                    m_dictiFailureModeTypeCausedByName[iFailureModeType.Name] = iEntryNumFailureModeType;
                    return true;
                }
            }
            return false;
        }

        public bool RemoveElementType(string iElementTypeName)
        {
            if (m_dictiEelementTypeByName.ContainsKey(iElementTypeName))
            {
                m_dictiEelementTypeByName.Remove(iElementTypeName);
                return true;
            }
            return false;
        }

        public bool RemoveElementJunctionType(string iElementJunctionTypeName)
        {
            if (m_dictiEelementJunctionTypeByName.ContainsKey(iElementJunctionTypeName))
            {
                m_dictiEelementJunctionTypeByName.Remove(iElementJunctionTypeName);
                return true;
            }
            return false;
        }

        public bool RemoveInitialStressType(string iStressTypeName)
        {
            if (m_dictiStressInitialByName.ContainsKey(iStressTypeName))
            {
                m_dictiStressInitialByName.Remove(iStressTypeName);
                return true;
            }
            return false;
        }

        public bool RemoveInitialConditionType(string iConditionTypeName)
        {
            if (m_dictiConditionInitialByName.ContainsKey(iConditionTypeName))
            {
                m_dictiConditionInitialByName.Remove(iConditionTypeName);
                return true;
            }
            return false;
        }

        public bool RemoveResultingStressType(string iStressTypeName)
        {
            if (m_dictiStressResultingByName.ContainsKey(iStressTypeName))
            {
                IEntryWithNumber<IStressType> stressToBeRemoved = m_dictiStressResultingByName[iStressTypeName];
                m_dictiStressResultingByName.Remove(iStressTypeName);
                m_dictstressResulting.Remove(stressToBeRemoved.Entry);
                return true;
            }
            return false;
        }

        public bool RemoveResultingConditionType(string iConditionTypeName)
        {
            if (m_dictiConditionResultingByName.ContainsKey(iConditionTypeName))
            {
                IEntryWithNumber<IConditionType> conditionToBeRemoved = m_dictiConditionResultingByName[iConditionTypeName];
                m_dictiConditionResultingByName.Remove(iConditionTypeName);
                m_dictconditionResulting.Remove(conditionToBeRemoved.Entry);
            }
            return false;
        }

        public bool RemoveCausedFailureModeType(string iFailureModeTypeName)
        {
            if (m_dictiFailureModeTypeCausedByName.ContainsKey(iFailureModeTypeName))
            {
                IEntryWithNumber<IFailureModeType> failuremodeToBeRemoved = m_dictiFailureModeTypeCausedByName[iFailureModeTypeName];
                m_dictiFailureModeTypeCausedByName.Remove(iFailureModeTypeName);
                m_dictfailuremodetypeCaused.Remove(failuremodeToBeRemoved.Entry);
                return true;
            }
            return false;
        }

        //public void EditMagResultingStressType(string iStressTypeName, int nMagnitude)
        //{
        //    if (m_dictiStressResultingByName.ContainsKey(iStressTypeName))
        //    {
        //        IStressType stressToBeEdited = m_dictiStressResultingByName[iStressTypeName].Entry;
        //        m_dictiStressResultingByName[iStressTypeName] = new CEntryWithNumber<IStressType>(stressToBeEdited, nMagnitude);
        //    }
        //}

        //public void EditResultingConditionType(IConditionType iConditionTypeName, int nMagnitude)
        //{
        //    if (m_dictiConditionResultingByName.ContainsKey(iConditionTypeName))
        //    {
        //        IConditionType conditionToBeEdited = m_dictiConditionResultingByName[iConditionTypeName].Entry;
        //        m_dictiConditionResultingByName[iConditionTypeName] = new CEntryWithNumber<IConditionType>(conditionToBeEdited, nMagnitude);
        //    }
        //}

        //public void EditCausedFailureModeType(string iFailureModeTypeName, int nProbability)
        //{
        //    if (m_dictiFailureModeTypeCausedByName.ContainsKey(iFailureModeTypeName))
        //    {
        //        IFailureModeType failuremodeToBeEdited = m_dictiFailureModeTypeCausedByName[iFailureModeTypeName].Entry;
        //        m_dictiFailureModeTypeCausedByName[iFailureModeTypeName] = new CEntryWithNumber<IFailureModeType>(failuremodeToBeEdited, nProbability);
        //    }
        //}

        public static EOperation OperationFromString(string strOperation)
        {
            switch (strOperation.Trim().ToUpper())
            {
                case "AND":
                    return EOperation.And;
                case "OR":
                    return EOperation.Or;
                default:
                    return EOperation.Nop;
            }
        }

        public static string OperationToString(EOperation eOperation)
        {
            switch (eOperation)
            {
                case EOperation.And:
                    return "AND";
                case EOperation.Or:
                    return "OR";
                default:
                    return "";
            }
        }

        public List<IEntryWithNumber<IFailureModeType>> GetRelevantCausedFailuremodeTypeWithProbability(IEnumerable<IFailureModeType> ienumfailuremodetypeFromElement)
        {
            return ienumfailuremodetypeFromElement.Where(iFailureModeType => m_dictfailuremodetypeCaused.ContainsKey(iFailureModeType))
                                             .Select(iFailureModeType => m_dictfailuremodetypeCaused[iFailureModeType]).ToList();
            //return (from iFailureModeType in ienumfailuremodetypeFromElement where m_dictfailuremodetypeCaused.ContainsKey(iFailureModeType) select m_dictfailuremodetypeCaused[iFailureModeType]).ToList();
        }

        /// <summary>
        /// Returns failure mode types along with severity and probability
        /// </summary>
        /// <param name="ienumFailureModeWithSeverityFromElement"></param>
        /// <returns></returns>
        public Dictionary<IFailureModeType, Tuple<int, int>> GetRelevantCausedFailuremodeTypeWithProbabilityAndSeverity(
            IEnumerable<IEntryWithNumber<IFailureModeType>> ienumFailureModeWithSeverityFromElement)
        {
             return ienumFailureModeWithSeverityFromElement
                     .Where(FailureModeWithSeverity => m_dictfailuremodetypeCaused.ContainsKey(FailureModeWithSeverity.Entry))
                     .ToDictionary(FailureModeWithSeverity=>FailureModeWithSeverity.Entry,
                                   FailureModeWithSeverity
                                       =>new Tuple<int, int>(FailureModeWithSeverity.Number,m_dictfailuremodetypeCaused[FailureModeWithSeverity.Entry].Number));

        }

        public bool IsElementTypeCause(string strName)
        {
            return m_dictiEelementTypeByName.ContainsKey(strName);
        }

        public bool IsElementJunctionTypeCause(string strName)
        {
            return m_dictiEelementJunctionTypeByName.ContainsKey(strName);
        }

        public bool IsStressInitialCause(string strName)
        {
            return m_dictiStressInitialByName.ContainsKey(strName);
        }

        public bool IsConditionInitialCause(string strName)
        {
            return m_dictiConditionInitialByName.ContainsKey(strName);
        }

        public int? IsStressResulting(string strName)
        {
            IEntryWithNumber<IStressType> iEntryWithNumber;
            m_dictiStressResultingByName.TryGetValue(strName, out iEntryWithNumber);
            if (iEntryWithNumber == null) return null;
            return iEntryWithNumber.Number;
        }

        public int? IsConditionResulting(string strName)
        {
            IEntryWithNumber<IConditionType> iEntryWithNumber;
            m_dictiConditionResultingByName.TryGetValue(strName, out iEntryWithNumber);
            if (iEntryWithNumber == null) return null;
            return iEntryWithNumber.Number;
        }

        public int? IsFailureModeResulting(string strName)
        {
            IEntryWithNumber<IFailureModeType> iEntryWithNumber;
            m_dictiFailureModeTypeCausedByName.TryGetValue(strName, out iEntryWithNumber);
            if (iEntryWithNumber == null) return null;
            return iEntryWithNumber.Number;
        }
        #endregion Methods

        #region Properties

        public EOperation StressOperation
        {
            get { return m_operationStress; }
            set { m_operationStress = value; }
        }

        public EOperation ConditionOperation
        {
            get { return m_operationCondition; }
            set { m_operationCondition = value; }
        }

        string IProcessType.StressOperation
        {
            get { return OperationToString(StressOperation); }
        }

        string IProcessType.ConditionOperation
        {
            get { return OperationToString(ConditionOperation); }
        }

        public List<IEntryWithNumber<IStressType>> ResultingStressTypeWithMagnitudeList
        {
            get { return m_dictstressResulting.Values.ToList(); }
        }

        public List<IEntryWithNumber<IConditionType>> ResultingConditionTypeWithMagnitudeList
        {
            get { return m_dictconditionResulting.Values.ToList(); }
        }

        public List<IEntryWithNumber<IFailureModeType>> CausedFailuremodeTypeWithProbabilityList
        {
            get { return m_dictfailuremodetypeCaused.Values.ToList(); }
        }

        public int ProcessInherentness
        {
            get { return ((int) m_operationStress + (int) m_operationCondition)/2; }
        }
        #endregion Properties

        #region MemberVariables
        private static readonly CEntryHelper<CProcessType> ms_oEntryHelper = new CEntryHelper<CProcessType>();
        private readonly Dictionary<string, IElementType> m_dictiEelementTypeByName = new Dictionary<string, IElementType>();
        private readonly Dictionary<string, IElementJunctionType> m_dictiEelementJunctionTypeByName = new Dictionary<string, IElementJunctionType>();
        private readonly Dictionary<string, IStressType> m_dictiStressInitialByName = new Dictionary<string, IStressType>();
        private readonly Dictionary<string, IConditionType> m_dictiConditionInitialByName = new Dictionary<string, IConditionType>();
        private readonly Dictionary<string, IEntryWithNumber<IStressType>> m_dictiStressResultingByName = new Dictionary<string, IEntryWithNumber<IStressType>>();
        private readonly Dictionary<string, IEntryWithNumber<IConditionType>> m_dictiConditionResultingByName = new Dictionary<string, IEntryWithNumber<IConditionType>>();
        private readonly Dictionary<string, IEntryWithNumber<IFailureModeType>> m_dictiFailureModeTypeCausedByName = new Dictionary<string, IEntryWithNumber<IFailureModeType>>();
        private readonly Dictionary<IStressType, IEntryWithNumber<IStressType>> m_dictstressResulting = new Dictionary<IStressType, IEntryWithNumber<IStressType>>();
        private readonly Dictionary<IConditionType, IEntryWithNumber<IConditionType>> m_dictconditionResulting = new Dictionary<IConditionType, IEntryWithNumber<IConditionType>>();
        private readonly Dictionary<IFailureModeType, IEntryWithNumber<IFailureModeType>> m_dictfailuremodetypeCaused = new Dictionary<IFailureModeType, IEntryWithNumber<IFailureModeType>>();

        private string m_strGroup;
        private EOperation m_operationStress;
        private EOperation m_operationCondition;
        #endregion MemberVariables

        #region InnerClasses


        #endregion InnerClasses
    }
}