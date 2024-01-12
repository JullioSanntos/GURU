using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMEA.Common;

namespace FMEA.Configuration
{
    class CStressEnvironmentalType : CStressType
    {
        #region Constructors

        protected CStressEnvironmentalType(long lId, string strName, string strDescription)
            : base(lId, strName, strDescription)
        {
        }

        #endregion //Constructor

        #region Methods
        private static CStressType CreateInternal(long lId, string strName, string strDescription)
        {
            return new CStressEnvironmentalType(lId, strName, strDescription);
        }

        public new static CStressEnvironmentalType Create(string strDescription, string strName)
        {
            CStressType oElementType = ms_oEntryHelper.CreateWithUniqueName(EUniquness.Name, strName,
                                                                                     strDescription, CreateInternal);
            if (oElementType == null)
            {
                return null;
            }
            ms_lststrInstanceNames.Add(strName);
            return (CStressEnvironmentalType)oElementType;
        }
        #endregion //Methods

        #region Properties
        #endregion //Properties

        #region MemberVariables
        private static List<string> ms_lststrInstanceNames = new List<string>();
        #endregion //MemberVariables

        #region InnerClasses
        #endregion //InnerClasses
    }
}
