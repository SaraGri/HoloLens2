using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUILogic : MonoBehaviour
{
    //für die Buttonsteuerung
    public Button[] uiButtons = new Button[8];
    private bool activElement;
    private List<Button> activeUIButtons;
    public InputField fileName;
    // Start is called before the first frame update
    void Start()
    {
        fileName = fileName.GetComponent<InputField>();
        activeUIButtons = new List<Button>();
        activElement = false;
        for (int i = 0; i < uiButtons.Length; i++)
        {
            uiButtons[i] = uiButtons[i].gameObject.GetComponent<Button>();
        }
    }

    public void checkUIObjectsActivity()
    {

        for (int i = 0; i < uiButtons.Length; i++)
        {
            if (uiButtons[i].gameObject.activeSelf)
            {
                activeUIButtons.Add(uiButtons[i]);
                uiButtons[i].gameObject.SetActive(false);
            }
        }
        if (fileName.gameObject.activeSelf)
        {
            fileName.gameObject.SetActive(false);
            activElement = true;
        }
    }

    public void reactivateClosedUIElements()
    {
        foreach (var button in activeUIButtons)
        {
            button.gameObject.SetActive(true);
        }
        if (activElement)
        {
            fileName.gameObject.SetActive(true);
            activElement = false;
        }
        activeUIButtons = new List<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void closeAllButtons()
    {
        for (int i = 0; i < uiButtons.Length; i++)
        {
            if (uiButtons[i].gameObject.activeSelf) {
                uiButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void closeButt(int a, int b)
    {
         if (uiButtons[a].gameObject.activeSelf) 
        {
                uiButtons[a].gameObject.SetActive(false);
        }

        if (uiButtons[b].gameObject.activeSelf)
        {
            uiButtons[b].gameObject.SetActive(false);
        }
    }

    public void activateButtons(int i)
    {
            if (!uiButtons[i].gameObject.activeSelf) {
                uiButtons[i].gameObject.SetActive(true);
            }
    }

    public void closeButton (int a) {
        if (uiButtons[a].gameObject.activeSelf)
        {
            uiButtons[a].gameObject.SetActive(false);
        }
    }

}
