using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIzone : MonoBehaviour
{
    public Image image;
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
        if (GameController.Instance.CurrentPlayer == null) return;
        if (GameController.Instance.CurrentPlayer.GetComponent<OutwardController>() == null) return;
        if (GameController.Instance.CurrentPlayer.GetComponent<OutwardController>().activeZone == null) return;

        ZoneScriptable zone = GameController.Instance.CurrentPlayer.GetComponent<OutwardController>().activeZone;
        
        if (zone == null) return;
        zoneName.text = zone.name;

        switch (zone.DefaultRestriction)
        {
            case RestrictionType.FREE:
                color = Color.white;
                break;
            case RestrictionType.RESTRICTED:
                color = Color.yellow;
                break;
            case RestrictionType.HOSTILE:
                color = Color.red;
                break;

        }
        image.color = color;
    }
}
