using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PreviewHandler {
    Tilemap tilemap;
    TileBase tileBase;
    Dictionary<Vector3Int, TileBase> tilesBeforePreviewOverride = new Dictionary<Vector3Int, TileBase>();


    public void UpdateTile(Tilemap map, TileBase tile) {
        tilemap = map;
        tileBase = tile;
    }

    public TileBase GetPreviousTile(Vector3Int position) {
        if (tilesBeforePreviewOverride.ContainsKey(position)) {
            return tilesBeforePreviewOverride[position];
        }

        return null;
    }

    public void SetPreview(Vector3Int position, bool isForbidden) {
        if (tilemap == null) return;

        if (!tilesBeforePreviewOverride.ContainsKey(position)) {
            tilesBeforePreviewOverride.Add(position, tilemap.GetTile(position));
        }

        if (!isForbidden) {
            tilemap.SetTile(position, tileBase);
        }
    }

    public void SetPreview(Vector3Int[] positions, bool[] isForbidden) {
        ResetPreview();

        for (int i = 0; i < positions.Length; i++) {
            SetPreview(positions[i], isForbidden[i]);
        }
    }

    public void ResetPreview() {
        foreach (var pair in tilesBeforePreviewOverride) {
            tilemap.SetTile(pair.Key, pair.Value);
        }

        Clear();
    }

    public void ResetPreview(Vector3Int position) {
        if (tilesBeforePreviewOverride.ContainsKey(position)) {
            tilemap.SetTile(position, tilesBeforePreviewOverride[position]);
            tilesBeforePreviewOverride.Remove(position);
        } else {
            tilemap.SetTile(position, null);
        }
    }

    public void Clear() {
        tilesBeforePreviewOverride.Clear();
    }
}
