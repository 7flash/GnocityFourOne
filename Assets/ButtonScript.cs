using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using System.Runtime.InteropServices;

public class ButtonScript : MonoBehaviour
{
    // [DllImport("__Internal")]
    // public static extern void Authorize();
    GameObject btn;
    GameObject t1;
    GameObject t2;

    void Awake()
    {
        btn = GameObject.Find("Button1");
        t1 = GameObject.Find("Text1");
        t2 = GameObject.Find("Text2");

        t1.gameObject.SetActive(false);
        t2.gameObject.SetActive(false);
    }

    public void OnPressed()
    {
        BeginAuth();
    }

    public void BeginAuth() {
        // if editor
        if (Application.isEditor)
        {
            Debug.Log("Editor");
            CompleteAuth("sevenflash12");
        } else {
            // Authorize();
        }
    }

    public void CompleteAuth(string accountName) {
        print(accountName);
        // find T1 and enable
        t1.gameObject.SetActive(true);
        // // find T2 and enable
        t2.gameObject.SetActive(true);
        // change sprite color of CanvasBackground to 1D2333
        GameObject.Find("CanvasBackground").GetComponent<UnityEngine.UI.RawImage>().color = new Color32(0x1D, 0x23, 0x33, 0xFF);

        // // set text on t2
        t2.GetComponent<TextMeshProUGUI>().text = accountName;

        // // disable this one
        btn.SetActive(false);        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
