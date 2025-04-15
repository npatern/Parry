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
    [SerializeField]
    GameObject IllegalItem;

    public Image imageBack;
    [SerializeField]
    GameObject IllegalItemBack;
    public void Update()
    {
        RefreshImage();
    }
    public void RefreshImage()
    {
        IllegalItem.SetActive(false);
        IllegalItemBack.SetActive(false);
        if (GameController.Instance.CurrentPlayer == null) return;
        if (GameController.Instance.CurrentPlayer.GetComponent<ToolsController>() == null) return;

        ToolsController tools = GameController.Instance.CurrentPlayer.GetComponent<ToolsController>();
        if (tools.CurrentWeaponWrapper == null) return;

        ItemWeaponWrapper wrapper = tools.CurrentWeaponWrapper;
        if (wrapper.icon == null) return;

        if (GameController.Instance.CurrentPlayer.TryGetComponent<OutwardController>(out OutwardController outward))
        if (outward.GetWeaponIllegality(tools,wrapper) >0)
                IllegalItem.SetActive(true);

        image.sprite = wrapper.icon;
        ItemName.text = wrapper.ItemName;
        ItemDesc.text = wrapper.Description;
        if (wrapper.stack>1)
            ItemStack.text = ""+wrapper.stack;
        else
            ItemStack.text = "";

        //back weapon:
        ItemWeaponWrapper wrapperBack = tools.GetWeaponOnTheBack();
        imageBack.sprite = wrapperBack.icon;
        if (outward.GetWeaponIllegality(tools, wrapperBack) > 0)
            IllegalItemBack.SetActive(true);
    }
}
