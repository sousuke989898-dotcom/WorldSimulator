using UnityEngine;
using Game.World;

public class WorldManager : MonoBehaviour
{
    [Header("Map Settings")]
    [SerializeField] private int mapWidth = 256;
    [SerializeField] private int mapHeight = 256;

    [Header("Display Mode")]
    [SerializeField] private DrawMode currentDrawMode = DrawMode.Terrain;

    [Header("Color Settings")]
    [SerializeField] private Gradient terrainGradient;
    [SerializeField] private Gradient heightGradient;
    [SerializeField] private short minHeight = -1000; // 最低標高
    [SerializeField] private short maxHeight = 1000;  // 最高標高


    private WorldMap worldMap;
    private Texture2D mapTexture;
    private SpriteRenderer spriteRenderer;

    private WaterSimulation waterSimulation = new();

    public enum DrawMode
    {
        Terrain,
        Height,
        Water,
        ThermalEnergy,
        Temperature
    }

    void Start()
    {
        worldMap = new WorldMap(mapWidth, mapHeight);

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        mapTexture = new Texture2D(mapWidth, mapHeight)
        {
            filterMode = FilterMode.Point
        };

        // 4. 初回描画
        UpdateMapDisplay();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) { currentDrawMode = DrawMode.Terrain; UpdateMapDisplay(); }
        if (Input.GetKeyDown(KeyCode.F2)) { currentDrawMode = DrawMode.Height; UpdateMapDisplay(); }
        if (Input.GetKeyDown(KeyCode.F3)) { currentDrawMode = DrawMode.Water; UpdateMapDisplay(); }
        if (Input.GetKeyDown(KeyCode.F4)) { currentDrawMode = DrawMode.ThermalEnergy; UpdateMapDisplay(); }
        if (Input.GetKeyDown(KeyCode.F5)) { currentDrawMode = DrawMode.Temperature; UpdateMapDisplay(); }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            // マップの中心を原点(0,0)にしている場合の座標変換
            // （※WorldManagerの配置やピボットによって微調整が必要です）
            int x = Mathf.FloorToInt(mouseWorldPos.x + (mapWidth / 2f));
            int y = Mathf.FloorToInt(mouseWorldPos.y + (mapHeight / 2f));

            if (worldMap.TerrainMap.IsInBounds(x, y))
            {
                Debug.Log($"--- Cell ({x}, {y}) ---");
                Debug.Log($"Height: {worldMap.HeightMap[x, y]}");
                Debug.Log($"Water: {worldMap.WaterMap[x, y]}");
                Debug.Log($"Thermal Energy: {worldMap.ThermalEnergyMap[x, y]}");
                //Debug.Log($"Temperature: {worldMap.GetTemperature(x, y):F2}");
            }

            
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int x = Mathf.FloorToInt(mouseWorldPos.x + (mapWidth / 2f));
            int y = Mathf.FloorToInt(mouseWorldPos.y + (mapHeight / 2f));

            if (worldMap.TerrainMap.IsInBounds(x, y))
            {
                worldMap.WaterMap[x,y] += 50;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            waterSimulation.RunWater(worldMap);
            UpdateMapDisplay();
        }


    }

    public void UpdateMapDisplay()
    {
        Color[] colorPixels = new Color[mapWidth * mapHeight];

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                int index = y * mapWidth + x;
                colorPixels[index] = GetPixelColorForMode(x, y);
            }
        }

        mapTexture.SetPixels(colorPixels);
        mapTexture.Apply();

        Sprite sprite = Sprite.Create(
            mapTexture,
            new Rect(0, 0, mapWidth, mapHeight),
            new Vector2(0.5f, 0.5f),
            1f
        );

        spriteRenderer.sprite = sprite;
    }

    private Color GetPixelColorForMode(int x, int y)
    {
        switch (currentDrawMode)
        {
            case DrawMode.Terrain:
                var height = worldMap.HeightMap[x, y];
                return GetTerrainColor(height);

            case DrawMode.Height:
                return heightGradient.Evaluate(Mathf.InverseLerp(minHeight,maxHeight,worldMap.HeightMap[x,y]));

            case DrawMode.Water:
                var water = worldMap.WaterMap[x, y];
                if (water == 0) return Color.black;
                float wAlpha = Mathf.Clamp01(water / 50f);
                return new Color(0f, 0.5f, 1f, wAlpha);

            case DrawMode.ThermalEnergy:
                int energy = worldMap.ThermalEnergyMap[x, y];
                float eNorm = Mathf.Clamp01((energy - 8000f) / 4000f);
                return new Color(eNorm, 0f, 1f - eNorm); // 熱い＝赤、冷たい＝青

            // case DrawMode.Temperature:
            //     float temp = worldMap.GetTemperature(x, y);
            //     float tNorm = Mathf.Clamp01((temp - 3000f) / 7000f);
            //     return GetTemperatureColor(tNorm);

            default:
                return Color.black;
        }
    }

    private Color GetTemperatureColor(float t)
    {
        if (t < 0.5f)
        {
            float ratio = t * 2.0f;
            return new Color(ratio, ratio, 1f); // 青 -> 白
        }
        else
        {
            float ratio = (t - 0.5f) * 2.0f;
            return new Color(1f, 1f - ratio, 1f - ratio); // 白 -> 赤
        }
    }

    private Color GetTerrainColor(short height)
    {
        // 1標高を 0.0f ～ 1.0f の範囲に正規化
        float t = Mathf.InverseLerp(minHeight, maxHeight, height);
        return terrainGradient.Evaluate(t);
    }
}