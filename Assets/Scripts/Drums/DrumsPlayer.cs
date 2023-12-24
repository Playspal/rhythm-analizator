using System.Collections;
using System.Collections.Generic;
using LooperPooper.Drums.Playback;
using UnityEngine;

namespace LooperPooper.Drums
{
    public class DrumsPlayer
    {
        public readonly DrumsLoop DrumsLoop;

        public int Pointer = 0;
        
        private readonly int _kick;
        private readonly int _snare;
        private readonly int _hat;
        private readonly int _crash;
        
        private readonly float _beatDuration = 0f;
        
        private int _pointerBar = 0;
        private int _pointerBeat = 0;
        private float _pointerTime = 0;

        private bool _charged = true;
        
        public DrumsPlayer(int kick, int snare, int hat, int crash, DrumsLoop drumsLoop)
        {
            DrumsLoop = drumsLoop;

            _kick = kick;
            _snare = snare;
            _hat = hat;
            _crash = crash;

            _beatDuration = 60f / drumsLoop.BeatsPerMinute / (drumsLoop.BeatsPerBar / 4);
        }

        public void Update()
        {
            if (_charged)
            {
                var beatKick = DrumsLoop.Bars[_pointerBar].BeatsKick[_pointerBeat];
                var beatSnare = DrumsLoop.Bars[_pointerBar].BeatsSnare[_pointerBeat];

                if (beatKick.Type != DrumsBeatType.None)
                {
                    AndroidNativeAudio.play(_kick, 1f);
                }
                
                if (beatSnare.Type != DrumsBeatType.None)
                {
                    AndroidNativeAudio.play(_snare, 0.75f);
                }
                
                if (Pointer % 2 == 0)
                {
                    AndroidNativeAudio.play(_hat, 0.1f);
                }

                if (_pointerBar == DrumsLoop.Bars.Length - 1)
                {
                    if (_pointerBeat == DrumsLoop.BeatsPerBar - 2)
                    {
                        AndroidNativeAudio.play(_crash, 0.5f);
                    }
                }
                
                _charged = false;
            }
            
            _pointerTime += Time.deltaTime;

            if (_pointerTime > _beatDuration)
            {
                _charged = true;
                
                _pointerTime -= _beatDuration;
                _pointerBeat++;
                Pointer++;
                
                if (_pointerBeat >= DrumsLoop.BeatsPerBar)
                {
                    _pointerBeat = 0;
                    _pointerBar++;

                    if (_pointerBar >= DrumsLoop.Bars.Length)
                    {
                        _pointerBar = 0;
                        Pointer = 0;
                    }
                }
            }
        }
    }
}