using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }
    public Canvas MainCanvas;
    public RectTransform CharacterPanel;
    public GameObject TextBubble;
    public GameObject TextBubbleDamageNumber;
    public GameObject HealthBar;
    public GameObject ArrowObject;
    public RectTransform StartScreen;
    public RectTransform UITrashParent;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    public void Update()
    {
        CharacterPanel.gameObject.SetActive(GameController.Instance.CurrentEntity != null);

        
    }
    public void SpawnTextBubble(string speech, Transform transform)
    {
        UITextBubbleMovement newTextBubble = Instantiate(TextBubble,  MainCanvas.transform, false).GetComponent<UITextBubbleMovement>();
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position+Vector3.up*3);
        newTextBubble.transform.position = screenPosition;
        newTextBubble.transform.SetParent(UITrashParent, false);
        newTextBubble.Speech = speech;
        newTextBubble.target = transform.position;
    }
    public void SpawnDamageNr(string speech, Transform transform, Color color, bool critical = false)
    {
        UITextBubbleMovement newTextBubble = Instantiate(TextBubbleDamageNumber, MainCanvas.transform, false).GetComponent<UITextBubbleMovement>();
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position - Vector3.up * 3);
        newTextBubble.transform.position = screenPosition;
        newTextBubble.transform.SetParent(UITrashParent, false);
        if (color == null) color = Color.red;
        newTextBubble.color = color;
        if (critical) speech = "<b>" + speech + " CRITICAL!</b>";
        //speech = "<color=red>" + speech + "</color>";
        newTextBubble.Speech = speech;
        newTextBubble.target = transform.position;
    }
    public GameObject SpawnHealthBar(StatusController statusController)
    {
        GameObject overhead = Instantiate(HealthBar);
        UIOverheadStatus bar = overhead.GetComponent<UIOverheadStatus>();
        overhead.transform.SetParent(UITrashParent,false);
        bar.statusController = statusController;
        return overhead;
    }
    public GameObject SpawnAwarenessArrow(SensesController sensesController)
    {
        GameObject overhead = Instantiate(ArrowObject, MainCanvas.transform, false);
        UIAwarenessArrow arrow = overhead.GetComponent<UIAwarenessArrow>();
        overhead.transform.SetParent(UITrashParent, false);
        arrow.sensesController = sensesController;
        return overhead;
    }
    public void ShowEndScreen()
    {
        StartScreen.gameObject.SetActive(true);
    }
    public void HideEndScreen()
    {
        StartScreen.gameObject.SetActive(false);
    }

}
