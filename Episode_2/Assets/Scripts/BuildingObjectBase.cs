using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Category {
    Wall,
    Floor
}

public enum PlaceType {
    Single,
    Line,
    Rectangle
}

[CreateAssetMenu (fileName = "Buildable", menuName = "BuildingObjects/Create Buildable")]
public class BuildingObjectBase : ScriptableObject {
    [SerializeField] Category category;
    [SerializeField] TileBase tileBase;
    [SerializeField] PlaceType placeType;

    public TileBase TileBase {
        get {
            return tileBase;
        }
    }

    public PlaceType PlaceType {
        get {
            return placeType;
        }
    }

    public Category Category {
        get {
            return category;
        }
    }

}