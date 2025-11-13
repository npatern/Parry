using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UICharacterPanel : MonoBehaviour
{
    public EntityController Entity;
    public TextMeshProUGUI NeedList;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI LogList;

    private void Update()
    {
        if (LevelController.Instance.CurrentEntity != null)
            Entity = LevelController.Instance.CurrentEntity;

        Redraw();
    }
    void Redraw()
    {
        if (Entity == null) return;
        Name.text = Entity.name;
        NeedList.text = GenerateListOfNeeds(Entity.ListOfNeeds);
        LogList.text = GenerateListOfLogs(Entity.Log);
    }
    string GenerateListOfNeeds(List<NeedScriptableObject> list)
    {
        string text = "";
        for (int i = 0; i < list.Count; i++)
        {
            if (i != 0) text += "<color=grey>";
            text +=" - "+ list[i].Name;
            if (i != 0) text += "</color>";
            text += "\n";
        }
        return text;
    }
    string GenerateListOfLogs(List<string> list)
    {
        string text = "";
        for (int i = 0; i < list.Count; i++)
        {
            if (i == list.Count-1) text = "<color=grey>"+text;
            text = " - " + list[i]+ "\n"+text;
        }
        return text;
    }

}
