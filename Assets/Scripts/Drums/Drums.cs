using LooperPooper.Drums.Input;
using LooperPooper.Drums.Playback;
using UnityEngine;

namespace LooperPooper.Drums
{
    public class Drums
    {
        public bool IsInput => _input != null;
        
        private readonly int _kick;
        private readonly int _snare;
        private readonly int _hat;
        private readonly int _crash;
        
        private DrumsInput _input;
        public DrumsPlayer Player { get; set; }

        public Drums(int kick, int snare, int hat, int crash)
        {
            _kick = kick;
            _snare = snare;
            _hat = hat;
            _crash = crash;
        }
        
        public void Update()
        {
            Player?.Update();
        }

        public void InputBegin()
        {
            _input = new DrumsInput();
            Player = null;
        }

        public void InputEnd()
        {
            _input.End();
            Player = new DrumsPlayer(_kick, _snare, _hat, _crash, new DrumsInputParser(_input).Parse());
            _input = null;
        }

        public void InsertKick()
        {
            _input.Add(DrumsBeatType.Kick);
            AndroidNativeAudio.play(_kick);
        }

        public void InsertSnare()
        {
            _input.Add(DrumsBeatType.Snare);
            AndroidNativeAudio.play(_snare, 0.75f);
        }
    }
}