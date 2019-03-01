using UnityEngine;
using UtilityKit;

public class TestModalWindow : MonoBehaviour
{
    void Start()
    {
        ModalPanelDetails modalPanelDetails = new ModalPanelDetails
        {
            questionString = "This is test",
            button1Details = new EventButtonDetails()
        };

        modalPanelDetails.button1Details.buttonString = "Yes!";
        modalPanelDetails.button1Details.action = YesFunction;

        modalPanelDetails.button2Details.buttonString = "No!";
        modalPanelDetails.button2Details.action = NoFunction;

        modalPanelDetails.button3Details.buttonString = "Cancel!";
        modalPanelDetails.button3Details.action = CancelFunction;

        ModalWindowManager.Choice(modalPanelDetails);
    }

    void YesFunction()
    {
        Debug.Log("You have selected Yes");
    }

    void NoFunction()
    {
        Debug.Log("You have selected No");
    }

    void CancelFunction()
    {
        Debug.Log("You have selected Cancel");
    }
}
