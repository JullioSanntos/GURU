///////////////////////////////////////////////////////////
//  Copyright © Corning Incorporated  2013
//  CResultingProcess.cs
//  Implementation of the Class CResultingProcess
//  Created on:      20-Sep-2013 4:57:05 PM
//  Original author: Vincent Blaignan
///////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMEA.Common;

namespace FMEA.Configuration
{
    //[YAXSerializableType(FieldsToSerialize = YAXSerializationFields.AttributedFieldsOnly)]
    //[YAXComment("CResultingProcess")]
    //[YAXSerializeAs("CResultingProcess")]

    /// <summary>
    /// 
    /// </summary>
    public class CResultingProcess : CEntry, IResultingProcess
    {
        #region Constructors

        public CResultingProcess(IProcessType oProcessType, IElementTypeBase elementtypebaseCause)
            : this(oProcessType, elementtypebaseCause, null, null)
        {
        }

        public CResultingProcess(IProcessType oProcessType, IElementTypeBase elementtypebaseCause,
                                 List<IStressType> lststresstypeCause, List<IConditionType> lstconditiontypeCause)
        {
            m_oProcessType = oProcessType;
            m_elementtypebaseCause = elementtypebaseCause;
            if (lststresstypeCause == null)
                m_lststresstypeCause = new List<IStressType>();
            else
                m_lststresstypeCause = lststresstypeCause;
            if (lstconditiontypeCause == null)
                m_lstconditiontypeCause = new List<IConditionType>();
            else
                m_lstconditiontypeCause = lstconditiontypeCause;
        }

        #endregion Constructors

        #region Methods
        public void AddStressType(IStressType iStressType)
        {
            m_lststresstypeCause.Add(iStressType);
        }

        public void AddConditionType(IConditionType iConditionType)
        {
            m_lstconditiontypeCause.Add(iConditionType);
        }
        #endregion Methods

        #region Properties

        public override string Name
        {
            get { return m_oProcessType.Name; }
        }

        public override string Description
        {
            get { return m_oProcessType.Description; }
        }

        public IProcessType ProcessType
        {
            get { return m_oProcessType; }
        }

        public IElementTypeBase ElementTypeBaseCause
        {
            get { return m_elementtypebaseCause; }
        }

        public List<IStressType> StressTypeCauseList
        {
            get { return m_lststresstypeCause; }
        }

        public List<IConditionType> ConditionTypeCauseList
        {
            get { return m_lstconditiontypeCause; }
        }


        #endregion Properties

        #region MemberVariables

        private IProcessType m_oProcessType;
        private IElementTypeBase m_elementtypebaseCause;
        private List<IStressType> m_lststresstypeCause;
        private List<IConditionType> m_lstconditiontypeCause;
        #endregion MemberVariables

        #region InnerClasses

        #endregion InnerClasses
    }
}
