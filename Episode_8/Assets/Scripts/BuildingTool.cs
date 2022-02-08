using UnityEngine;

enum ToolType {
    None,
    Eraser
}

[CreateAssetMenu(fileName = "Tool", menuName = "LevelBuilding/Create Tool")]
public class BuildingTool : BuildingObjectBase {
    [SerializeField] private ToolType toolType;
    ToolController tc;

    void Awake() {
        tc = ToolController.GetInstance();
    }

    public void Use(Vector3Int[] positions, out BuildingHistoryStep historyStep) {
        historyStep = null;
        switch (toolType) {
            case ToolType.Eraser:
                tc.Eraser(positions, out BuildingHistoryStep historyStepEraser);
                historyStep = historyStepEraser;
                break;
            default:
                Debug.LogError("Tool Type not set");
                break;
        }
    }
}

