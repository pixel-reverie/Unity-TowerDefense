using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class TowerButton : MonoBehaviour
{
    Button button;
    public Text costText;
    public Text towerName;
    Action OnClickCallback;

    public void Initialise(Tower tower, Action onClickCallback)
    {
        OnClickCallback = onClickCallback;
        button = this.GetComponent<Button>();
        costText.text = tower.cost.ToString();
        towerName.text = tower.towerName;
    }

    public void SetActiveState(bool active)
    {
        button.interactable = active;
    }

    public void OnClick()
    {
        OnClickCallback.Invoke();
    }
}