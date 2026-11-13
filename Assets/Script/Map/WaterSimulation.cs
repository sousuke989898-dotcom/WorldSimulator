using UnityEngine;
namespace Game.World
{
    public class WaterSimulation
    {
        public Vector2Int[] directions = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left};

        public void RunWater(WorldMap currentMap, WorldMap nextMap)
        {
            for (int x = 0; x < currentMap.Width; x++)
            {
                for (int y = 0; y < currentMap.Height; y++)
                {
                    int cHeight = currentMap.HeightMap[x,y];
                    if (cHeight < 0)
                    {
                        nextMap.WaterFlowMap[x,y] = Vector2Int.zero;
                        continue;
                    }
                    int cWater = currentMap.WaterMap[x,y];
                    if (cWater <= 0)
                    {
                        nextMap.WaterFlowMap[x,y] = Vector2Int.zero;
                        continue;
                    }
                    int cLevel = cHeight + cWater;

                    Vector2Int cFlow = currentMap.WaterFlowMap[x, y];
                    
                    float[] weights = new float[4];
                    float totalWeight = 0f;
                    int validCount = 0;

                    for (int i = 0; i < directions.Length; i ++)
                    {
                        int nx = x + directions[i].x;
                        int ny = y + directions[i].y;
                        if (!currentMap.IsInBounds(nx, ny)) continue;

                        int nHeight = currentMap.HeightMap[nx, ny];
                        int nWater = currentMap.WaterMap[nx, ny];
                        int nLevel;

                        if (nHeight <= 0)
                        {
                            nLevel = -WorldMap.HeightRange; 
                        }
                        else
                        {
                            nLevel = nHeight + nWater;
                        }
                        if (nLevel >= cLevel) continue;

                        float diff = cLevel - nLevel;

                        Vector2Int dirVec = directions[i];
                        float dot = (cFlow.x * dirVec.x) + (cFlow.y * dirVec.y);
                        float inertiaBonus = 1.0f;
                        if (dot > 0)
                        {
                            inertiaBonus += dot * 0.2f;
                        }
                        weights[i] = diff * inertiaBonus;
                        totalWeight += weights[i];
                        validCount++;
                    }

                    if (validCount == 0 || totalWeight <= 0f) continue;

                    for (int i = 0; i < directions.Length; i++)
                    {
                        if (weights[i] > 0f)
                        {
                            weights[i] /= totalWeight;
                        }
                    }

                    // int amountToFlow = Mathf.Max(1,  cWater / 2);
                    int amountToFlow = Mathf.Max(1, cWater / 2);
                    int distributedWater = 0;

                    int bestDirectionIndex = -1;
                    float maxWeight = -1f;

                    for (int i = 0; i < directions.Length; i++)
                    {
                        if (weights[i] <= 0f) continue;

                        if (weights[i] > maxWeight)
                        {
                            maxWeight = weights[i];
                            bestDirectionIndex = i;
                        }

                        int nx = x + directions[i].x;
                        int ny = y + directions[i].y;

                        int flowWater = (int)(amountToFlow * weights[i]);
                        if (flowWater <= 0) continue;

                        distributedWater += flowWater;

                        if (currentMap.HeightMap[nx, ny] >= 0)
                        {
                            nextMap.WaterMap[nx, ny] += flowWater;
                            nextMap.WaterFlowMap[nx, ny] += directions[i] * flowWater;
                        }

                    }

                    if (distributedWater == 0 && bestDirectionIndex != -1)
                    {
                        int i = bestDirectionIndex;
                        int nx = x + directions[i].x;
                        int ny = y + directions[i].y;

                        distributedWater = 1; // 強制的に1流す

                        if (currentMap.HeightMap[nx, ny] >= 0)
                        {
                            nextMap.WaterMap[nx, ny] += 1;
                            nextMap.WaterFlowMap[nx, ny] += directions[i];
                        }
                    }

                    nextMap.WaterMap[x, y] -= distributedWater;

                    if (distributedWater > 0)
                    {
                        nextMap.WaterFlowMap[x, y] /= 2;
                    }
                    else
                    {
                        nextMap.WaterFlowMap[x, y] = Vector2Int.zero;
                    }
                }
            }
        }
    }
}