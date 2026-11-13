using UnityEngine;
using Game.World;
using UnityEngine.Tilemaps;

public class WorldManager : MonoBehaviour
{
    [Header("Map Settings")]
    [SerializeField] private int mapWidth = 256;
    [SerializeField] private int mapHeight = 256;

    [SerializeField] private Tilemap terrainTileMap;
    [SerializeField] private TileBase seaTile;
    [SerializeField] private TileBase groundTile;
    [SerializeField] private TileBase rockTile;
    [SerializeField] private TileBase snowTile;

    [SerializeField] private TileBase allowTile;


    [SerializeField] private WaterFlowRenderer waterFlowRenderer;
    [SerializeField] private WaterRenderer waterRenderer;

    private int seed = 0;

    private WorldMap currentMap;
    private WorldMap nextMap;
    private SpriteRenderer spriteRenderer;

    private WaterSimulation waterSimulation = new();

    private float tickTimer = 0f;



    void Start()
    {
        seed = Random.Range(0, 1000000000);
        currentMap = new WorldMap(mapWidth, mapHeight);
        currentMap.Initialize(seed);
        nextMap = currentMap.Clone();

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        waterFlowRenderer.Initialize(currentMap);


        // 4. 初回描画
        UpdateTileMap(currentMap);
        waterRenderer.RenderWater(currentMap);
    }

    void Update()
    {
        nextMap = currentMap.Clone();
        tickTimer += Time.deltaTime;

        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int vector = terrainTileMap.WorldToCell(mouseWorldPos);

            if (nextMap.WaterMap.IsInBounds(vector.x, vector.y))
            {
                nextMap.WaterMap[vector.x,vector.y] += 50;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int pos = terrainTileMap.WorldToCell(mouseWorldPos);

            if (nextMap.WaterMap.IsInBounds(pos.x, pos.y))
            {
                Debug.Log(nextMap.HeightMap[pos.x, pos.y] + " " + nextMap.WaterMap[pos.x, pos.y]);
            }
        }

        if (tickTimer >= 0.2)
        {
            tickTimer = 0f;
            waterSimulation.RunWater(currentMap, nextMap);
        }


        waterFlowRenderer.Initialize(nextMap);
        waterRenderer.RenderWater(nextMap);
        UpdateTileMap(nextMap);
        (currentMap,nextMap) = (nextMap,currentMap);
    }

    public void UpdateTileMap(WorldMap worldMap)
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                int h = worldMap.HeightMap[x,y];
                TileBase tile;
                if (h < 0) tile = rockTile;
                else if (h < 300) tile = groundTile;
                else if (h < 600) tile = rockTile;
                else tile = snowTile;
                terrainTileMap.SetTile(new (x,y,0),tile);
                int w = worldMap.WaterMap[x,y];
            }
        }
    }

}