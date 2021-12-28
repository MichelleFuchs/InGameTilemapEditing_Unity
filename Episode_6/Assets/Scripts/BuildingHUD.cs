using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class BuildingHUD : Singleton<BuildingHUD> {
    [SerializeField] List<UICategory> categories;
    [SerializeField] Transform wrapperElement;
    [SerializeField] GameObject categoryPrefab;
    [SerializeField] GameObject itemPrefab;

    Dictionary<UICategory, GameObject> uiElements = new Dictionary<UICategory, GameObject>();
    Dictionary<GameObject, Transform> elementItemSlot = new Dictionary<GameObject, Transform>();

    private void Start() {
        BuildUI();
    }

    private void BuildUI() {

        foreach (UICategory cat in categories) {

            if (!uiElements.ContainsKey(cat)) {
                // instantiate new entry
                var inst = Instantiate(categoryPrefab, Vector3.zero, Quaternion.identity);
                inst.transform.SetParent(wrapperElement, false);

                uiElements[cat] = inst;
                elementItemSlot[inst] = inst.transform.Find("Items");
            }

            // set visible name
            Text text = uiElements[cat].GetComponentInChildren<Text>();
            text.text = cat.name;

            // set name in hierarchy
            uiElements[cat].name = cat.name;

            // set index  
            uiElements[cat].transform.SetSiblingIndex(cat.SiblingIndex);

            // set color
            Image img = uiElements[cat].GetComponentInChildren<Image>();
            img.color = cat.BackgroundColor;
        }

        BuildingObjectBase[] buildables = GetAllBuildables();

        foreach (BuildingObjectBase b in buildables) {
            if (b.UICategory == null) {
                continue;
            }

            // get items slot of the according category
            var itemsParent = elementItemSlot[uiElements[b.UICategory]];

            var inst = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
            inst.transform.SetParent(itemsParent, false);

            // set name in hierarchy
            inst.name = b.name;

            // Set Tile Image
            Image img = inst.GetComponent<Image>();
            Tile t = (Tile)b.TileBase;
            img.sprite = t.sprite;

            // Apply BuildingObjectBase to Button
            var script = inst.GetComponent<BuildingButtonHandler>();
            script.Item = b;
        }
    }

    private BuildingObjectBase[] GetAllBuildables() {
        return Resources.LoadAll<BuildingObjectBase>("Scriptables/Buildables");
    }

}