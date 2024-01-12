using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMEA.Common;
using FMEA.Configuration;
using FMEA.System;

namespace FMEA.Engine
{
    //VBB_TODO: Continue
    class CElementAnalysed : CEntry
    {
        //public CElementAnalysed(long lId, string strName, string strDescription, IElementTypeBase iElementTypeBase, List<IFailureModeType> lstiFailureModeType, List<IConditionType> lsticonditiontypeInitial, List<IStressType> lstistresstypeInitial, List<CElementAnalysed> lstelementanalysedNeighbor, IElementBase ielementbaseFromSystem) : base(lId, strName, strDescription)
        public CElementAnalysed(IElementBase iElementBase) : base(iElementBase.Id, iElementBase.Name, iElementBase.Description)
        {
            m_iElementTypeBase = iElementBase.ElementTypeBase;
            m_lstiFailureModeType = iElementBase.FailureModeTypeList;
            m_lsticonditiontypeInitial = iElementBase.ConditiontypeInitialList;
            m_lstistresstypeInitial = iElementBase.StresstypeInitialList;
            m_lstelementanalysedNeighbor = null;
            m_ielementbaseFromSystem = iElementBase;
        }

        public void SetNeighbor(List<CElementAnalysed> lstelementanalysedNeighbor)
        {
            m_lstelementanalysedNeighbor = lstelementanalysedNeighbor;
        }

        protected IElementTypeBase m_iElementTypeBase;
        protected List<IFailureModeType> m_lstiFailureModeType;
        protected List<IConditionType> m_lsticonditiontypeInitial;
        protected List<IStressType> m_lstistresstypeInitial;
        protected List<CElementAnalysed> m_lstelementanalysedNeighbor;
        protected IElementBase m_ielementbaseFromSystem;
        protected bool m_bNewStressOrConditionAdded = true;
    }
}
