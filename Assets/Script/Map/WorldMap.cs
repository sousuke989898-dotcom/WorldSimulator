using System;
using UnityEngine;

namespace Game.World
{
    public class WorldMap
    {
        public int Width { get; }
        public int Height { get; }

        public const int BaseThermalEnergy = 10000;
        public const int HeightRange = 1000;


        public MapLayer<short> TerrainMap { get; set; }
        public MapLayer<short> HeightMap { get; set; }
        public MapLayer<short> WaterMap { get; set; }
        public MapLayer<int> ThermalEnergyMap {get; set;}

        public WorldMap(int width, int height)
        {
            if (width < 5 || height < 5)
                throw new ArgumentException("Map size must be at least 5x5.");
            Width = width;
            Height = height;

            TerrainMap = new(width, height);
            HeightMap = new(width, height);
            WaterMap = new(width, height);
            ThermalEnergyMap = new(width, height);

            System.Random rand = new();
            GenerateBaseMap(rand);
        }

        private void GenerateBaseMap(System.Random rand)
        {
            FastNoiseLite hNoise = new(rand.Next()); //標高ノイズ
            FastNoiseLite tNoise = new(rand.Next()); //気温ノイズ

            hNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2); // 滑らかなノイズ
            hNoise.SetFractalType(FastNoiseLite.FractalType.FBm);      // 波の重ね合わせ
            hNoise.SetFractalOctaves(4);                               // 重ね合わせる回数（多いほど細かい起伏が増える）
            hNoise.SetFrequency(0.005f);                                // 波の大きさ（数値を小さくすると広大な大陸、大きくすると細かい島々になる）

            tNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2); // 滑らかなノイズ
            tNoise.SetFrequency(0.002f);                              // 波の大きさ（数値を小さくすると広大な大陸、大きくすると細かい島々になる）

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    short height = (short)(hNoise.GetNoise(x,y) * HeightRange);
                    HeightMap[x, y] = height;

                    if (height < 0)WaterMap[x,y] = (short)-height; //海

                    float climateNoise = tNoise.GetNoise(x, y); 
                    int thermalVariation = (int)(climateNoise * 2000);
                    int initialEnergy = BaseThermalEnergy + thermalVariation;
                    ThermalEnergyMap[x, y] = Math.Max(0, initialEnergy);

                    
                }
            }
        }



        public bool CanMove(Vector2Int nextPos)
        {
            if (!TerrainMap.IsInBounds(nextPos)) return false;
            var h = HeightMap[nextPos];
            return h >= 0;
        }
    }
}