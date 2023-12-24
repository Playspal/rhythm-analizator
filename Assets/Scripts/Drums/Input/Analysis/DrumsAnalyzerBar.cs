using System.Collections.Generic;
using System.Linq;

namespace LooperPooper.Drums.Input.Analysis
{
    public class DrumsAnalyzerBar
    {
        public IReadOnlyList<DrumsAnalyzerBeat> Beats => _beats;
        public float BeginTime => _beats.Count > 0 ? _beats[0].Source.Time : 0;
        public float Duration => _beats.Count > 0 ? _beats[^1].Time + _beats[^1].Duration : 0;

        public int Size => _beats.Sum(beat => beat.Size);
        
        private readonly List<DrumsAnalyzerBeat> _beats = new ();

        public DrumsAnalyzerBar()
        {
        }
        
        public DrumsAnalyzerBar(DrumsAnalyzerBar source)
        {
            source.Beats
                .ToList()
                .ForEach(beat => 
                    AddToEnd(new DrumsAnalyzerBeat(beat)));
        }

        public bool AddToBegin(DrumsAnalyzerBeat beat)
            => TryInsertAt(0, beat);

        public bool AddToEnd(DrumsAnalyzerBeat beat)
            => TryInsertAt(_beats.Count, beat);
        
        public bool TryTakeFromBegin(out DrumsAnalyzerBeat beat)
            => TryTakeFrom(0, out beat);
        
        public bool TryTakeFromEnd(out DrumsAnalyzerBeat beat)
            => TryTakeFrom(_beats.Count - 1, out beat);

        private bool TryInsertAt(int index, DrumsAnalyzerBeat beat)
        {
            beat.Bar = this;

            if (index == 0)
            {
                _beats.Insert(0, beat);    
            }
            else
            {
                _beats.Add(beat);
            }
            

            return true;
        }
        
        private bool TryTakeFrom(int index, out DrumsAnalyzerBeat beat)
        {
            if (index < 0 || index >= _beats.Count)
            {
                beat = null;
                return false;
            }
            
            beat = _beats[index];

            _beats.Remove(beat);
            
            return true;
        }
    }
}