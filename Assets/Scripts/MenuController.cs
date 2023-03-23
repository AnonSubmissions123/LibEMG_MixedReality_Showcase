using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine.UI;
using TMPro;

public class MenuController : MonoBehaviour
{
    public Text text;
    public GameObject menu;
    private GridObjectCollection gc;
    private ScrollingObjectCollection so;
    private MyoReaderClient emgReader;
    private int selectedButton = 3;

    void Start() {
        gc = FindObjectOfType<GridObjectCollection>();
        so = FindObjectOfType<ScrollingObjectCollection>();
        emgReader = FindObjectOfType<MyoReaderClient>();
        text.text = "";
        menu.SetActive(false);
    }

    void Update() {
        if (emgReader.consecutive == 5) {
            if (emgReader.control == "4") {
                // Move Up - Flexion
                UpScroll();
            } else if (emgReader.control == "3") {
                // Move Down - Extension
                DownScroll();
            } else if (emgReader.control == "0") {
                // Hand close
                ButtonClick();
            } else if (emgReader.control == "1") {
                // Hand Open - Change Menu Open
                menu.SetActive(!menu.activeSelf);
            }
            if (menu.activeSelf) {
                UpdateButtonColors();
            }
            emgReader.control = ""; // Reset
            emgReader.consecutive = 0;
        }
    }

    void ButtonClick() {
        text.text = "Button " + selectedButton + " clicked!";
    }

    void DownScroll() {
        if (selectedButton < 10) {
            selectedButton += 1;
        }
        so.MoveByTiers(1);
        gc.UpdateCollection();
    }

    void UpScroll() {
        if (selectedButton > 1) {
            selectedButton -= 1;
        }
        so.MoveByTiers(-1);
        gc.UpdateCollection();
    }

    private void UpdateButtonColors() {
        for(int i=1; i<11; i++) {
            GameObject button = GameObject.Find("Button"+i);
            var icon = button.transform.Find("IconAndText");
            var textMeshPro = icon.GetComponentInChildren<TextMeshPro>();
            if (textMeshPro) {
                if(i == selectedButton) {
                    textMeshPro.color = Color.red;
                    textMeshPro.fontStyle = FontStyles.Underline;
                } else {
                    textMeshPro.color = Color.white;
                    textMeshPro.fontStyle = FontStyles.Normal;
                }
            }
        }
    }
}
