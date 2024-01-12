using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FMEA.Common;

namespace FMEA.Configuration
{
    public class CFailureModeType : CEntry, IFailureModeType
    {
        #region Constructors

        public CFailureModeType(long lId, string strName, string strDescription, bool bAppliesToJunctions, bool bAppliesToElements)
            : base(lId, strName, strDescription)
        {
            if (!IsNameProperlyConstructed(m_strName))
            {
                throw new Exception(string.Format("Trying to create a FailureModeType with an invalid name: \"{0}\"", m_strName));
            }
            m_bAppliesToJunctions = bAppliesToJunctions;
            m_bAppliesToElements = bAppliesToElements;
            MatchCollection oMatchCollection = Regex.Matches(m_strName, @"[^ ]+");
            //IsNameProperlyConstructed guarentees that the size of the collection is 3
            m_strEnergyType = oMatchCollection[0].Value;
            m_strPrimaryFunction = oMatchCollection[1].Value;
            m_strIpound = oMatchCollection[2].Value;
            m_strPrimaryFunctionAndIpound = string.Format("{0} {1}", m_strPrimaryFunction, m_strIpound);
        }

        #endregion Constructors

        #region Methods

        private static CFailureModeType CreateInternal(long lId, string strName, string strDescription)
        {
            return new CFailureModeType(lId, strName, strDescription, true, true);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="lId"></param>
        /// <param name="strName"></param>
        /// <param name="strDescription"></param>
        /// <param name="aobjParam">PropagateToElement, PropagateToElementJunction</param>
        /// <returns></returns>
        public static CFailureModeType Create(long? lId, string strName, string strDescription, object[] aobjParam)
        {
            CFailureModeType oFailureModeType = ms_oEntryHelper.CreateWithUniqueName(EUniquness.IdAndName, lId, strName, strDescription,
                                                        CreateInternal);
            oFailureModeType.m_bAppliesToJunctions = (bool)aobjParam[0];
            oFailureModeType.m_bAppliesToElements = (bool)aobjParam[1];
            if (aobjParam.Length==3)
            {
                oFailureModeType.m_lstAutoConditionWithMagnitude = (List<IEntryWithNumber<IConditionType>>)aobjParam[2];
            }
            return oFailureModeType;
        }

        public static CFailureModeType GetByName(string strName)
        {
            return ms_oEntryHelper.GetByName(strName);
        }

        public static CFailureModeType GetById(long lId)
        {
            return ms_oEntryHelper.GetById(lId);
        }

        public List<string> GetNameList()
        {
            return ms_oEntryHelper.GetNameList();
        }

        public void AddCausingProcessType(CProcessType oProcessType)
        {
            if (!m_lstprocesstypeCausing.Contains(oProcessType))
            {
                m_lstprocesstypeCausing.Add(oProcessType);
            }
        }

        public static bool IsNameProperlyConstructed(string strProposedname)
        {
            string strstrProposednameUpper = strProposedname.ToUpper();
            return (strstrProposednameUpper.EndsWith(astrFailureModeSuffixes[0])
                    || strstrProposednameUpper.EndsWith(astrFailureModeSuffixes[1]))
                   && Regex.Matches(strstrProposednameUpper, @"[^ ]+").Count == 3;
            //return strstrProposednameUpper.StartsWith(astrFailureModePrefixes[0]) ||
            //       strstrProposednameUpper.StartsWith(astrFailureModePrefixes[1]);
        }

        public string EnergyType
        {
            get { return m_strEnergyType; }
        }

        public string PrimaryFunction
        {
            get { return m_strPrimaryFunction; }
        }

        public string Ipound
        {
            get { return m_strIpound; }
        }

        public string PrimaryFunctionAndIpound
        {
            get { return m_strPrimaryFunctionAndIpound; }
        }
        #endregion Methods

        #region Properties

        public bool AppliesToJunctions
        {
            get { return m_bAppliesToJunctions; }
        }

        public bool AppliesToElements
        {
            get { return m_bAppliesToElements; }
        }

        public List<IEntryWithNumber<IConditionType>> AutoConditionWithMagnitudeList
        {
            get { return m_lstAutoConditionWithMagnitude; }
        }

        public bool HasAutoConditionWithMagnitude
        {
            get { return m_lstAutoConditionWithMagnitude != null && m_lstAutoConditionWithMagnitude.Count != 0; }
        }
        #endregion Properties

        #region MemberVariables

        private static readonly string[] astrFailureModePrefixes = new string[] { "TOO MUCH", "LACK OF" };
        private static readonly string[] astrFailureModeSuffixes = new string[] { "HIGH", "LOW" };
        private static CEntryHelper<CFailureModeType> ms_oEntryHelper = new CEntryHelper<CFailureModeType>();
        private List<CProcessType> m_lstprocesstypeCausing = new List<CProcessType>();
        private bool m_bAppliesToJunctions = true;
        private bool m_bAppliesToElements = true;
        private List<IEntryWithNumber<IConditionType>> m_lstAutoConditionWithMagnitude = null;
        private string m_strEnergyType;
        private string m_strPrimaryFunction;
        private string m_strIpound;
        private string m_strPrimaryFunctionAndIpound;
        #endregion MemberVariables

        #region InnerClasses

        #endregion InnerClasses

        #region Implementation of IComparable<in IFailureModeType>

        public int CompareTo(IFailureModeType other)
        {
            return this.Name.CompareTo(other.Name);
        }

        #endregion
    }
}