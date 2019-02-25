using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace UtilityKit
{
    public class ModalWindowManager : Singleton<ModalWindowManager>
    {
        public TextMeshProUGUI question;
        public Button button1;
        public Button button2;
        public Button button3;

        public TextMeshProUGUI button1Text;
        public TextMeshProUGUI button2Text;
        public TextMeshProUGUI button3Text;

        public GameObject modalPanelObject;

        public static void Choice(ModalPanelDetails details)
        {
            Instance.modalPanelObject.SetActive(true);
            Instance.modalPanelObject.transform.SetAsLastSibling();

            Instance.button1.gameObject.SetActive(false);
            Instance.button2.gameObject.SetActive(false);
            Instance.button3.gameObject.SetActive(false);

            Instance.question.text = details.questionString;

            Instance.button1.onClick.RemoveAllListeners();
            Instance.button1.onClick.AddListener(details.button1Details.action);
            Instance.button1.onClick.AddListener(ClosePanel);
            Instance.button1Text.text = details.button1Details.buttonString;
            Instance.button1.gameObject.SetActive(true);

            if (details.button2Details != null)
            {
                Instance.button2.onClick.RemoveAllListeners();
                Instance.button2.onClick.AddListener(details.button2Details.action);
                Instance.button2.onClick.AddListener(ClosePanel);
                Instance.button2Text.text = details.button2Details.buttonString;
                Instance.button2.gameObject.SetActive(true);
            }

            if (details.button3Details != null)
            {
                Instance.button3.onClick.RemoveAllListeners();
                Instance.button3.onClick.AddListener(details.button3Details.action);
                Instance.button3.onClick.AddListener(ClosePanel);
                Instance.button3Text.text = details.button3Details.buttonString;
                Instance.button3.gameObject.SetActive(true);
            }
        }

        public static void ClosePanel()
        {
            Animator animator = Instance.GetComponentInChildren<Animator>();
            animator.SetTrigger("ClosePanel");
            Instance.StartCoroutine(WaitForToolPanelClosed(animator.GetCurrentAnimatorStateInfo(0).length));
        }

        static IEnumerator WaitForToolPanelClosed(float delay)
        {
            if (delay != 0)
                yield return new WaitForSeconds(delay);

            Instance.modalPanelObject.SetActive(false);
        }
    }
}