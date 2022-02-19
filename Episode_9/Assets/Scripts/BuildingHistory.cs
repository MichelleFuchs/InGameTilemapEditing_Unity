using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingHistory : Singleton<BuildingHistory> {
    List<BuildingHistoryStep> history = new List<BuildingHistoryStep>();
    int currentIndex = -1;

    public bool CanUndo => currentIndex >= 0;
    public bool CanRedo => currentIndex < history.Count - 1;

    public void Add(BuildingHistoryStep entry) {
        // remove everything after the current index
        history.RemoveRange(currentIndex + 1, history.Count - (currentIndex + 1));
        history.Add(entry);
        currentIndex++;
    }

    public void UndoStep() {
        if (currentIndex > -1) {
            history[currentIndex].Undo();
            currentIndex--;
        }
    }

    public void RedoStep() {
        if (currentIndex < history.Count - 1) {
            currentIndex++;
            history[currentIndex].Redo();
        }
    }
}

public class BuildingHistoryStep {
    private BuildingHistoryItem[] historyItems;

    public BuildingHistoryStep(Tilemap[] maps, TileBase[] previousTiles, TileBase[] newTiles, Vector3Int[] positions) {
        historyItems = new BuildingHistoryItem[positions.Length];

        for (int i = 0; i < positions.Length; i++) {
            historyItems[i] = new BuildingHistoryItem(maps[i], previousTiles[i], newTiles[i], positions[i]);
        }
    }

    public BuildingHistoryStep(Tilemap map, TileBase[] previousTiles, TileBase[] newTiles, Vector3Int[] positions) {
        historyItems = new BuildingHistoryItem[positions.Length];

        for (int i = 0; i < positions.Length; i++) {
            historyItems[i] = new BuildingHistoryItem(map, previousTiles[i], newTiles[i], positions[i]);
        }
    }

    public BuildingHistoryStep(BuildingHistoryItem[] items) {
        historyItems = items;
    }

    public void Undo() {
        foreach (BuildingHistoryItem item in historyItems) {
            item.Undo();
        }
    }

    public void Redo() {
        foreach (BuildingHistoryItem item in historyItems) {
            item.Redo();
        }
    }
}

public class BuildingHistoryItem {
    private Tilemap map;
    private Vector3Int position;
    private TileBase previousTile;
    private TileBase newTile;

    public BuildingHistoryItem(Tilemap map, TileBase previousTile, TileBase newTile, Vector3Int position) {
        this.map = map;
        this.previousTile = previousTile;
        this.newTile = newTile;
        this.position = position;
    }

    public void Undo() {
        map.SetTile(position, previousTile);
    }

    public void Redo() {
        map.SetTile(position, newTile);
    }
}
