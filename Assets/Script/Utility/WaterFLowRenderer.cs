using UnityEngine;

namespace Game.World
{
    public class WaterFlowRenderer : MonoBehaviour
    {
        [SerializeField] private WorldMap worldMap;


        public void Initialize(WorldMap worldMap)
        {
            this.worldMap = worldMap;
        }

        private void OnDrawGizmos()
        {
            if (worldMap == null) return;

            for (int x = 0; x < worldMap.Width; x++)
            {
                for (int y = 0; y < worldMap.Height; y++)
                {
                    Vector2 flow = worldMap.WaterFlowMap[x, y];
                    if (flow == Vector2.zero) continue;

                    Vector3 startPos = new(x + 0.5f, y + 0.5f, 0);
                    Vector3 endPos = startPos + (Vector3)(flow * 0.1f); 
                    Gizmos.color = Color.Lerp(Color.black, Color.black, Mathf.Clamp01(flow.magnitude * 0.5f));
                    Gizmos.DrawLine(startPos, endPos);
                    Gizmos.DrawSphere(endPos, 0.05f);
                }
            }
        }
    }
}