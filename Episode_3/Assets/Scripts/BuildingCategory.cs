using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum PlaceType {
    None,
    Single,
    Line,
    Rectangle
}

[CreateAssetMenu(fileName = "Category", menuName = "LevelBuilding/Create Category")]
public class BuildingCategory : ScriptableObject {
    [SerializeField] PlaceType placeType;
    [SerializeField] int sortingOrder = 0;
    Tilemap tilemap;

    public PlaceType PlaceType {
        get {
            return placeType;
        }
    }

    public Tilemap Tilemap {
        get {
            return tilemap;
        }

        set {
            tilemap = value;
        }
    }

    public int SortingOrder {
        get {
            return sortingOrder;
        }
    }

}
