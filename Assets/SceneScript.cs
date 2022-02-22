using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using QuickType;
using UnityEngine.Networking;
using System;

public class SceneScript : MonoBehaviour
{
    [DllImport("__Internal")]
    public static extern void Setup();

    [DllImport("__Internal")]
    public static extern void Login();

    [DllImport("__Internal")]
    public static extern void Spin(string landKey, string accountName);

    [DllImport("__Internal")]
    public static extern void GetBalance(string accountName);

    private string accountName = "owensigor111";
    
    private string landKey = "";
    private GameObject accountSection;
    
    private GameObject landSection;

    // Start is called before the first frame update
    void Start()
    {
        accountSection = GameObject.Find("AccountSection");
        landSection = GameObject.Find("LandSection");

        SetupScene();

        SetupLands();

        SetupAccount();
    }

    void SetupAccount() {
        if (Application.isEditor)
        {
            SetupCallback();
        }
        else
        {
            Setup();
        }  
    }

    void SetupScene()
    {
        accountSection.SetActive(true);
        landSection.SetActive(true);

        accountSection.transform.Find("Authorized").gameObject.SetActive(false);
        accountSection.transform.Find("Unauthorized").gameObject.SetActive(true);
        landSection.transform.Find("SpinButton").gameObject.SetActive(false);

        accountSection.transform.Find("Authorized")
            .Find("LogoutButton")
            .GetComponent<Button>()
            .onClick.AddListener(() => {
                accountSection.transform.Find("Authorized").gameObject.SetActive(false);
                accountSection.transform.Find("Unauthorized").gameObject.SetActive(true);
                landSection.transform.Find("SpinButton").gameObject.SetActive(false);
            });
    }

    void SetupLands() {
        // read lands.json from resources as object
        TextAsset textAsset = Resources.Load<TextAsset>("lands");

        // parse json to object
        Land[] landItems = Land.FromJson(textAsset.text);

        ShowLand(landItems[0]);

        var landLabels = new List<string>();
        // add from landItems to landLabels
        foreach (var land in landItems)
        {
            landLabels.Add(land.LandId);
        }

        var dropdown = landSection.transform
            .Find("LandDropdown").GetComponent<TMP_Dropdown>();

        dropdown.AddOptions(landLabels);

        dropdown.onValueChanged.AddListener(delegate {
            Land chosenLand = landItems[dropdown.value];

            ShowLand(chosenLand);
        });
    }

    void ShowLand(Land land) {
        landSection.transform.Find("LandLocation")
            .GetComponent<TextMeshProUGUI>().text = land.Location;
    
        landSection.transform.Find("LandOwner")
            .GetComponent<TextMeshProUGUI>().text = land.Account;
    
        landKey = land.LandId;
    }

    void ShowBalance(string accountName) {
        GetBalance(accountName);
    }

    void GetBalanceCallback(string balance) {
        accountSection.transform.Find("Authorized")
            .Find("TokenBalance").GetComponent<TextMeshProUGUI>().text = balance;
    }

    void ShowBalanceNative(string accountName) {
        string queryUrl = "https://wax.greymass.com/v1/chain/get_table_rows";

        string queryBody = "{\"json\":true,\"code\":\"gnokentokens\",\"scope\":\""+accountName+"\",\"table\":\"accounts\",\"lower_bound\":null,\"upper_bound\":null,\"index_position\":1,\"key_type\":\"\",\"limit\":\"100\",\"reverse\":false,\"show_payer\":false}";

        print(queryBody);

        Dictionary<string, string> bodyQuery = new Dictionary<string, string>();
        bodyQuery.Add("json", "true");
        bodyQuery.Add("code", "gnokentokens");
        bodyQuery.Add("scope", "owensigor111");
        bodyQuery.Add("table", "accounts");

        // send post request
        UnityWebRequest www = UnityWebRequest.Post(queryUrl, bodyQuery);
        
        // set json headers
        www.SetRequestHeader("Content-Type", "application/json");

        // get response

        // wait for coroutine completed
        StartCoroutine(WaitForRequest(www, delegate (string balance) {
            accountSection.transform.Find("Authorized")
                .Find("TokenBalance").GetComponent<TextMeshProUGUI>().text = balance;            
        }));
    }

    private IEnumerator WaitForRequest(UnityWebRequest www, Action<string> callback)
    {
        // wait for response
        yield return www.SendWebRequest();

        // get response
        string response = www.downloadHandler.text;

        print(response);

        // parse response to object
        Balance balance = Balance.FromJson(response);

        string balanceValue = balance.Rows[0].Balance;

        callback.Invoke(balanceValue);
    }

    void SetupCallback() {
        accountSection.transform.Find("Unauthorized")
            .Find("LoginButton").GetComponent<Button>().onClick.AddListener(() => {
                if (Application.isEditor)
                {
                    LoginCallback(accountName);
                }
                else
                {
                    Login();
                }
            });

        landSection.transform.Find("SpinButton")
            .GetComponent<Button>().onClick.AddListener(() => {
                if (Application.isEditor)
                {
                    SpinCallback("trxid111");
                }
                else
                {
                    // TODO:
                    // currently the problem is that these two arguments are being passed as numbers unexpectedly 
                    Spin(landKey, accountName);
                }
            });
    }

    void LoginCallback (string accountName) {
        print("LoginCallback: " + accountName);

        this.accountName = accountName;

        accountSection.transform.Find("Authorized").gameObject.SetActive(true);
        accountSection.transform.Find("Unauthorized").gameObject.SetActive(false);
        landSection.transform.Find("SpinButton").gameObject.SetActive(true);

        accountSection.transform.Find("Authorized")
            .Find("AccountName").GetComponent<TextMeshProUGUI>().text = accountName;
    
        ShowBalance(accountName);
    }

    void SpinCallback (string transactionId) {
        print("Transaction callback: " + transactionId);

        ShowBalance(accountName);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
