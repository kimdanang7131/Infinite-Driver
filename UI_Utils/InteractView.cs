using UnityEngine;
using UnityEngine.UI;

public class InteractView : MonoBehaviour
{
    [SerializeField] private Button myButton;
    [SerializeField] private GameObject[] otherPanels;

    void Start()
    {
        if(myButton == null)
        {
            Utils.LogError();
            return;
        }

        myButton.onClick.AddListener(() => myButton.interactable = false);
    }
}
