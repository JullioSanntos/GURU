using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMEA.Common;

namespace FMEA.Configuration
{
    class CStressOperationalType : CStressType
    {
        #region Constructors

        protected CStressOperationalType(long lId, string strName, string strDescription) : base(lId, strName, strDescription)
        {
        }

        #endregion //Constructor

        #region Methods
        private static CStressType CreateInternal(long lId, string strName, string strDescription)
        {
            return new CStressOperationalType(lId, strName, strDescription);
        }

        public new static CStressOperationalType Create(string strDescription, string strName)
        {
            CStressType oElementType = ms_oEntryHelper.CreateWithUniqueName(EUniquness.Name, strName,
                                                                                     strDescription, CreateInternal);
            if (oElementType == null)
            {
                return null;
            }
            ms_lststrInstanceNames.Add(strName);
            return (CStressOperationalType)oElementType;
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
