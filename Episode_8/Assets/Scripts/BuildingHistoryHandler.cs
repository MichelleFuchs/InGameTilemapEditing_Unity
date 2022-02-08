using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingHistoryHandler : MonoBehaviour {
    [SerializeField] Button undoButton, redoButton;
    BuildingHistory buildingHistory;

    private void Awake() {
        undoButton.onClick.AddListener(Undo);
        redoButton.onClick.AddListener(Redo);

        buildingHistory = BuildingHistory.GetInstance();
    }

    private void Update() {
        SetInteractable(buildingHistory.CanUndo, undoButton);
        SetInteractable(buildingHistory.CanRedo, redoButton);
    }

    private void SetInteractable(bool interactable, Button button) {
        if (button.interactable != interactable) {
            button.interactable = interactable;
        }
    }

    private void Undo() {
        buildingHistory.UndoStep();
    }

    private void Redo() {
        buildingHistory.RedoStep();
    }

}
