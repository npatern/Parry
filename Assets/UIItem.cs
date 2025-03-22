using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItem : MonoBehaviour
{
    public Image image;
    public void Update()
    {
        RefreshImage();
    }
    public void RefreshImage()
    {
        if (GameController.Instance.CurrentPlayer == null) return;
        if (GameController.Instance.CurrentPlayer.GetComponent<ToolsController>() == null) return;
        if (GameController.Instance.CurrentPlayer.GetComponent<ToolsController>().CurrentWeaponWrapper == null) return;
        if (GameController.Instance.CurrentPlayer.GetComponent<ToolsController>().CurrentWeaponWrapper.icon == null) return;

        image.sprite = GameController.Instance.CurrentPlayer.GetComponent<ToolsController>().CurrentWeaponWrapper.icon;
    }
}
