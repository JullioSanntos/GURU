using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMEA.Common;
using FMEA.Constants;

namespace FMEA.Configuration
{
    public class CStressType : CEntry, IStressType
    {
        #region Constructors

        protected CStressType(long lId, string strName, string strDescription)
            : base(lId, strName, strDescription)
        {
        }
        #endregion Constructors

        #region Methods

        private static CStressType CreateInternal(long lId, string strName, string strDescription)
        {
            return new CStressType(lId, strName, strDescription);
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="lId"></param>
        ///// <param name="strDescription"></param>
        ///// <param name="strName"></param>
        ///// <param name="bInitial">All stress types are expected to be at least initial. It does not make sense
        ///// for a stress type to be only resulting because it would never generate a process and therefore
        ///// would have no effect on the physical analysis</param>
        ///// <param name="bResulting"></param>
        ///// <returns></returns>
        //public static CStressType CreateOperational(long? lId, string strName, string strDescription, bool bInitial = true, bool bResulting = false)
        //{
        //    return Create(lId, strName, strDescription, EStressCategory.Operational, bInitial, bResulting);
        //}

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="lId"></param>
        ///// <param name="strDescription"></param>
        ///// <param name="strName"></param>
        ///// <param name="bInitial">All stress types are expected to be at least initial. It does not make sense
        ///// for a stress type to be only resulting because it would never generate a process and therefore
        ///// would have no effect on the physical analysis</param>
        ///// <param name="bResulting"></param>
        ///// <returns></returns>
        //public static CStressType CreateEnvironmental(long? lId, string strName, string strDescription, bool bInitial = true, bool bResulting = false)
        //{
        //    return Create(lId, strName, strDescription, EStressCategory.Environmental, bInitial, bResulting);
        //}

        /// <summary>
        ///
        /// </summary>
        /// <param name="lId"></param>
        /// <param name="strDescription"></param>
        /// <param name="strName"></param>
        /// <param name="aobjParam"> IsEnvironementalType, IsInitial, IsResulting </param>
        /// <param name="eStressCategory"></param>
        /// <param name="bInitial">All stress types are expected to be at least initial. It does not make sense
        /// for a stress type to be only resulting because it would never generate a process and therefore
        /// would have no effect on the physical analysis</param>
        /// <param name="bResulting"></param>
        /// <returns></returns>
        public static CStressType Create(long? lId, string strName, string strDescription,
                                         object[] aobjParam)
        {
            CStressType oStressType = ms_oEntryHelper.CreateWithUniqueName(EUniquness.IdAndName, lId, strName,
                                                                                   strDescription, CreateInternal);
            if (oStressType != null)
            {
                //oStressType.m_eStressCategory = (bool)aobjParam[0] ? EStressCategory.Environmental : EStressCategory.Operational;
                oStressType.m_bIsInitial = (bool)aobjParam[0];
                oStressType.m_bIsResulting = (bool)aobjParam[1];
                oStressType.m_nProbabilityPropagateToElement = (int)aobjParam[2];
                oStressType.m_nProbabilityPropagateToElementJunction = (int)aobjParam[3];
                //oStressType.m_nMagnitude = (int)aobjParam[5];

                //switch (oStressType.m_eStressCategory)
                //{
                //    case EStressCategory.Operational:
                //        ms_lststrOperationalStresses.Add(strName);
                //        break;
                //    case EStressCategory.Environmental:
                //        ms_lststrEnvironementalStresses.Add(strName);
                //        break;
                //    default:
                //        break;
                //}
            }
            return oStressType;
        }

        public static CStressType GetByName(string strName)
        {
            return ms_oEntryHelper.GetByName(strName);
        }

        public static CStressType GetById(long lId)
        {
            return ms_oEntryHelper.GetById(lId);
        }

        public List<string> GetNameList()
        {
            return ms_oEntryHelper.GetNameList();
        }

        #endregion Methods

        #region Properties

        //public EStressCategory StressCategory
        //{
        //    get { return m_eStressCategory; }
        //}

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

        private bool m_bIsInitial = false;
        private bool m_bIsResulting = false;
        //private EStressCategory m_eStressCategory;
        private static List<string> ms_lststrOperationalStresses = new List<string>();
        private static List<string> ms_lststrEnvironementalStresses = new List<string>();
        private int m_nProbabilityPropagateToElement = CConstants.mc_nNumberDefault;
        private int m_nProbabilityPropagateToElementJunction = 5;
        //private int m_nMagnitude = CConstants.mc_nNumberDefault;

        protected static CEntryHelper<CStressType> ms_oEntryHelper = new CEntryHelper<CStressType>();

        #endregion MemberVariables

        #region InnerClasses

        #endregion InnerClasses
    }
}