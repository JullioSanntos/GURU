using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMEA.Common;

namespace FMEA.Configuration
{
   public class Configuration : IConfiguration
    {
        #region Constructors

        public Configuration()
        {
            //Init(lstoElementType, lstoElementJunctionType, lstoStressType, lstoConditionType, oProcessMapper);
        }

        //public Configuration(List<CElementType> lstoElementType, List<CElementJunctionType> lstoElementJunctionType, List<CStressType> lstoStressType, List<CConditionType> lstoConditionType, CProcessMapper oProcessMapper)
        //{
        //    Init(lstoElementType, lstoElementJunctionType, lstoStressType, lstoConditionType, oProcessMapper);
        //}

        #endregion Constructors

        #region Methods

        //public void Init(List<CElementType> lstoElementType, List<CElementJunctionType> lstoElementJunctionType, List<CStressType> lstoStressType, List<CConditionType> lstoConditionType, CProcessMapper oProcessMapper)
        //{
        //    m_lstoElementType = lstoElementType;
        //    m_lstoElementJunctionType = lstoElementJunctionType;
        //    m_lstoStressType = lstoStressType;
        //    m_lstoConditionType = lstoConditionType;
        //    m_oProcessMapper = oProcessMapper;

        //    m_lstoElementTypeBase = new List<CElementTypeBase>(lstoElementType);
        //    m_lstoElementTypeBase.AddRange(lstoElementJunctionType);
        //    m_lststresstypeOperational =
        //        m_lstoStressType.Where(oStress => oStress.StressCategory == EStressCategory.Operational).ToList();
        //    m_lststresstypeEnvironmental =
        //        m_lstoStressType.Where(oStress => oStress.StressCategory == EStressCategory.Environmental).ToList();
        //}

        private TEntry CreateEntry<TEntry>(long? lId, string strName, string strDescription,
                                           object[] aobjParam,
                                           Func<long?, string, string, object[], TEntry> funcCreateEntry,
                                           Dictionary<string, TEntry> dictstrtentryAllByName) where TEntry : CEntry
        {
            if (lId == null)
            {
                while (m_dictloentryAllEntriesById.Keys.Contains(m_lCurrentId))
                {
                    m_lCurrentId++;
                }
                lId = m_lCurrentId;
            }
            if (m_dictloentryAllEntriesById.Keys.Contains((long)lId) ||
                dictstrtentryAllByName.Keys.Contains(strName))
            {
                return null;
            }
            TEntry tEntry = funcCreateEntry(lId, strName, strDescription, aobjParam);
            m_dictloentryAllEntriesById.Add((long)lId, tEntry);
            dictstrtentryAllByName.Add(strName, tEntry);
            return tEntry;
        }

        private TEntry CreateEntry<TEntry>(long? lId, string strName, string strDescription,
                                      Func<long?, string, string, TEntry> funcCreateEntry,
                                      Dictionary<string, TEntry> dictstrtentryAllByName) where TEntry : CEntry
        {
            if (lId == null)
            {
                while (m_dictloentryAllEntriesById.Keys.Contains(m_lCurrentId))
                {
                    m_lCurrentId++;
                }
                lId = m_lCurrentId;
            }
            if (m_dictloentryAllEntriesById.Keys.Contains((long)lId) ||
                dictstrtentryAllByName.Keys.Contains(strName))
            {
                return null;
            }
            TEntry tEntry = funcCreateEntry(lId, strName, strDescription);
            m_dictloentryAllEntriesById.Add((long)lId, tEntry);
            dictstrtentryAllByName.Add(strName, tEntry);
            return tEntry;
        }

        //private TEntry CreateEntry<TEntry>(long? lId, string strName, string strDescription,
        //                              bool bInitialOrAppliesToJunctions, bool bResultingOrAppliesToElements,
        //                              Func<long?, string, string, bool, bool, TEntry> funcCreateEntry,
        //                              Dictionary<string, TEntry> dictstrtentryAllByName) where TEntry : CEntry
        //{
        //    if (lId == null)
        //    {
        //        while (m_dictloentryAllEntriesById.Keys.Contains(m_lCurrentId))
        //        {
        //            m_lCurrentId++;
        //        }
        //        lId = m_lCurrentId;
        //    }
        //    if (m_dictloentryAllEntriesById.Keys.Contains((long)lId) ||
        //        dictstrtentryAllByName.Keys.Contains(strName))
        //    {
        //        return null;
        //    }
        //    TEntry tEntry = funcCreateEntry(lId, strName, strDescription, bInitialOrAppliesToJunctions, bResultingOrAppliesToElements);
        //    m_dictloentryAllEntriesById.Add((long)lId, tEntry);
        //    dictstrtentryAllByName.Add(strName, tEntry);
        //    return tEntry;
        //}

        //private TEntry CreateEntry<TEntry>(long? lId, string strName, string strDescription,
        //                      bool bInitial, bool bResulting,
        //                      bool nProbabilityPropagateToElement, bool nProbabilityPropagateToElementJunction,
        //                      Func<long?, string, string, bool, bool, bool, bool, TEntry> funcCreateEntry,
        //                      Dictionary<string, TEntry> dictstrtentryAllByName) where TEntry : CEntry
        //{
        //    if (lId == null)
        //    {
        //        while (m_dictloentryAllEntriesById.Keys.Contains(m_lCurrentId))
        //        {
        //            m_lCurrentId++;
        //        }
        //        lId = m_lCurrentId;
        //    }
        //    if (m_dictloentryAllEntriesById.Keys.Contains((long)lId) ||
        //        dictstrtentryAllByName.Keys.Contains(strName))
        //    {
        //        return null;
        //    }
        //    TEntry tEntry = funcCreateEntry(lId, strName, strDescription, bInitial, bResulting, nProbabilityPropagateToElement, nProbabilityPropagateToElementJunction);
        //    m_dictloentryAllEntriesById.Add((long)lId, tEntry);
        //    dictstrtentryAllByName.Add(strName, tEntry);
        //    return tEntry;
        //}

        public CElementType CreateElementType(long? lId, string strName, string strDescription)
        {
            return CreateEntry<CElementType>(lId, strName, strDescription, CElementType.Create, m_dictloElementTypeByName);
        }

        public CElementJunctionType AddElementJunctionType(long? lId, string strName, string strDescription)
        {
            return CreateEntry<CElementJunctionType>(lId, strName, strDescription, CElementJunctionType.Create,
                                                  m_dictloElementJunctionTypeByName);
        }

        public CFailureModeType CreateFailureModeType(long? lId, string strName, string strDescription,
                                                      bool bAppliesToJunctions, bool bAppliesToElements,
                                                      List<IEntryWithNumber<IConditionType>> lstAutoConditionTypeWithMagnitudes = null)
        {
            if (lstAutoConditionTypeWithMagnitudes == null || lstAutoConditionTypeWithMagnitudes.Count==0)
            {
                return CreateEntry<CFailureModeType>(lId, strName, strDescription, new object[] { bAppliesToJunctions, bAppliesToElements },
                                                     CFailureModeType.Create, m_dictloFailureModeTypeByName);
            }
            return CreateEntry<CFailureModeType>(lId, strName, strDescription,
                                                 new object[] {bAppliesToJunctions, bAppliesToElements, lstAutoConditionTypeWithMagnitudes},
                                                 CFailureModeType.Create, m_dictloFailureModeTypeByName);
        }

        public CStressType CreateStressType(long? lId, string strName, string strDescription,
                                            bool bInitial, bool bResulting,
                                            int nProbabilityPropagateToElement, int nProbabilityPropagateToElementJunction)
        {
            return CreateEntry<CStressType>(lId, strName, strDescription,
                                               new object[] { bInitial, bResulting,
                                                              nProbabilityPropagateToElement, nProbabilityPropagateToElementJunction},
                                               CStressType.Create, m_dictloStressTypeByName);
            //CStressType oStressType = null;
            //switch (eStressCategory)
            //{
            //        case EStressCategory.Operational:
            //        oStressType = CreateEntry<CStressType>(lId, strName, strDescription, bInitial, bResulting,
            //                                            CStressType.CreateOperational, m_dictloStressTypeByName);
            //        break;
            //        case EStressCategory.Environmental:
            //        oStressType = CreateEntry<CStressType>(lId, strName, strDescription, bInitial, bResulting,
            //                                            CStressType.CreateEnvironmental, m_dictloStressTypeByName);
            //        break;
            //}
            //return oStressType;
        }

       public CConditionType CreateConditionType(long? lId, string strName, string strDescription,
                                                 bool bInitial, bool bResulting,
                                                 int nProbabilityPropagateToElement, int nProbabilityPropagateToElementJunction,
                                                 bool bForInterfaceOnly, bool bOnlyOneSelectable)
        {
            return CreateEntry<CConditionType>(lId, strName, strDescription,
                                               new object[] { bInitial, bResulting, nProbabilityPropagateToElement, nProbabilityPropagateToElementJunction, bForInterfaceOnly, bOnlyOneSelectable},
                                               CConditionType.Create, m_dictloConditionTypeByName);
        }

        public CProcessType CreateProcessType(long? lId, string strGroup, string strName, string strDescription,
                                              EOperation operationStress,
                                              EOperation operationCondition)
        //bool bInitial = true, bool bResulting = false)
        {
            return CreateEntry<CProcessType>(lId, strName, strDescription,
                                             new object[] {strGroup, operationStress, operationCondition }, CProcessType.Create,
                                             m_dictloProcessTypeByName);
        }

        public void AddProcessToMapper(CProcessType oProcessType, IElementTypeBase[] aiElementTypeBase,
                                       IStressType[] aiStressTypeInitial, IConditionType[] aiConditionTypeInitial)
        {
            m_oProcessMapper.AddProcess(oProcessType, aiElementTypeBase, aiStressTypeInitial, aiConditionTypeInitial);
        }

        private void GenerateIndexes()
        {
        }

        private bool EntryNameExist<TEntry>(string strName, Dictionary<string, TEntry> dictstrtentryAllByName) where TEntry : CEntry
        {
            //VBB_TODO: Remove ToLower
            strName = strName.ToLower();
            return dictstrtentryAllByName != null && dictstrtentryAllByName.ContainsKey(strName);
        }

        public bool ElementTypeNameExist(string strName)
        {
            return EntryNameExist(strName, m_dictloElementTypeByName);
        }

        public bool ElementJunctionTypeNameExist(string strName)
        {
            return EntryNameExist(strName, m_dictloElementJunctionTypeByName);
        }

        public bool StressTypeNameExist(string strName)
        {
            return EntryNameExist(strName, m_dictloStressTypeByName);
        }

        public bool ConditionTypeNameExist(string strName)
        {
            return EntryNameExist(strName, m_dictloConditionTypeByName);
        }

        public bool FailureModeTypeNameExist(string strName)
        {
            return EntryNameExist(strName, m_dictloFailureModeTypeByName);
        }

        public bool ProcessTypeNameExist(string strName)
        {
            return EntryNameExist(strName, m_dictloProcessTypeByName);
        }

        private TEntry GetEntryByName<TEntry>(string strName, Dictionary<string, TEntry> dictstrtentryAllByName, bool bThrowException = true) where TEntry : CEntry
        {
            //VBB_TODO: Remove ToLower
            strName = strName.ToLower();
            if (dictstrtentryAllByName == null || !dictstrtentryAllByName.ContainsKey(strName))
            {
                if (bThrowException)
                {
                    throw new Exception(string.Format("Can't find configuration entry of type {0} with name {1}", typeof (TEntry), strName));
                }
                return null;
            }
            else
            {
                return dictstrtentryAllByName[strName];
            }
        }

        public IElementType GetElementTypeByName(string strName, bool bThrowException = true)
        {
            return GetEntryByName(strName, m_dictloElementTypeByName, bThrowException);
        }

        public IElementJunctionType GetElementJunctionTypeByName(string strName, bool bThrowException = true)
        {
            return GetEntryByName(strName, m_dictloElementJunctionTypeByName, bThrowException);
        }

        public IElementJunctionType GetElementJunctionTypeByElementTypeNames(string strElementType1Name, string strElemenType2Name, bool bThrowException = true)
        {
            return GetEntryByName(CElementJunctionType.CreateElementJunctionTypeName(strElementType1Name, strElemenType2Name),
                                  m_dictloElementJunctionTypeByName, bThrowException);
        }

        public IStressType GetStressTypeByName(string strName, bool bThrowException = true)
        {
            return GetEntryByName(strName, m_dictloStressTypeByName, bThrowException);
        }

        public IConditionType GetConditionTypeByName(string strName, bool bThrowException = true)
        {
            return GetEntryByName(strName, m_dictloConditionTypeByName, bThrowException);
        }

        public IFailureModeType GetFailureModeTypeByName(string strName, bool bThrowException = true)
        {
            return GetEntryByName(strName, m_dictloFailureModeTypeByName, bThrowException);
        }

        public IProcessType GetProcessTypeByName(string strName, bool bThrowException = true)
        {
            return GetEntryByName(strName, m_dictloProcessTypeByName, bThrowException);
        }

        public List<IResultingProcess> GetResultingProcesses(IElementTypeBase iElementTypeBase, IEnumerable<IStressType> aiStressTypeInitial,
                                               IEnumerable<IConditionType> aiConditionTypeInitial)
        {
            return m_oProcessMapper.GetProcesses(iElementTypeBase, aiStressTypeInitial, aiConditionTypeInitial);
        }

        /// <summary>
        /// Given the version that was used to create a serialized system this function
        //  checks if the current configuration being can be used to deserialize the system.
        /// </summary>
        /// <param name="strConfigurationVersion"></param>
        /// <returns></returns>
        public bool CheckConfigurationCompatible(string strConfigurationVersion)
        {
            //VBB_TODO: Implement
            return true;
        }
        #endregion Methods

        #region Properties
        public string JunctionSeparator
        {
            get { return CElementJunctionType.strELEMENT_JUNCTION_TYPE_SEPARATOR; }
        }

        public string Version
        {
            get { return m_strVersion; }
        }

       public List<IConditionType> InitialConditionTypes
       {
           get
           {
               if (m_lstconditiontypeInitial==null)
               {
                   m_lstconditiontypeInitial = new List<IConditionType>();
                   foreach (IConditionType iConditionType in m_dictloConditionTypeByName.Values)
                   {
                       if (iConditionType.IsInitial)
                       {
                           m_lstconditiontypeInitial.Add(iConditionType);
                       }
                   }
               }
               return m_lstconditiontypeInitial;
           }
       }

       public List<IStressType> InitialStresstypes
       {
           get
           {
               if (m_lststresstypeInitial == null)
               {
                   m_lststresstypeInitial = new List<IStressType>();
                   foreach (IStressType iStressType in m_dictloStressTypeByName.Values)
                   {
                       if (iStressType.IsInitial)
                       {
                           m_lststresstypeInitial.Add(iStressType);
                       }
                   }
               }
               return m_lststresstypeInitial;
           }
       }

       public List<IConditionType> ResultingConditionTypes
       {
           get
           {
               if (m_lstconditiontypeResulting == null)
               {
                   m_lstconditiontypeResulting = new List<IConditionType>();
                   foreach (IConditionType iConditionType in m_dictloConditionTypeByName.Values)
                   {
                       if (iConditionType.IsResulting)
                       {
                           m_lstconditiontypeResulting.Add(iConditionType);
                       }
                   }
               }
               return m_lstconditiontypeResulting;
           }
       }

       public List<IStressType> ResultingStresstypes
       {
           get
           {
               if (m_lststresstypeResulting == null)
               {
                   m_lststresstypeResulting = new List<IStressType>();
                   foreach (IStressType iStressType in m_dictloStressTypeByName.Values)
                   {
                       if (iStressType.IsResulting)
                       {
                           m_lststresstypeResulting.Add(iStressType);
                       }
                   }
               }
               return m_lststresstypeResulting;
           }
       }


       public List<IFailureModeType> FailureModeTypes
       {
           get
           {
               if (m_lstiFailureModeTypes == null)
               {
                   m_lstiFailureModeTypes = new List<IFailureModeType>();
                   foreach (IFailureModeType iFailureModeType in m_dictloFailureModeTypeByName.Values)
                   {
                       m_lstiFailureModeTypes.Add(iFailureModeType);
                   }
               }
               return m_lstiFailureModeTypes;
           }
       }

       public List<IElementType> ElementTypes
       {
           get
           {
               if (m_lstiElementType == null)
               {
                   m_lstiElementType = new List<IElementType>();
                   foreach (IElementType iElementType in m_dictloElementTypeByName.Values)
                   {
                       m_lstiElementType.Add(iElementType);
                   }
               }
               return m_lstiElementType;
           }
       }

       public List<IElementJunctionType> ElementJunctionTypes
       {
           get
           {
               if (m_lstiElementJunctionType == null)
               {
                   m_lstiElementJunctionType = new List<IElementJunctionType>();
                   foreach (IElementJunctionType iElementJunctionType in m_dictloElementJunctionTypeByName.Values)
                   {
                       m_lstiElementJunctionType.Add(iElementJunctionType);
                   }
               }
               return m_lstiElementJunctionType;
           }
       }

       public List<IProcessType> ProcessTypes
       {
           get
           {
               if (m_lstiProcessType == null)
               {
                   m_lstiProcessType = new List<IProcessType>();
                   foreach (IProcessType iProcessType in m_dictloProcessTypeByName.Values)
                   {
                       m_lstiProcessType.Add(iProcessType);
                   }
               }
               return m_lstiProcessType;
           }
       }


        #endregion Properties

        #region MemberVariables

        //VBB_TODO: Implement
        private string m_strVersion = "0.0.0";

        /// <summary>
        /// Used to assign an unique id to an entry when none is specified
        /// </summary>
        private long m_lCurrentId = 1;

        private CProcessMapper m_oProcessMapper = new CProcessMapper();

        /// <summary>
        /// Maintained during adds to make sure all entries have unique Id
        /// </summary>
        private Dictionary<long, CEntry> m_dictloentryAllEntriesById = new Dictionary<long, CEntry>();

        /// <summary>
        /// Maintained during adds to make sure all ElementType have unique Name
        /// </summary>
        private Dictionary<string, CElementType> m_dictloElementTypeByName = new Dictionary<string, CElementType>();

        /// <summary>
        /// Maintained during adds to make sure all ElementJunctionType have unique Name
        /// </summary>
        private Dictionary<string, CElementJunctionType> m_dictloElementJunctionTypeByName = new Dictionary<string, CElementJunctionType>();

        /// <summary>
        /// Maintained during adds to make sure all StressType have unique Name
        /// </summary>
        private Dictionary<string, CStressType> m_dictloStressTypeByName = new Dictionary<string, CStressType>();

        /// <summary>
        /// Maintained during adds to make sure all ConditionType have unique Name
        /// </summary>
        private Dictionary<string, CConditionType> m_dictloConditionTypeByName = new Dictionary<string, CConditionType>();

        /// <summary>
        /// Maintained during adds to make sure all FailureModeType have unique Name
        /// </summary>
        private Dictionary<string, CFailureModeType> m_dictloFailureModeTypeByName = new Dictionary<string, CFailureModeType>();

        /// <summary>
        /// Maintained during adds to make sure all ProcessType have unique Name
        /// </summary>
        private Dictionary<string, CProcessType> m_dictloProcessTypeByName = new Dictionary<string, CProcessType>();

        //private Dictionary<long, CElementType> m_dictloElementTypeById = new Dictionary<long, CElementType>();

        //private Dictionary<long, CElementJunctionType> m_dictloElementJunctionTypeById = new Dictionary<long, CElementJunctionType>();

        //private Dictionary<string, CElementTypeBase> m_dictloElementTypeBaseByName = new Dictionary<string, CElementTypeBase>();
        //private Dictionary<long, CElementTypeBase> m_dictloElemenTypeBaseById = new Dictionary<long, CElementTypeBase>();

        //private Dictionary<long, CStressType> m_dictloStressTypeInitialById = new Dictionary<long, CStressType>();
        //private Dictionary<string, CStressType> m_dictloStressTypeInitialByName = new Dictionary<string, CStressType>();
        //private Dictionary<long, CStressType> m_dictloStressTypeResultingById = new Dictionary<long, CStressType>();
        //private Dictionary<string, CStressType> m_dictloStressTypeResultingByName = new Dictionary<string, CStressType>();

        //private Dictionary<long, CConditionType> m_dictloConditionTypeInitialById = new Dictionary<long, CConditionType>();
        //private Dictionary<string, CConditionType> m_dictloConditionTypeInitialByName = new Dictionary<string, CConditionType>();
        //private Dictionary<long, CConditionType> m_dictloConditionTypeResultingById = new Dictionary<long, CConditionType>();
        //private Dictionary<string, CConditionType> m_dictloConditionTypeResultingByName = new Dictionary<string, CConditionType>();

        //private Dictionary<long, CFailureModeType> m_dictloFailureModeTypeById = new Dictionary<long, CFailureModeType>();

        //private List<CElementType> m_lstoElementType;
        //private List<CElementJunctionType> m_lstoElementJunctionType;
        //private List<CStressType> m_lstoStressType;
        //private List<CConditionType> m_lstoConditionType;
        //private CProcessMapper m_oProcessMapper;

        //private List<CElementTypeBase> m_lstoElementTypeBase;
        //private List<CStressType> m_lststresstypeOperational;
        //private List<CStressType> m_lststresstypeEnvironmental;

        private List<IConditionType> m_lstconditiontypeInitial = null;
        private List<IStressType> m_lststresstypeInitial = null;
        private List<IConditionType> m_lstconditiontypeResulting = null;
        private List<IStressType> m_lststresstypeResulting = null;
        private List<IFailureModeType> m_lstiFailureModeTypes = null;
       private List<IElementType> m_lstiElementType = null;
       private List<IElementJunctionType> m_lstiElementJunctionType = null;
       private List<IProcessType> m_lstiProcessType = null;

       #endregion MemberVariables

       #region InnerClasses

       #endregion InnerClasses
    }
}