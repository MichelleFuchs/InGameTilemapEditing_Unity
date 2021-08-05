using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Buildable", menuName = "BuildingObjects/Create Buildable")]
public class BuildingObjectBase : ScriptableObject {
    [SerializeField] BuildingCategory category;
    [SerializeField] TileBase tileBase;
    [SerializeField] PlaceType placeType;

    public TileBase TileBase {
        get {
            return tileBase;
        }
    }

    public PlaceType PlaceType {
        get {
            return placeType == PlaceType.None ? category.PlaceType : placeType;
        }
    }

    public BuildingCategory Category {
        get {
            return category;
        }
    }

}