using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Action OnRoadPlacementButtonClicked;
    public Action OnHousePlacementButtonClicked;
    public Action OnSpecialPlacementButtonClicked;

    public Button placeRoadButton;
    public Button placeHouseButton;
    public Button placeSpecialButton;

    public Color outlineColor;
    List<Button> buttonList;

    private void Start()
    {
        buttonList = new List<Button> { placeRoadButton, placeHouseButton, placeSpecialButton };

        placeRoadButton.onClick.AddListener(() =>
        {
            ResetButtonColor();
            ModifyButtonOutline(placeRoadButton);
            OnRoadPlacementButtonClicked?.Invoke();
        });

        placeHouseButton.onClick.AddListener(() =>
        {
            ResetButtonColor();
            ModifyButtonOutline(placeHouseButton);
            OnHousePlacementButtonClicked?.Invoke();
        });

        placeSpecialButton.onClick.AddListener(() =>
        {
            ResetButtonColor();
            ModifyButtonOutline(placeSpecialButton);
            OnSpecialPlacementButtonClicked?.Invoke();
        });
    }

    private void ModifyButtonOutline(Button button)
    {
        var outline = button.GetComponent<Outline>();
        outline.effectColor = outlineColor;
        outline.enabled = true;
    }

    private void ResetButtonColor()
    {
        foreach (var button in buttonList)
        {
            button.GetComponent<Outline>().enabled = false;
        }
    }
}
