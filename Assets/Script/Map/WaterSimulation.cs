using System.Collections.Generic;
using UnityEngine;
namespace Game.World
{
    public class WaterSimulation
    {

        public void RunWater(WorldMap map) //水を流す
        {
            List<Vector2Int> directions = new(){Vector2Int.up,Vector2Int.down,Vector2Int.left,Vector2Int.right};
            var tmp = map.WaterMap.Clone();

            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    var cHeight = map.HeightMap[x,y];
                    if (cHeight < 0) continue; //海

                    var cWater  = map.WaterMap[x,y];
                    if (cWater == 0) continue; //水がない


                    int cWaterLevel = cHeight + cWater; //水位
                    int minWaterLevel = cWaterLevel;

                    int bestX = x;
                    int bestY = y;

                    foreach (Vector2Int dir in directions)
                    {
                        int nx = x + dir.x;
                        int ny = y + dir.y;
                        if (!map.HeightMap.IsInBounds(nx, ny)) continue;

                        var nWater = map.WaterMap[nx,ny];
                        var nHeifht = map.HeightMap[nx,ny];
                        if (nHeifht < 0)
                        {
                            bestX = nx;
                            bestY = ny;
                            break;
                        }
                        int nWaterLevel = nHeifht + nWater;

                        if (nWaterLevel < minWaterLevel)
                        {
                            minWaterLevel = nWaterLevel;
                            bestX = nx;
                            bestY = ny;
                        }
                    }

                    if (bestX != x || bestY != y)
                    {

                        short diff = (short)(cWaterLevel - minWaterLevel);
                        short flowAmount = (short)Mathf.Max(1, diff / 2);
                        tmp[x, y] -= flowAmount;
                        
                        if (map.HeightMap[bestX, bestY] >= 0) //海は除外
                        {
                            tmp[bestX, bestY] += flowAmount;
                        }
                    }
                }
            }
            map.WaterMap = tmp;

        }

    }
}