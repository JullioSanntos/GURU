using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMEA.Configuration
{
    public class CProcessMapper
    {
        #region Constructors

        public CProcessMapper()
        {
        }

        public CProcessMapper(List<CMatrixLine> lstoMatrixLine)
        {
            m_lstoMatrixLine = lstoMatrixLine;
        }

        #endregion Constructors

        #region Methods

        //public void AddMatrixLine(CMatrixLine oMatrixLine)
        //{
        //    m_lstoMatrixLine.Add(oMatrixLine);
        //}

        public void AddProcess(CProcessType oProcessType, IElementTypeBase[] aiElementTypeBase,
                               IStressType[] aiStressTypeInitial, IConditionType[] aiConditionTypeInitial)
        {
            m_lstoMatrixLine.Add(new CMatrixLine(oProcessType, aiElementTypeBase, aiStressTypeInitial, aiConditionTypeInitial));
        }

        public List<IResultingProcess> GetProcesses(IElementTypeBase iElementTypeBase, IEnumerable<IStressType> alInitialStressBaseIndex,
                                               IEnumerable<IConditionType> alInitialConditionIndex)
        {
            //return (from oMatrixLine in m_lstoMatrixLine
            //        where
            //            oMatrixLine.IsProcessGenerated(iElementTypeBase, ienumStressTypeApplied,
            //                                           ienumConditionTypeApplied)
            //        select oMatrixLine.ProcessTypeInterface).ToList();
            List<IResultingProcess> lstiProcessType = new List<IResultingProcess>();
            foreach (CMatrixLine oMatrixLine in m_lstoMatrixLine)
            {
                //if (oMatrixLine.IsProcessGenerated(iElementTypeBase, alInitialStressBaseIndex, alInitialConditionIndex))
                //{
                //    lstiProcessType.Add(oMatrixLine.ProcessTypeInterface);
                //}
                IResultingProcess iResultingProcess = oMatrixLine.GetGeneratedProcess(iElementTypeBase, alInitialStressBaseIndex, alInitialConditionIndex);
                if (iResultingProcess!=null)
                {
                    lstiProcessType.Add(iResultingProcess);
                }
            }
            return lstiProcessType;
        }

        #endregion Methods

        #region Properties

        #endregion Properties

        #region MemberVariables

        private List<CMatrixLine> m_lstoMatrixLine = new List<CMatrixLine>();

        #endregion MemberVariables

        #region InnerClasses

        #endregion InnerClasses
    }

    public class CMatrixLine
    {
        #region Constructors

        public CMatrixLine(CProcessType oProcessType, IElementTypeBase[] aiElementTypeBase,
                           IStressType[] aiStressTypeInitial, IConditionType[] aiConditionTypeInitial)
        {
            Init(oProcessType, aiElementTypeBase, aiStressTypeInitial, aiConditionTypeInitial);
        }

        #endregion Constructors

        #region Methods

        public override string ToString()
        {
            //return base.ToString();
            return string.Format("[MatrixLine - {0} E{1} C{2} S{3}]", m_oProcessType.Name, m_aiElementTypeBase.Count(),
                                 m_aiConditionTypeInitial.Count(), m_aiStressTypeInitial.Count());
        }

        //public void Init(string oProcessType, bool[] abMap, int nElementTypeBaseStartIndex,
        //                 int nInitialStressBaseStartIndex, int anInitialConditionStartIndex);
        //{
        //    List<int> lstnElementTypeBaseIndex;
        //    List<int> lstnInitialStressBaseIndex;
        //    List<int> lstnInitialConditionIndex;
        //    Init(oProcessType, alElementTypeBaseIndex, ienumStressTypeApplied, ienumConditionTypeApplied);
        //}

        public void Init(CProcessType oProcessType, IElementTypeBase[] alElementTypeBaseIndex,
                         IStressType[] alInitialStressBaseIndex, IConditionType[] alInitialConditionIndex)
        {
            m_oProcessType = oProcessType;
            m_aiElementTypeBase = alElementTypeBaseIndex;
            m_aiStressTypeInitial = alInitialStressBaseIndex;
            m_aiConditionTypeInitial = alInitialConditionIndex;
        }

        public bool IsProcessGenerated(IElementTypeBase iElementTypeBaseIndex, IEnumerable<IStressType> ienumStressTypeApplied,
                                       IEnumerable<IConditionType> ienumConditionTypeApplied)
        {
            bool bCheckElementBase = ContainsAtLeastOne(m_aiElementTypeBase, iElementTypeBaseIndex);
            bool bCheckStresses = (m_oProcessType.StressOperation == EOperation.Nop) ||
                                  ((m_oProcessType.StressOperation == EOperation.Or) &&
                                   ContainsAtLeastOne(m_aiStressTypeInitial, ienumStressTypeApplied)) ||
                                  ((m_oProcessType.StressOperation == EOperation.And) &&
                                   ContainsAllOrMore(m_aiStressTypeInitial, ienumStressTypeApplied));
            bool bCheckCondition = (m_oProcessType.ConditionOperation == EOperation.Nop) ||
                                  ((m_oProcessType.ConditionOperation == EOperation.Or) &&
                                   ContainsAtLeastOne(m_aiConditionTypeInitial, ienumConditionTypeApplied)) ||
                                  ((m_oProcessType.ConditionOperation == EOperation.And) &&
                                   ContainsAllOrMore(m_aiConditionTypeInitial, ienumConditionTypeApplied));
            return bCheckElementBase && bCheckStresses && bCheckCondition;
            return (ContainsAtLeastOne(m_aiElementTypeBase, iElementTypeBaseIndex)
                    && ContainsAtLeastOne(m_aiStressTypeInitial, ienumStressTypeApplied)
                    && ContainsAtLeastOne(m_aiConditionTypeInitial, ienumConditionTypeApplied));
        }

        public IResultingProcess GetGeneratedProcess(IElementTypeBase iElementTypeBase, IEnumerable<IStressType> ienumStressTypeApplied,
                                       IEnumerable<IConditionType> ienumConditionTypeApplied)
        {
            if (!ContainsAtLeastOne(m_aiElementTypeBase, iElementTypeBase))
                return null;
            List<IStressType> lstiStressTypes = null;
            List<IConditionType> lstiConditionTypes = null;
            int nMatchStressTypeCount = 0;
            int nMatchConditionTypeCount = 0;
            if (m_oProcessType.StressOperation != EOperation.Nop)
            {
                IEnumerable<IStressType> ienumEntry = FindAllMatches(m_aiStressTypeInitial, ienumStressTypeApplied);
                lstiStressTypes = (ienumEntry == null || !ienumEntry.Any()) ? null : ((IEnumerable<IStressType>)ienumEntry).ToList();
                nMatchStressTypeCount = lstiStressTypes == null ? 0 : lstiStressTypes.Count();
            }
            if (m_oProcessType.ConditionOperation != EOperation.Nop)
            {
                IEnumerable<IConditionType> ienumEntry = FindAllMatches(m_aiConditionTypeInitial, ienumConditionTypeApplied);
                lstiConditionTypes = (ienumEntry == null || !ienumEntry.Any()) ? null : ((IEnumerable<IConditionType>)ienumEntry).ToList();
                nMatchConditionTypeCount = lstiConditionTypes == null ? 0 : lstiConditionTypes.Count();
            }
            bool bCheckStresses = (m_oProcessType.StressOperation == EOperation.Nop) ||
                                  ((m_oProcessType.StressOperation == EOperation.Or) &&
                                   nMatchStressTypeCount != 0) ||
                                  ((m_oProcessType.StressOperation == EOperation.And) &&
                                   nMatchStressTypeCount == m_aiStressTypeInitial.Length);
            bool bCheckCondition = (m_oProcessType.ConditionOperation == EOperation.Nop) ||
                                  ((m_oProcessType.ConditionOperation == EOperation.Or) &&
                                   nMatchConditionTypeCount !=0 ) ||
                                  ((m_oProcessType.ConditionOperation == EOperation.And) &&
                                   nMatchConditionTypeCount == m_aiStressTypeInitial.Length);
            if (bCheckStresses && bCheckCondition)
            {
                return new CResultingProcess(m_oProcessType, iElementTypeBase, lstiStressTypes, lstiConditionTypes);
            }
            return null;
            //return bCheckElementBase && bCheckStresses && bCheckCondition;
            //return (ContainsAtLeastOne(m_aiElementTypeBase, iElementTypeBase)
            //        && ContainsAtLeastOne(m_aiStressTypeInitial, ienumStressTypeApplied)
            //        && ContainsAtLeastOne(m_aiConditionTypeInitial, ienumConditionTypeApplied));
        }

        private static bool ContainsAtLeastOne(IEnumerable<IEntry> alBase, IEntry lInput)
        {
            return alBase.Any(t => t.Id == lInput.Id);
            //for (int nBaseIndex = 0; nBaseIndex < alBase.Length; nBaseIndex++)
            //{
            //    if (alBase[nBaseIndex].Id == lInput)
            //    {
            //        return true;
            //    }
            //}
            //return false;
        }

        private static bool ContainsAtLeastOne(IEnumerable<IEntry> alBase, IEnumerable<IEntry> alInput)
        {
            return alBase.Any(delegate(IEntry t1) { return alInput.Any(delegate(IEntry t) { return t1.Id == t.Id; }); });
            //for (int nBaseIndex = 0; nBaseIndex < alBase.Length; nBaseIndex++)
            //{
            //    for (int nInputIndex = 0; nInputIndex < alInput.Length; nInputIndex++)
            //    {
            //        if (alBase[nBaseIndex] == alInput[nInputIndex])
            //        {
            //            return true;
            //        }
            //    }
            //}
            //return false;
        }

        private static bool ContainsAllOrMore(IEnumerable<IEntry> alBase, IEnumerable<IEntry> alInput)
        //private static bool ContainsAllOrMore(List<IEntry> alBase, List<IEntry> alInput)
        {
            foreach (IEntry entryBase in alBase)
            {
                bool bEntryFound = false;
                foreach (var entryInput in alInput)
                {
                    if (entryBase == entryInput)
                    {
                        bEntryFound = true;
                        break;
                    }
                }
                if (!bEntryFound)
                {
                    return false;
                }
            }
            return true;

#if false
            for (int nBaseIndex = 0; nBaseIndex < alBase.Count; nBaseIndex++)
            {
                int nInputIndex = 0;
                //while (nInputIndex<alInput.Count alBase[nBaseIndex] != alInput[nInputIndex])
                //{
                //    nInputIndex++;
                //}
                //if (nInputIndex>
                //{
                //}
                bool bEntryFound = false;
                for (nInputIndex = 0; nInputIndex < alInput.Count; nInputIndex++)
                {
                    if (alBase[nBaseIndex] == alInput[nInputIndex])
                    {
                        bEntryFound = true;
                        break;
                    }
                }
                if (!bEntryFound)
                {
                    return false;
                }
            }
            return true;
#endif
        }

        private static IEnumerable<IEntry> FindAllMatches(IEnumerable<IEntry> ienumEntry1, IEnumerable<IEntry> ienumEntry2)
        {
            return (from ientry1 in ienumEntry1 from ientry2 in ienumEntry2 where ientry1 == ientry2 select ientry1);
            //List<IEntry> lstiEntryMatch = new List<IEntry>();
            //foreach (IEntry ientry1 in ienumEntry1)
            //{
            //    foreach (IEntry ientry2 in ienumEntry2)
            //    {
            //        if (ientry1 == ientry2)
            //        {
            //            lstiEntryMatch.Add(ientry1);
            //        }
            //    }
            //}
            //return lstiEntryMatch;
        }

        private static IEnumerable<IConditionType> FindAllMatches(IEnumerable<IConditionType> ienumEntry1, IEnumerable<IConditionType> ienumEntry2)
        {
            return (from ientry1 in ienumEntry1 from ientry2 in ienumEntry2 where ientry1 == ientry2 select ientry1);
        }

        private static IEnumerable<IStressType> FindAllMatches(IEnumerable<IStressType> ienumEntry1, IEnumerable<IStressType> ienumEntry2)
        {
            return (from ientry1 in ienumEntry1 from ientry2 in ienumEntry2 where ientry1 == ientry2 select ientry1);
        }

        #endregion Methods

        #region Properties

        public CProcessType ProcessType
        {
            get { return m_oProcessType; }
        }

        public IProcessType ProcessTypeInterface
        {
            get { return m_oProcessType; }
        }

        #endregion Properties

        #region MemberVariables

        private CProcessType m_oProcessType;
        private IElementTypeBase[] m_aiElementTypeBase;
        private IStressType[] m_aiStressTypeInitial;
        private IConditionType[] m_aiConditionTypeInitial;

        #endregion MemberVariables
    }

    public static class CReverseDict<T>
    {
        public static Dictionary<T, int> Create(T[] aoT)
        {
            Dictionary<T, int> dicttnReturned = new Dictionary<T, int>(aoT.Length);
            for (int nIndex = 0; nIndex < aoT.Length; nIndex++)
            {
                dicttnReturned.Add(aoT[nIndex], nIndex);
            }
            return dicttnReturned;
        }
    }

    public class CMapper<TH, TV>
    {
        public CMapper(TH[] athHorizontal, TV[] atvVertical, bool[,] a2BMap)
        {
            m_athHorizontal = athHorizontal;
            m_atvVertical = atvVertical;
            m_a2bMap = a2BMap;
            m_dictthnIndexFromHorizontal = CReverseDict<TH>.Create(athHorizontal);
            m_dicttvnIndexFromVertical = CReverseDict<TV>.Create(atvVertical);
        }

        public List<TV> GetVertical(List<TH> lstthIn, EAndOr eAndOr)
        {
            int[] abIndexesOfListItems = new int[lstthIn.Count];

            for (int nListIndex = 0; nListIndex < lstthIn.Count; nListIndex++)
            {
                abIndexesOfListItems[nListIndex] = m_dictthnIndexFromHorizontal[lstthIn[nListIndex]];
            }
            List<TV> lsttvReturned = new List<TV>();
            for (int nVerIndex = 0; nVerIndex < m_atvVertical.Length; nVerIndex++)
            {
                //for (int nHorIndex = 0; nHorIndex <; nHorIndex++)
                //{
                //}
                int nInListIndex = 0;
                bool bDone = false;
                while (!bDone && nInListIndex < lstthIn.Count)
                {
                    if (eAndOr == EAndOr.AtLeastOneMatchFromInputList)
                    {
                        if (m_a2bMap[abIndexesOfListItems[nInListIndex], nVerIndex] == true)
                        {
                            lsttvReturned.Add(m_atvVertical[nVerIndex]);
                            bDone = true;
                        }
                    }
                    else if (eAndOr == EAndOr.AllMatchFromInputList)
                    {
                        if (m_a2bMap[abIndexesOfListItems[nInListIndex], nVerIndex] != true)
                        {
                            bDone = true;
                        }
                    }
                    nInListIndex++;
                }
                if (nInListIndex == lstthIn.Count && !bDone && eAndOr == EAndOr.AllMatchFromInputList)
                {
                    lsttvReturned.Add(m_atvVertical[nVerIndex]);
                }
            }
            return lsttvReturned;
        }

        private TH[] m_athHorizontal;
        private TV[] m_atvVertical;

        /// <summary>
        /// Contains the map [HIndex, VIndex]
        /// </summary>
        private bool[,] m_a2bMap;

        //private

        private Dictionary<TH, int> m_dictthnIndexFromHorizontal = new Dictionary<TH, int>();
        private Dictionary<TV, int> m_dicttvnIndexFromVertical = new Dictionary<TV, int>();
    }

    public enum EAndOr
    {
        AtLeastOneMatchFromInputList, AllMatchFromInputList,
    }
}