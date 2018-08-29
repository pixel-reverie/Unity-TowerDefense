using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSlot : MonoBehaviour
{
    public Tower tower;

    public MeshRenderer meshRenderer;
    public Color defaultColour;
    public Color hoverColor;

    public void OnUpdate()
    {
        if(tower)
        {
            tower.OnUpdate();
        }
    }

    Action OnDestroyCallback;
    public void Initialise(Action onDestroyCallback)
    {
        OnDestroyCallback = onDestroyCallback;
        if (tower)
        {
            GameObject.Destroy(tower.gameObject);
            tower = null;
        }
    }

    public void PlaceTower(Tower towerPrefab, Action OnSuccess)
    {
        if (!tower)
        {
            Tower newTower = GameObject.Instantiate<Tower>(towerPrefab);
            newTower.transform.parent = this.transform;
            newTower.transform.localPosition = Vector3.zero;
            tower = newTower;
            tower.Initialise();
            OnSuccess.Invoke();
        }
    }
}
