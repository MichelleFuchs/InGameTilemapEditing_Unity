using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;


public class BuildingCreator : Singleton<BuildingCreator> {
    [SerializeField]
    Tilemap defaultMap, toolPreviewMap;

    // if one of those maps contains a tile at the position, placing is not allowed
    [SerializeField] List<Tilemap> forbidPlacingWithMaps;

    PlayerInput playerInput;
    BuildingHistory buildingHistory;
    PreviewHandler previewHandler = new PreviewHandler();

    TileBase tileBase;
    BuildingObjectBase selectedObj;

    Camera _camera;

    Vector2 mousePos;
    Vector3Int currentGridPosition;
    Vector3Int lastGridPosition;

    bool holdActive;
    Vector3Int holdStartPosition;

    BoundsInt bounds;

    protected override void Awake() {
        base.Awake();
        playerInput = new PlayerInput();
        _camera = Camera.main;
        buildingHistory = BuildingHistory.GetInstance();
    }

    private void OnEnable() {
        playerInput.Enable();

        playerInput.Gameplay.MousePosition.performed += OnMouseMove;

        playerInput.Gameplay.MouseClick.started += OnLeftClick;
        playerInput.Gameplay.MouseClick.performed += OnLeftClick;
        playerInput.Gameplay.MouseClick.canceled += OnLeftClick;

        playerInput.Gameplay.MouseRightClick.performed += OnRightClick;

    }

    private void OnDisable() {
        playerInput.Disable();

        playerInput.Gameplay.MousePosition.performed -= OnMouseMove;

        playerInput.Gameplay.MouseClick.started -= OnLeftClick;
        playerInput.Gameplay.MouseClick.performed -= OnLeftClick;
        playerInput.Gameplay.MouseClick.canceled -= OnLeftClick;

        playerInput.Gameplay.MouseRightClick.performed -= OnRightClick;
    }

    /// <section>
    /// GETTER & SETTER
    /// </section>

    private BuildingObjectBase SelectedObj {
        set {
            selectedObj = value;

            tileBase = selectedObj != null ? selectedObj.TileBase : null;

            previewHandler.UpdateTile(tilemap, tileBase);
            UpdatePreview();
        }
    }

    private Tilemap tilemap {
        get {
            if (selectedObj != null && selectedObj.Category != null && selectedObj.Category.Tilemap != null) {
                return selectedObj.Category.Tilemap;
            }

            if (selectedObj != null && selectedObj is BuildingTool) {
                return toolPreviewMap;
            }

            return defaultMap;
        }
    }

    /// <section>
    /// UPDATE
    /// </section>

    private void Update() {
        // if something is selected - show preview
        if (selectedObj != null) {
            Vector3 pos = _camera.ScreenToWorldPoint(mousePos);
            Vector3Int gridPos = tilemap.WorldToCell(pos);

            if (gridPos != currentGridPosition) {
                lastGridPosition = currentGridPosition;
                currentGridPosition = gridPos;


                if (holdActive) {
                    HandleDrawing();
                } else {
                    UpdatePreview();
                }
            }
        }
    }

    /// <section>
    /// INPUT HANDLING
    /// </section>

    private void OnMouseMove(InputAction.CallbackContext ctx) {
        mousePos = ctx.ReadValue<Vector2>();
    }

    private void OnLeftClick(InputAction.CallbackContext ctx) {
        if (selectedObj != null && !EventSystem.current.IsPointerOverGameObject()) {
            if (ctx.phase == InputActionPhase.Started) {
                holdActive = true;

                if (ctx.interaction is TapInteraction) {
                    holdStartPosition = currentGridPosition;
                }
                HandleDrawing();
            } else {
                if (ctx.interaction is SlowTapInteraction || ctx.interaction is TapInteraction && ctx.phase == InputActionPhase.Performed) {
                    holdActive = false;
                    HandleDrawRelease();
                }
            }
        } else {
            previewHandler.ResetPreview();
            holdActive = false;
        }
    }

    private void OnRightClick(InputAction.CallbackContext ctx) {
        previewHandler.ResetPreview();
        holdActive = false;
        SelectedObj = null;
    }

    /// <section>
    /// PUBLIC METHODS
    /// </section>

    public void ObjectSelected(BuildingObjectBase obj) {
        SelectedObj = obj;
    }

    /// <section>
    /// DRAW HANDLING
    /// </section>

    private void UpdatePreview() {
        previewHandler.ResetPreview(lastGridPosition);
        previewHandler.SetPreview(currentGridPosition, IsForbidden(currentGridPosition));
    }

    private void HandleDrawing() {
        if (selectedObj != null) {
            switch (selectedObj.PlaceType) {
                case PlaceType.Line:
                    bounds = DrawRenderer.LineRenderer(holdStartPosition, currentGridPosition);
                    break;
                case PlaceType.Rectangle:
                    bounds = DrawRenderer.RectangleRenderer(holdStartPosition, currentGridPosition);
                    break;
                default:
                    bounds = DrawRenderer.SingleRenderer(currentGridPosition);
                    break;
            }

            // draw bounds for preview map
            DrawBounds(tilemap, true);
        }
    }

    private void HandleDrawRelease() {
        if (selectedObj != null) {
            switch (selectedObj.PlaceType) {
                case PlaceType.Line:
                case PlaceType.Rectangle:
                    DrawBounds(tilemap);
                    break;
                case PlaceType.Single:
                default:
                    DrawItem(tilemap, currentGridPosition, tileBase);
                    break;
            }
        }
    }

    private void DrawBounds(Tilemap map, bool isPreview = false) {
        List<Vector3Int> positions = new List<Vector3Int>();
        List<bool> isForbidden = new List<bool>();
        for (int x = bounds.xMin; x <= bounds.xMax; x++) {
            for (int y = bounds.yMin; y <= bounds.yMax; y++) {
                positions.Add(new Vector3Int(x, y, 0));
                isForbidden.Add(IsForbidden(new Vector3Int(x, y, 0)));
            }
        }

        if (isPreview) {
            previewHandler.SetPreview(positions.ToArray(), isForbidden.ToArray());
        } else {
            DrawItem(map, positions.ToArray(), tileBase);
        }
    }

    private void DrawItem(Tilemap map, Vector3Int position, TileBase tileBase) {
        Vector3Int[] positions = new Vector3Int[] { position };
        DrawItem(map, positions, tileBase);
    }

    private void DrawItem(Tilemap map, Vector3Int[] positions, TileBase tileBase) {

        if (selectedObj.GetType() == typeof(BuildingTool)) {
            // it is a tool
            previewHandler.ResetPreview();

            BuildingTool tool = (BuildingTool)selectedObj;

            tool.Use(positions, out BuildingHistoryStep historyStep);

            // a tool must not neccessarily return a historyStep
            if (historyStep != null) {
                buildingHistory.Add(historyStep);
            }

        } else {
            TileBase[] previousTiles = new TileBase[positions.Length];
            TileBase[] newTiles = new TileBase[positions.Length];

            for (int i = 0; i < positions.Length; i++) {
                previousTiles[i] = previewHandler.GetPreviousTile(positions[i]);

                if (!IsForbidden(positions[i])) {
                    newTiles[i] = tileBase;
                    map.SetTile(positions[i], tileBase);
                } else {
                    // if not allowed make sure to keep the previous one
                    newTiles[i] = previousTiles[i];
                }
            }

            buildingHistory.Add(new BuildingHistoryStep(map, previousTiles, newTiles, positions));

            previewHandler.Clear();
        }

        previewHandler.SetPreview(currentGridPosition, IsForbidden(currentGridPosition));
    }

    private bool IsForbidden(Vector3Int pos) {
        if (selectedObj == null) return false;

        List<BuildingCategory> restrictedCategories = selectedObj.PlacementRestrictions;
        // get the according tilemaps for each category
        List<Tilemap> restrictedMaps = restrictedCategories.ConvertAll(category => category.Tilemap);

        // merge both lists together
        List<Tilemap> allMaps = forbidPlacingWithMaps.Concat(restrictedMaps).ToList();

        return allMaps.Any(map => {
            if (map.HasTile(pos)) {
                return map.GetTile(pos) != tileBase;
            }

            return false;
        });
    }
}