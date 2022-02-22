mergeInto(LibraryManager.library, {
    Setup: function() {
        // manually add below line to index.html line 108
        // window.unityInstance = unityInstance;

        unityInstance.SendMessage("Scene", "SetupCallback");
    },
    Login: function() {
        const popup = window.open(`${window.location.href}popup.html`, "WalletPopup", "height=800,width=600");

        window.addEventListener("message", event => {
            if (event.data.type === "loginSuccess") {
                unityInstance.SendMessage("Scene", "LoginCallback", event.data.accountName);
            } else if (event.data.type === "ready") {
                popup.postMessage({
                    type: "login"
                });
            }
        });
    },
    Spin: function(...args) {
        const landKey = UTF8ToString(args[0])
        const accountName = UTF8ToString(args[1])

        console.log("landKey", landKey);
        console.log("accountName", accountName);
        
        const popup = window.open(
            `${window.location.href}popup.html`,
            "WalletPopup",
            "height=800,width=600"
        );

        window.addEventListener("message", (event) => {
            if (event.data.type === "ready") {
                popup.postMessage({
                    type: "transaction",
                    contract: "gnocitylands",
                    action: "spin",
                    actor: accountName,
                    data: {
                        account: accountName,
                        land_key: landKey
                    }
                })
            } else if (event.data.type === "transactionSuccess") {
                unityInstance.SendMessage("Scene", "SpinCallback", event.data.transactionId);
            }
        });
    },
    GetBalance: async function(...args) {
        const accountName = UTF8ToString(args[0])

        print("GetBalance", accountName);

        const response = await fetch("https://wax.greymass.com/v1/chain/get_table_rows", {
            "body": `{"json":true,"code":"gnokentokens","scope":"${accountName}","table":"accounts"}`,
            "method": "POST",
            "headers": {
                'Accept': 'application/json',
                'Content-Type': 'application/json'                
            }
        });

        unityInstance.SendMessage("Scene", "GetBalanceCallback", JSON.stringify(await response.json()));
    }
})