using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMEA.Common;

namespace FMEA.Configuration
{
    public class CElementTypeBase : CEntry, IElementTypeBase
    {
        #region Constructors

        protected CElementTypeBase(long lId, string strName, string strDescription)
            : base(lId, strName, strDescription)
        {
        }
        #endregion //Constructor

        #region Methods
        private static CElementTypeBase CreateInternal(long lId, string strName, string strDescription)
        {
            return new CElementTypeBase(lId, strName, strDescription);
        }

        public static CElementTypeBase Create(long? lId, string strName, string strDescription)
        {
            return ms_oEntryHelper.CreateWithUniqueName(EUniquness.IdAndName, lId,  strName, strDescription, CreateInternal);
        }

        public static CElementTypeBase GetByName(string strName)
        {
            return ms_oEntryHelper.GetByName(strName);
        }

        public static CElementTypeBase GetById(long lId)
        {
            return ms_oEntryHelper.GetById(lId);
        }

        public List<string> GetNameList()
        {
            return ms_oEntryHelper.GetNameList();
        }

        public void AddFailureModeType(IFailureModeType iFailureModeType)
        {
            if (!m_dictstriFailureModeType.Keys.Contains(iFailureModeType.Name))
            {
                m_dictstriFailureModeType.Add(iFailureModeType.Name, iFailureModeType);
            }
        }

        public void AddFailureModeType(List<IFailureModeType> lstiFailureModeType)
        {
            foreach (IFailureModeType iFailureModeType in lstiFailureModeType)
            {
                AddFailureModeType(iFailureModeType);
            }
        }
        #endregion //Methods

        #region Properties

        public Dictionary<string, IFailureModeType> FailureModeTypeInterfacesByNameDictionary
        {
            get { return m_dictstriFailureModeType; }
        }

        public bool IsJunctionType
        {
            get { return this is CElementJunctionType; }
        }

        #endregion //Properties

        #region MemberVariables
        protected static CEntryHelper<CElementTypeBase> ms_oEntryHelper = new CEntryHelper<CElementTypeBase>();
        protected Dictionary<string, IFailureModeType> m_dictstriFailureModeType = new Dictionary<string, IFailureModeType>();

        #endregion //MemberVariables

        #region InnerClasses

        #endregion //InnerClasses
    }
}
