namespace LooperPooper.Drums.Input.Analysis
{
    public class DrumsAnalyzerBarOperation
    {
        private readonly DrumsAnalyzerBar _barA;
        private readonly DrumsAnalyzerBar _barB;
        private readonly DrumsAnalyzerBar _barC;
        
        public DrumsAnalyzerBarOperation(DrumsAnalyzerBar barA, DrumsAnalyzerBar barB, DrumsAnalyzerBar barC)
        {
            _barA = barA;
            _barB = barB;
            _barC = barC;
        }

        public bool TryDo(DrumsAnalyzerBarOperationType operationType)
        {
            if (!IsOperationValid(operationType))
            {
                return false;
            }
            
            switch (operationType)
            {
                case DrumsAnalyzerBarOperationType.AtoB:
                    return TryMoveLastBeatToNextBar(_barA, _barB);

                case DrumsAnalyzerBarOperationType.AtoB_BtoC:
                    return TryMoveLastBeatToNextBar(_barA, _barB) &&
                           TryMoveLastBeatToNextBar(_barB, _barC);

                case DrumsAnalyzerBarOperationType.AtoB_CtoB:
                    return TryMoveLastBeatToNextBar(_barA, _barB) &&
                           TryMoveFirstBeatToPreviousBar(_barC, _barB);

                case DrumsAnalyzerBarOperationType.BtoA:
                    return TryMoveFirstBeatToPreviousBar(_barB, _barA);

                case DrumsAnalyzerBarOperationType.BtoA_BtoC:
                    return TryMoveFirstBeatToPreviousBar(_barB, _barA) && 
                           TryMoveLastBeatToNextBar(_barB, _barC);

                case DrumsAnalyzerBarOperationType.BtoA_CtoB:
                    return TryMoveFirstBeatToPreviousBar(_barB, _barA) &&
                           TryMoveFirstBeatToPreviousBar(_barC, _barB);

                case DrumsAnalyzerBarOperationType.BtoC:
                    return TryMoveLastBeatToNextBar(_barB, _barC);

                case DrumsAnalyzerBarOperationType.CtoB:
                    return TryMoveFirstBeatToPreviousBar(_barC, _barB);

                default:
                    return false;
            }
        }
        
        private static bool TryMoveFirstBeatToPreviousBar(DrumsAnalyzerBar from, DrumsAnalyzerBar to)
        {
            if (!from.TryTakeFromBegin(out var beat))
            {
                return false;
            }
            
            to.AddToEnd(beat);
            return true;
        }
        
        private static bool TryMoveLastBeatToNextBar(DrumsAnalyzerBar from, DrumsAnalyzerBar to)
        {
            if (!from.TryTakeFromEnd(out var beat))
            {
                return false;
            }
            
            to.AddToBegin(beat);
            return true;
        }

        private bool IsOperationValid(DrumsAnalyzerBarOperationType operationType)
        {
            var isBarCRequired = operationType == DrumsAnalyzerBarOperationType.BtoC ||
                                 operationType == DrumsAnalyzerBarOperationType.CtoB ||
                                 operationType == DrumsAnalyzerBarOperationType.AtoB_BtoC ||
                                 operationType == DrumsAnalyzerBarOperationType.AtoB_CtoB ||
                                 operationType == DrumsAnalyzerBarOperationType.BtoA_BtoC ||
                                 operationType == DrumsAnalyzerBarOperationType.BtoA_CtoB;

            return !isBarCRequired || _barC != null;
        }
    }
}