using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIItem : MonoBehaviour
{
    public Image image;
    [SerializeField]
    TextMeshProUGUI ItemName;
    [SerializeField]
    TextMeshProUGUI ItemDesc;
    [SerializeField]
    TextMeshProUGUI ItemStack;
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

        ItemWeaponWrapper wrapper = GameController.Instance.CurrentPlayer.GetComponent<ToolsController>().CurrentWeaponWrapper;

        image.sprite = wrapper.icon;
        ItemName.text = wrapper.ItemName;
        ItemDesc.text = wrapper.Description;
        if (wrapper.stack>1)
            ItemStack.text = ""+wrapper.stack;
        else
            ItemStack.text = "";
    }
}
