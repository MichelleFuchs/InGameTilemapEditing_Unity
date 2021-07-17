using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class BuildingCreator : Singleton<BuildingCreator> {
    [SerializeField] Tilemap previewMap,
    defaultMap;
    PlayerInput playerInput;

    TileBase tileBase;
    BuildingObjectBase selectedObj;

    Camera _camera;

    Vector2 mousePos;
    Vector3Int currentGridPosition;
    Vector3Int lastGridPosition;

    protected override void Awake () {
        base.Awake ();
        playerInput = new PlayerInput ();
        _camera = Camera.main;
    }

    private void OnEnable () {
        playerInput.Enable ();

        playerInput.Gameplay.MousePosition.performed += OnMouseMove;
        playerInput.Gameplay.MouseLeftClick.performed += OnLeftClick;
        playerInput.Gameplay.MouseRightClick.performed += OnRightClick;
    }

    private void OnDisable () {
        playerInput.Disable ();

        playerInput.Gameplay.MousePosition.performed -= OnMouseMove;
        playerInput.Gameplay.MouseLeftClick.performed -= OnLeftClick;
        playerInput.Gameplay.MouseRightClick.performed -= OnRightClick;
    }

    private BuildingObjectBase SelectedObj {
        set {
            selectedObj = value;

            tileBase = selectedObj != null ? selectedObj.TileBase : null;

            UpdatePreview ();
        }
    }

    private void Update () {
        // if something is selected - show preview
        if (selectedObj != null) {
            Vector3 pos = _camera.ScreenToWorldPoint (mousePos);
            Vector3Int gridPos = previewMap.WorldToCell (pos);

            if (gridPos != currentGridPosition) {
                lastGridPosition = currentGridPosition;
                currentGridPosition = gridPos;

                UpdatePreview ();
            }
        }
    }

    private void OnMouseMove (InputAction.CallbackContext ctx) {
        mousePos = ctx.ReadValue<Vector2> ();
    }

    private void OnLeftClick (InputAction.CallbackContext ctx) {
        if (selectedObj != null) {
            HandleDrawing ();
        }
    }

    private void OnRightClick (InputAction.CallbackContext ctx) {
        SelectedObj = null;
    }

    public void ObjectSelected (BuildingObjectBase obj) {
        SelectedObj = obj;
    }

    private void UpdatePreview () {
        // Remove old tile if existing
        previewMap.SetTile (lastGridPosition, null);
        // Set current tile to current mouse positions tile
        previewMap.SetTile (currentGridPosition, tileBase);
    }

    private void HandleDrawing () {
        DrawItem ();
    }

    private void DrawItem () {
        // TODO: automatically select tilemap
        defaultMap.SetTile (currentGridPosition, tileBase);
    }

}