using UnityEngine;

namespace LooperPooper.Drums.Input.Analysis.Comparison
{
    public class TimeComparator
    {
        private const float THRESHOLD = 0.1f;
        
        public readonly bool IsEquals;
        
        public TimeComparator(float a, float b)
        {
            var delta = Mathf.Abs(a - b);
            
            IsEquals = delta < THRESHOLD;
        }
        
        public TimeComparator(float a, float b, float threshold)
        {
            var delta = Mathf.Abs(a - b);
            
            IsEquals = delta < threshold;
        }
    }
}