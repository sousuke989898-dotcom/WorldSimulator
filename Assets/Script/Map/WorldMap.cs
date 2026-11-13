using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Game.World
{
    public class WorldMap
    {
        public int Width { get; }
        public int Height { get; }

        public const int HeightRange = 1000;

        public MapLayer<int> HeightMap { get; set; }
        public MapLayer<int> WaterMap { get; set; }
        public MapLayer<Vector2Int> WaterFlowMap { get; set; }


        public WorldMap(int width, int height)
        {
            if (width < 5 || height < 5)
                throw new ArgumentException("Map size must be at least 5x5.");
            Width = width;
            Height = height;

            HeightMap = new(width, height);
            WaterMap = new(width, height);
            WaterFlowMap = new(width, height);

        }
        
        public void Initialize(int seed)
        {
            GenerateBaseMap(seed);
        }

        private void GenerateBaseMap(int seed)
        {
            FastNoiseLite hNoise = new(seed); //標高ノイズ

            hNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2); // 滑らかなノイズ
            hNoise.SetFractalType(FastNoiseLite.FractalType.FBm);      // 波の重ね合わせ
            hNoise.SetFractalOctaves(4);                               // 重ね合わせる回数（多いほど細かい起伏が増える）
            hNoise.SetFrequency(0.005f);                                // 波の大きさ（数値を小さくすると広大な大陸、大きくすると細かい島々になる）


            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int height = (int)(hNoise.GetNoise(x,y) * HeightRange);
                    HeightMap[x, y] = height;

                    if (height < 0)WaterMap[x,y] = -height; //海
                }
            }

        }



        public int GetSafeHeight(int x, int y)
        {
            if (IsInBounds(x,y)) return HeightMap[x,y];
            else return HeightRange;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsInBounds(int x, int y) => (uint)x < (uint)Width && (uint)y < (uint)Height;


        public bool CanMove(Vector2Int nextPos)
        {
            if (!HeightMap.IsInBounds(nextPos)) return false;
            var h = HeightMap[nextPos];
            return h >= 0;
        }

        public WorldMap Clone()
        {
            return new(Width, Height)
            {
                HeightMap = HeightMap.Clone(),
                WaterMap = WaterMap.Clone(),
                WaterFlowMap = WaterFlowMap.Clone(),
            };
        }
    }
}