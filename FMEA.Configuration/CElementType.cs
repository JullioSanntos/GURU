using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMEA.Common;

namespace FMEA.Configuration
{
    public class CElementType : CElementTypeBase, IElementType
    {
        #region Constructors
        protected CElementType(long lId, string strName, string strDescription) : base(lId, strName, strDescription)
        {
        }
        #endregion //Constructor

        #region Methods
        private static CElementTypeBase CreateInternal(long lId, string strName, string strDescription)
        {
            return new CElementType(lId, strName, strDescription);
        }

        public new static CElementType Create(long? lId, string strName, string strDescription)
        {
            CElementTypeBase oElementTypeBase = ms_oEntryHelper.CreateWithUniqueName(EUniquness.IdAndName, lId, strName,
                                                                                     strDescription, CreateInternal);
            if (oElementTypeBase == null)
            {
                return null;
            }
            ms_lststrInstanceNames.Add(strName);
            return (CElementType) oElementTypeBase;
        }

        #endregion //Methods

        public static List<string> InstanceNameList
        {
            get { return ms_lststrInstanceNames; }
        }

        #region Properties

        #endregion //Properties

        #region MemberVariables
        private static List<string> ms_lststrInstanceNames = new List<string>(); 
        private List<CProcessType> m_lstprocesstypeCaused = new List<CProcessType>();
        #endregion //MemberVariables

        #region InnerClasses
        #endregion //InnerClasses
    }
}
