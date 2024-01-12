using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMEA.Common;
using FMEA.Constants;

namespace FMEA.Configuration
{
    public class CConditionType : CEntry, IConditionType
    {
        #region Constructors

        private CConditionType(long lId, string strName, string strDescription)
            : base(lId, strName, strDescription)
        {
        }

        #endregion Constructors

        #region Methods

        private static CConditionType CreateInternal(long lId, string strName, string strDescription)
        {
            return new CConditionType(lId, strName, strDescription);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="lId"></param>
        /// <param name="strName"></param>
        /// <param name="strDescription"></param>
        /// <param name="bInitial">All condition types are expected to be at least initial. It does not make sense
        /// for a condition type to be only resulting because it would never generate a process and therefore
        /// would have no effect on the physical analysis</param>
        /// <param name="bResulting"></param>
        /// <param name="bAppliesToJunctions">Flags condition which applies to junctions (surface) </param>
        /// <param name="bAppliesToElements">Flags condition which applies to Elements</param>
        /// <returns></returns>
        //public static CConditionType Create(long? lId, string strName, string strDescription,
        //                                     bool bInitial = true, bool bResulting = false,
        //                                     bool bAppliesToJunctions = true, bool bAppliesToElements = true)
        //{
        //    CConditionType oConditionType = ms_oEntryHelper.CreateWithUniqueName(EUniquness.IdAndName, lId, strName,
        //                                                                         strDescription, CreateInternal);
        //    if (oConditionType != null)
        //    {
        //        oConditionType.m_bIsInitial = bInitial;
        //        oConditionType.m_bIsResulting = bResulting;
        //        oConditionType.m_nProbabilityPropagateToElement = bAppliesToJunctions;
        //        oConditionType.m_nProbabilityPropagateToElementJunction = bAppliesToElements;
        //    }
        //    return oConditionType;
        //}

        /// <summary>
        ///
        /// </summary>
        /// <param name="lId"></param>
        /// <param name="strName"></param>
        /// <param name="strDescription"></param>
        /// <param name="aobjParam"> IsInitial, IsResulting, PropagateToElement, PropagateToElementJunction
        /// All condition types are expected to be at least initial. It does not make sense
        /// for a condition type to be only resulting because it would never generate a process and therefore
        /// would have no effect on the physical analysis</param>
        /// <returns></returns>
        public static CConditionType Create(long? lId, string strName, string strDescription,
                                            object[] aobjParam)
        {
            if (aobjParam == null) throw new ArgumentNullException("aobjParam");
            CConditionType oConditionType = ms_oEntryHelper.CreateWithUniqueName(EUniquness.IdAndName, lId, strName,
                                                                                 strDescription, CreateInternal);
            //int nSpacePosition = oConditionType.Name.IndexOf(' ');
            //if (nSpacePosition < 0)
            //{
            //    oConditionType.m_strGroupName = "";
            //    oConditionType.m_strShortName = oConditionType.Name;
            //}
            //else
            //{
            //    oConditionType.m_strGroupName = oConditionType.Name.Split();
            //    oConditionType.m_strShortName = oConditionType.Name;
            //}

            string[] strNameSplit = oConditionType.Name.Split(' ');
            if(strNameSplit.Length<2)
            {
                oConditionType.m_strGroupName = "";
                oConditionType.m_strShortName = oConditionType.Name;
            }
            else
            {
                oConditionType.m_strGroupName = strNameSplit[0];
                oConditionType.m_strShortName = string.Join(" ", strNameSplit.Skip(1));
            }

            if (oConditionType != null)
            {
                oConditionType.m_bIsInitial = (bool)aobjParam[0];
                oConditionType.m_bIsResulting = (bool)aobjParam[1];
                oConditionType.m_nProbabilityPropagateToElement = (int)aobjParam[2];
                oConditionType.m_nProbabilityPropagateToElementJunction = (int)aobjParam[3];
                oConditionType.m_bForInterfaceOnly = (bool)aobjParam[4];
                oConditionType.m_bOnlyOneSelectable = (bool)aobjParam[5];
                //oConditionType.m_nMagnitude = (int)aobjParam[4];
            }
            return oConditionType;
        }

        public static CConditionType GetByName(string strName)
        {
            return ms_oEntryHelper.GetByName(strName);
        }

        public static CConditionType GetById(long lId)
        {
            return ms_oEntryHelper.GetById(lId);
        }

        public List<string> GetNameList()
        {
            return ms_oEntryHelper.GetNameList();
        }

        #endregion Methods

        #region Properties
        public string ShortName
        {
            get { return m_strShortName; }
        }

        public string GroupName
        {
            get { return m_strGroupName; }
        }

        public bool IsInitial
        {
            get { return m_bIsInitial; }
            set { m_bIsInitial = value; }
        }

        public bool IsResulting
        {
            get { return m_bIsResulting; }
            set { m_bIsResulting = value; }
        }

        public bool PropagateToElement
        {
            get { return m_nProbabilityPropagateToElement!=0; }
        }

        public bool PropagateToElementJunction
        {
            get { return m_nProbabilityPropagateToElementJunction!=0; }
        }

        public int ProbabilityPropagateToElement
        {
            get { return m_nProbabilityPropagateToElement; }
        }

        public int ProbabilityPropagateToElementJunction
        {
            get { return m_nProbabilityPropagateToElementJunction; }
        }

        //public int Number
        //{
        //    get { return m_nMagnitude; }
        //}
        #endregion Properties

        #region MemberVariables
        private string m_strShortName;
        private string m_strGroupName;
        private bool m_bOnlyOneSelectable;
        private bool m_bIsInitial = false;
        private bool m_bIsResulting = false;
        private int m_nProbabilityPropagateToElement = CConstants.mc_nNumberDefault;
        private int m_nProbabilityPropagateToElementJunction = 5;
        //private int m_nMagnitude = CConstants.mc_nNumberDefault;

        private static CEntryHelper<CConditionType> ms_oEntryHelper = new CEntryHelper<CConditionType>();
        private bool m_bForInterfaceOnly;
        #endregion MemberVariables

        #region InnerClasses

        #endregion InnerClasses
    }
}