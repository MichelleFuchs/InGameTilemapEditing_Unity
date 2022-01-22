using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class ToolController : Singleton<ToolController> {
    List<Tilemap> tilemaps = new List<Tilemap>();

    private void Start() {
        List<Tilemap> maps = FindObjectsOfType<Tilemap>().ToList();

        maps.ForEach(map => {
            if (map.name.Contains("Tilemap_")) {
                tilemaps.Add(map);
            }
        });

        tilemaps.Sort((a, b) => {
            TilemapRenderer aRenderer = a.GetComponent<TilemapRenderer>();
            TilemapRenderer bRenderer = b.GetComponent<TilemapRenderer>();

            return bRenderer.sortingOrder.CompareTo(aRenderer.sortingOrder);
        });
    }

    public void Eraser(Vector3Int[] positions, out BuildingHistoryStep historyStep) {
        // // Delete on ALL maps
        // tilemaps.ForEach(map => {
        //     map.SetTile(position, null);
        // });

        List<BuildingHistoryItem> items = new List<BuildingHistoryItem>();

        foreach (Vector3Int position in positions) {
            BuildingHistoryItem item = null;
            // Only delete the top tile
            tilemaps.Any(map => {
                if (map.HasTile(position)) {
                    item = new BuildingHistoryItem(map, map.GetTile(position), null, position);
                    map.SetTile(position, null);
                    return true;
                }

                return false;
            });

            // we don't save any values where nothing had happened
            if (item != null) {
                items.Add(item);
            }
        }

        historyStep = new BuildingHistoryStep(items.ToArray());
    }
}
