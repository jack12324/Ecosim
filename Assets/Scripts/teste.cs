using System;
using Generators;
using UnityEngine;

namespace DefaultNamespace
{
    public class teste : MonoBehaviour
    {
        private MapAttributes _attributes;
        private MapGenerator _gen;
        private int _x;
        public bool newJob = true;
        private void Start()
        {
            _gen = GetComponent<MapGenerator>();
            _x = 0;
            _attributes = new MapAttributes(241,  _gen.noiseScale, _gen.octaves, _gen.persistence, _gen.lacunarity, _gen.offset, _gen.seed);
            InvokeRepeating(nameof(Noises), 2, 1);
        }

        private void Noises()
        {
            //_gen.offset.x = _x;
            if (!newJob)
            {
                // _gen.GenerateMapData();
            }
            else
            {
                //_gen.GenerateMapDataTestingNewNoiseJob();
            }
            //_x++;
        }
    }
}