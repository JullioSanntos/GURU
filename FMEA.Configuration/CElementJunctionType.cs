using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using FMEA.Common;

namespace FMEA.Configuration
{
    public class CElementJunctionType : CElementTypeBase, IElementJunctionType
    {
        public const string strELEMENT_JUNCTION_TYPE_SEPARATOR = " - ";//"_";

        #region Constructors
        protected CElementJunctionType(long lId, string strName, string strDescription)
            : base(lId, strName, strDescription)
        {
        }
        #endregion //Constructor

        #region Methods
        private static CElementTypeBase CreateInternal(long lId, string strName, string strDescription)
        {
            return new CElementJunctionType(lId, strName, strDescription);
        }

        public new static CElementJunctionType Create(long? lId, string strName, string strDescription)
        {
            CElementTypeBase oElementTypeBase = ms_oEntryHelper.CreateWithUniqueName(EUniquness.IdAndName, lId, strName,
                                                                                     strDescription, CreateInternal);
            if (oElementTypeBase == null)
            {
                return null;
            }
            ms_lststrInstanceNames.Add(strName);
            return (CElementJunctionType)oElementTypeBase;
        }
        
        public static string CreateElementJunctionTypeName(string strElementTypeName1, string strElementTypeName2)
        {
            // ALGORITHM: Construction of Element Junction type name.
            return (string.Compare(strElementTypeName1, strElementTypeName2, true,
                                   CultureInfo.InvariantCulture) > 0)
                       ? (strElementTypeName2 + strELEMENT_JUNCTION_TYPE_SEPARATOR + strElementTypeName1)
                       : (strElementTypeName1 + strELEMENT_JUNCTION_TYPE_SEPARATOR + strElementTypeName2);
        }

        public static List<string> CreateExpectedElementJunctionTypeNameList(List<string> lststrElementJunctionTypeNames)
        {
            List<string> lststrReturn = new List<string>();
            for (int nIndex1 = 0; nIndex1 < lststrElementJunctionTypeNames.Count; nIndex1++)
            {
                for (int nIndex2 = 0; nIndex2 < lststrElementJunctionTypeNames.Count; nIndex2++)
                {
                    string strJunctionName = CreateElementJunctionTypeName(lststrElementJunctionTypeNames[nIndex1],
                                                                       lststrElementJunctionTypeNames[nIndex2]);
                    if (!lststrReturn.Contains(strJunctionName))
                    {
                       lststrReturn.Add(strJunctionName); 
                    }
                }
            }
            return lststrReturn;
        }

        #endregion //Methods

        #region Properties

        public static List<string> InstanceNameList
        {
            get { return ms_lststrInstanceNames; }
        }

        public string JunctionSeparator
        {
            get { return strELEMENT_JUNCTION_TYPE_SEPARATOR; }
        }
        #endregion //Properties

        #region MemberVariables
        private static List<string> ms_lststrInstanceNames = new List<string>();
        #endregion //MemberVariables

        #region InnerClasses
        #endregion //InnerClasses
    }
}
