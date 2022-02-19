using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public enum PlaceType {
    None,
    Single,
    Line,
    Rectangle,
}

[CreateAssetMenu(fileName = "Category", menuName = "LevelBuilding/Create Category")]
public class BuildingCategory : ScriptableObject {
    [SerializeField] PlaceType placeType;
    [SerializeField] int sortingOrder = 0;
    [SerializeField] List<BuildingCategory> placementRestrictions;

    Tilemap tilemap;

    public List<BuildingCategory> PlacementRestrictions {
        get {
            return placementRestrictions;
        }
    }

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