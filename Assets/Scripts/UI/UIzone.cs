using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIzone : MonoBehaviour
{
    public Image image;
    public Image[] images;
    [SerializeField]
    TextMeshProUGUI zoneName;
    Color color;
    //string text;
    public void Update()
    {
        RefreshImage();
    }
    public void RefreshImage()
    {
        if (GameplayController.Instance.CurrentPlayer == null) return;
        if (GameplayController.Instance.CurrentPlayer.GetComponent<OutwardController>() == null) return;
        if (GameplayController.Instance.CurrentPlayer.GetComponent<OutwardController>().activeZone == null) return;
        OutwardController outward = GameplayController.Instance.CurrentPlayer.GetComponent<OutwardController>();
        ZoneScriptable zone = GameplayController.Instance.CurrentPlayer.GetComponent<OutwardController>().activeZone;
        
        if (zone == null) return;
        zoneName.text = zone.name;
        int suspicionLevel = outward.HowMuchBeingIllegal();
        switch (suspicionLevel)
        {
            case 0:
                color = Color.white;
                break;
            case 1:
                color = Color.yellow;
                break;
            case 2:
                color = Color.red;
                break;

        }
        image.color = color;
        foreach (Image _image in images) _image.color = color;
    }
}
