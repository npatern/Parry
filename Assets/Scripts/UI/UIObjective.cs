using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIObjective : MonoBehaviour
{
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Description;
    public TextMeshProUGUI RequirementList;

    private void Update()
    {
        ObjectiveWrapper currentObjective = ObjectiveController.Instance.currentObjective;
        Name.text = currentObjective.Name;
        Description.text = currentObjective.Description;
        string requirementText = "";
        foreach (RequirementWrapper requirement in currentObjective.RequirementWrappers)
        {
            if (requirement.Completed)
                requirementText += "<s>";
            requirementText += requirement.Text;
            if (requirement.Completed)
                requirementText += "</s>";
            requirementText += "\n";
        }
        RequirementList.text = requirementText;
    }
}
