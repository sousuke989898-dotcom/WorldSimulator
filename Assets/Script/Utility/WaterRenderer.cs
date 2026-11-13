using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.World
{
    public class WaterRenderer : MonoBehaviour
    {
        [SerializeField] private Tilemap waterTilemap;
        [SerializeField] private Tile waterTile;

        public void RenderWater(WorldMap map)
        {
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    int waterAmount = map.WaterMap[x, y];
                    Vector3Int cellPos = new Vector3Int(x, y, 0);

                    if (waterAmount <= 0)
                    {
                        waterTilemap.SetTile(cellPos, null);
                    }
                    else
                    {
                        if (waterTilemap.GetTile(cellPos) == null)
                        {
                            waterTilemap.SetTile(cellPos, waterTile);
                        }

                        float alpha = Mathf.Lerp(0.2f, 1.0f, Mathf.Clamp01(waterAmount / 10f));
                        
                        Color waterColor = new(0.2f, 0.5f, 1.0f, alpha);
                        
                        waterTilemap.SetColor(cellPos, waterColor);
                    }
                }
            }
        }
    }
}