// Daniel Elkoni

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Threading.Tasks;

// This file is dedicated to handling all api interaction

public class APIHandler : MonoBehaviour
{


    public UpgradeManager upm;

    public List<Unicorn> unisList = new List<Unicorn>();

    public GameObject loginPage;

    public GameObject structs;

    public RawImage tempImage;

    public string apiKey;
    public string usrEmail;
    public string password;
    public string usrId;
    public string token;
    public string refreshToken;
    public bool loggedIn;

    public GameObject emailInput;
    public GameObject passInput;
    public TextMeshProUGUI errorText;

    RestClient client = new RestClient("https://api.strandedunicorns.com/");

    void Start()
    {
        apiKey = "cFAXu2uSHEkqLdbTTyLM7Ge25tXJoNoM";
        if (PlayerPrefs.GetInt("logged") == 1)
        {
            loggedIn = true;
            loginPage.SetActive(false);
            RefreshToken(PlayerPrefs.GetString("refresh"));
            GetUnis();
        }
        else
        {
            loggedIn = false;
            loginPage.SetActive(true);
            structs.SetActive(false);
        }
    }

    public void LogOrRef()
    {
        int temp = PlayerPrefs.GetInt("logged");
        if (temp == 0)
        {
            RefreshToken(refreshToken);
        }

    }

    public void RefreshToken(string refToken)
    {
        var request = new RestRequest("auth/refresh-token", Method.Put);
        request.AddHeader("x-api-key", apiKey);
        request.AddHeader("authorization", refToken);
        request.AddHeader("Origin", "idlestaking.strandedverse.com");
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Accept", "application/json");
        var _params = new
        {
            refresh = refToken,
            fields = new string[] { "refresh", "token" }
        };
        var sparams = JsonConvert.SerializeObject(_params);
        request.AddParameter("application/json", sparams, ParameterType.RequestBody);
        RestResponse response = client.Execute(request);

        JObject _response = JObject.Parse(response.Content);

        refreshToken = _response["user"]["refresh"].ToString();
        token = _response["user"]["token"].ToString();
    }

    // Functionality for logging in, using input boxes in game
    public void CheckCreds()
    {
        usrEmail = emailInput.GetComponent<TMP_InputField>().text;
        password = passInput.GetComponent<TMP_InputField>().text;

        if (usrEmail.Contains("@") != true)
        {
            errorText.text = "Not a valid email address";
            emailInput.GetComponent<TMP_InputField>().text = "";
            passInput.GetComponent<TMP_InputField>().text = "";
        }
        else if (CheckTaken(usrEmail) == false)
        {
            errorText.text = "No user with this email";
            emailInput.GetComponent<TMP_InputField>().text = "";
            passInput.GetComponent<TMP_InputField>().text = "";
        }
        else
        {
            LoginUser();
        }
    }

    // Functionality to actually login a user with error handling
    public void LoginUser()
    {
        var request = new RestRequest("auth/login", Method.Post);
        request.AddHeader("x-api-key", apiKey);
        request.AddHeader("Origin", "idlestaking.strandedverse.com");
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Accept", "application/json");
        var _params = new
        {
            email = usrEmail,
            password = password,
            fields = new[] { "id", "token", "refresh" }
        };
        var serializedParams = JsonConvert.SerializeObject(_params);
        request.AddParameter("application/json", serializedParams, ParameterType.RequestBody);
        RestResponse response = client.Execute(request);
        string content = response.Content;
        JObject _response = JObject.Parse(content);
        JToken error = _response["error"];
        if (error.ToString() != "")
        {
            errorText.text = error["message"].ToString();
            emailInput.GetComponent<TMP_InputField>().text = "";
            passInput.GetComponent<TMP_InputField>().text = "";
        }
        else
        {
            JToken user = _response["user"];
            usrId = user["id"].ToString();
            token = user["token"].ToString();
            refreshToken = user["refresh"].ToString();
            emailInput.GetComponent<TMP_InputField>().text = "";
            passInput.GetComponent<TMP_InputField>().text = "";
            errorText.text = "";

            var request2 = new RestRequest($"user/{usrId}/cryos", Method.Post);
            request2.AddHeader("x-api-key", apiKey);
            request2.AddHeader("authorization", token);
            request2.AddHeader("Origin", "idlestaking.strandedverse.com");
            request2.AddHeader("Content-Type", "application/json");
            request2.AddHeader("Accept", "application/json");

            var params2 = new
            {
                fields = new[] { "total_count" }
            };

            var serializedParams2 = JsonConvert.SerializeObject(params2);
            request2.AddParameter("application/json", serializedParams2, ParameterType.RequestBody);
            RestResponse response2 = client.Execute(request2);
            string content2 = response2.Content;
            JObject _response2 = JObject.Parse(content2);

            if ((int)_response2["total_count"] > 0)
            {
                loginPage.SetActive(false);
                loggedIn = true;
                PlayerPrefs.SetInt("logged", 1);
            }
            else
            {
                emailInput.GetComponent<TMP_InputField>().text = "";
                passInput.GetComponent<TMP_InputField>().text = "";
                errorText.text = "No Cryos found";
            }
        }
        structs.SetActive(true);
        GetUnis();
    }


    public bool CheckTaken(string email)
    {
        var request = new RestRequest($"auth/user-taken?email={email}", Method.Get);
        request.AddHeader("x-api-key", apiKey);
        request.AddHeader("Origin", "idlestaking.strandedverse.com");
        request.AddHeader("Accept", "application/json");
        RestResponse response = client.Execute(request);

        JObject _response = JObject.Parse(response.Content);
        return (bool)_response["user_taken"];
    }

    // Dust being a primary currency outside of the game, this allows the users actual account to gain dust from the game
    public void SendDust(float amount)
    {
        var request = new RestRequest($"/user/{usrId}/receive-dust", Method.Post);
        request.AddHeader("x-api-key", apiKey);
        request.AddHeader("authorization", token);
        request.AddHeader("Origin", "idlestaking.strandedverse.com");
        request.AddHeader("Accept", "application/json");
        var _params = new
        {
            amount = amount,
            game = "Miner"
        };
        var serializedParams = JsonConvert.SerializeObject(_params);
        request.AddParameter("application/json", serializedParams, ParameterType.RequestBody);
        RestResponse response = client.Execute(request);
    }

    // Staking/nft functionality integrated, characters could be "staked" to boost game rewards
    public void StakeUni(Unicorn uni, string location)
    {
        if (uni.IsStakeAble == true)
        {
            var request = new RestRequest($"unicorn/{uni.UniID}/stake", Method.Post);
            request.AddHeader("x-api-key", apiKey);
            request.AddHeader("authorization", token);
            request.AddHeader("Origin", "idlestaking.strandedverse.com");
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");

            var _params = new
            {
                task = location,
                fields = new[] {"staked", "startedAt"}
            };
            var serializedParams = JsonConvert.SerializeObject(_params);
            request.AddParameter("application/json", serializedParams, ParameterType.RequestBody);
            RestResponse response = client.Execute(request);

            uni.IsStaked = true;
            unisList[uni.SpotInList].IsStaked = true;

            if (location == "quarters")
            {
                uni.IsStakeAble = true;
            }
            else
            {
                uni.IsStakeAble = false;
            }
        }
        else
        {
            uni.IsStaked = false;
            unisList[uni.SpotInList].IsStaked = false;
        }
    }

    // "Destake" functionality, allowing to unlock characters for a debuff
    public void DeStakeUni(Unicorn uni)
    {
        var request = new RestRequest($"unicorn/{uni.UniID}/destake", Method.Post);
        request.AddHeader("x-api-key", apiKey);
        request.AddHeader("authorization", token);
        request.AddHeader("Origin", "idlestaking.strandedverse.com");
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Content-Type", "application/json");
        RestResponse response = client.Execute(request);
        uni.IsStaked = false;
    }

    // Getting characters from the api that are owned by player
    public void GetUnis()
    {
        List<Unicorn> tempUnis = new List<Unicorn>();

        var request = new RestRequest($"user/{usrId}/unicorns", Method.Post);
        request.AddHeader("x-api-key", apiKey);
        request.AddHeader("authorization", token);
        request.AddHeader("Origin", "idlestaking.strandedverse.com");
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Accept", "application/json");
        var _params = new
        {
            fields = new string[] { "cooldown" ,"id", "name", "pegasus_level", "task", "image_url", "staked", "unstaked", "count", "traits", "trait_count", "trait_health"}
        };
        var serializedParams = JsonConvert.SerializeObject(_params);
        request.AddParameter("application/json", serializedParams, ParameterType.RequestBody);
        RestResponse response = client.Execute(request);
        string content = response.Content;
        JObject json = JObject.Parse(content);

        Debug.Log(json.ToString());

        JToken stakedUnis = json["staked"]["items"];
        Debug.Log(stakedUnis.ToString());
        JToken unstakedUnis = json["unstaked"]["items"];
        Debug.Log(unstakedUnis.ToString());
        for (int i = 0; i < (int)json["staked"]["count"]; i++)
        {
            JToken uni = stakedUnis[i];
            string id = uni["id"].ToString();
            string name = uni["name"].ToString();
            int type = 0;
            bool tasked;
            bool cooldown;
            int taskLoc;

            switch (uni["pegasus_level"].ToString())
            {
                case "stepOne":
                    type = 1;
                    break;
                case "stepTwo":
                    type = 2;
                    break;
                case "stepThree":
                    type = 3;
                    break;
            }
            if (uni["task"] != null)
            {
                tasked = true;
                taskLoc = -1;
                switch (uni["task"].ToString())
                {
                    case "scrapyard_one":
                        taskLoc = 0;
                        break;
                    case "scrapyard_two":
                        taskLoc = 1;
                        break;
                    case "factory_one":
                        taskLoc = 2;
                        break;
                    case "factory_two":
                        taskLoc = 3;
                        break;
                    case "refinery_one":
                        taskLoc = 4;
                        break;
                    case "refinery_two":
                        taskLoc = 5;
                        break;
                    case "storage_one":
                        taskLoc = 6;
                        break;
                    case "storage_two":
                        taskLoc = 7;
                        break;
                    case "shipping_one":
                        taskLoc = 8;
                        break;
                    case "shipping_two":
                        taskLoc = 9;
                        break;
                    case "power_one":
                        taskLoc = 10;
                        break;
                    case "power_two":
                        taskLoc = 11;
                        break;
                    case "quarters":
                        taskLoc = 12;
                        break;
                }
            }
            else
            {
                taskLoc = -1;
                tasked = false;
            }
            if (uni["cooldown"] != null)
            {
                if (uni["cooldown"].ToString() == "true")
                {
                    cooldown = true;
                }
                else
                {
                    cooldown = false;
                }
            }
            else
            {
                cooldown = false;
            }

            tempUnis.Add(new Unicorn(name, type, tasked, true, cooldown, true, id, i));
            
            if (taskLoc > -1)
            {
                tempUnis[i].TaskLoc = taskLoc;
            }
        }
        for (int i = 0; i < (int)json["unstaked"]["count"]; i++)
        {
            JToken uni = unstakedUnis[i];
            string id = uni["id"].ToString();
            string name = uni["name"].ToString();
            int type = 0;
            bool cooldown;
            bool isStakeAble;

            switch (uni["pegasus_level"].ToString())
            {
                case "stepOne":
                    type = 1;
                    break;
                case "stepTwo":
                    type = 2;
                    break;
                case "stepThree":
                    type = 3;
                    break;
            }
            
            if (uni["cooldown"] != null)
            {
                if (uni["cooldown"].ToString() == "true")
                {
                    cooldown = true;
                }
                else
                {
                    cooldown = false;
                }
            }
            else
            {
                cooldown = false;
            }

            int traitHealth = 0;
            for (int j = 0; j < (int)uni["trait_count"]; j++)
            {
                if (uni["traits"][j]["trait_health"] != null)
                {
                    traitHealth += (int)uni["traits"][j]["trait_health"];
                }
            }
            if (traitHealth > 0)
            {
                isStakeAble = true;
            }
            else
            {
                isStakeAble = false;
            }

            tempUnis.Add(new Unicorn(name, type, false, false, cooldown, isStakeAble, id, i));
        }
        unisList = tempUnis;
    }

    // Same as staking a character, used to assign "tasks" and a more intuitive function name
    public void TaskUni(string uniTaskLoc, Unicorn uni)
    {
        StakeUni(uni, uniTaskLoc);
    }

    // Same as destaking with additional functionality to update the client side game
    public void DeTaskUni(Unicorn uni)
    {
        StakeUni(uni, "quarters");
        uni.IsTasked = false;
        unisList[uni.SpotInList] = uni;
    }
}
