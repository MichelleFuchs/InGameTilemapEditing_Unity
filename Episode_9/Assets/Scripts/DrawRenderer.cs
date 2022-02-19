using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DrawRenderer {

    public static BoundsInt SingleRenderer(Vector3Int position) {
        BoundsInt bounds = new BoundsInt();

        bounds.min = position;
        bounds.max = position;

        return bounds;
    }

    public static BoundsInt RectangleRenderer(Vector3Int startPosition, Vector3Int currentPosition) {
        BoundsInt bounds = new BoundsInt();

        bounds.xMin = currentPosition.x < startPosition.x ? currentPosition.x : startPosition.x;
        bounds.xMax = currentPosition.x > startPosition.x ? currentPosition.x : startPosition.x;
        bounds.yMin = currentPosition.y < startPosition.y ? currentPosition.y : startPosition.y;
        bounds.yMax = currentPosition.y > startPosition.y ? currentPosition.y : startPosition.y;

        return bounds;
    }

    public static BoundsInt LineRenderer(Vector3Int startPosition, Vector3Int currentPosition) {
        BoundsInt bounds = new BoundsInt();

        float diffX = Mathf.Abs(currentPosition.x - startPosition.x);
        float diffY = Mathf.Abs(currentPosition.y - startPosition.y);

        bool lineIsHorizontal = diffX >= diffY;

        if (lineIsHorizontal) {
            bounds.xMin = currentPosition.x < startPosition.x ? currentPosition.x : startPosition.x;
            bounds.xMax = currentPosition.x > startPosition.x ? currentPosition.x : startPosition.x;
            bounds.yMin = startPosition.y;
            bounds.yMax = startPosition.y;
        } else {
            bounds.xMin = startPosition.x;
            bounds.xMax = startPosition.x;
            bounds.yMin = currentPosition.y < startPosition.y ? currentPosition.y : startPosition.y;
            bounds.yMax = currentPosition.y > startPosition.y ? currentPosition.y : startPosition.y;
        }

        return bounds;
    }

}
