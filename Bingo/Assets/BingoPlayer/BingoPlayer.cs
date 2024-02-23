using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NETWORK_ENGINE;
public class BingoPlayer : NetworkComponent
{
    public string[] cardStrings;
    public int[] cardNums = new int[25];
    public bool[] cardHits = new bool[25];
    public string pName = "";
    public bool ready = false;
    public GameObject[] cardTiles;
    public InputField NameField;
    public Toggle readyButton;
    public GameObject numberTile;
    public GameObject bingoCard;
    public GameObject bingoController;


    public override void HandleMessage(string flag, string value)
    {
        if (flag == "NAME")
        {
            if (IsServer)
            {
                    pName = value;
                    NameField.text = value;
                    SendUpdate("NAME", value);                
            }
            if (IsClient)
            {
                pName = value;
                NameField.text = value;

                if (IsLocalPlayer && !(value.Equals("")))
                {
                    readyButton.interactable = true;
                }
            }
        }
        if(flag == "READY")
        {
            if(IsServer)
            {
                ready = bool.Parse(value);
                bingoController.GetComponent<BingoController>().CheckReady();
                readyButton.isOn = ready;
                bingoController.GetComponent<BingoController>().bingoPlayers = GameObject.FindGameObjectsWithTag("Bingo");
                SendUpdate("READY", value);
            }
            if (IsClient)
            {
                ready = bool.Parse(value);
                readyButton.isOn = ready;
                
            }
        }
        if(flag == "GENERATE")
        {
            
            if (IsServer)
            {
                int multiplier = -1;
                int t;
                for (int i = 0; i < 25; i++)
                {
                    if (i % 5 == 0)
                        multiplier++;
                        t = ((int)Random.Range(1, 15)) + 15 * multiplier;      
                    
                    for(int j = multiplier*5;j< multiplier*5+5;j++)
                    {
                        if(t == cardNums[i])
                        {
                            t = ((int)Random.Range(1, 15)) + 15 * multiplier;
                            j = multiplier*5-1;
                        }                       
                    }
                    cardNums[i] = t;
                    cardTiles[i] = GameObject.Instantiate(numberTile);
                    cardTiles[i].GetComponent<Text>().text = cardNums[i].ToString();
                    cardTiles[i].transform.SetParent(bingoCard.transform);
                }
                SendUpdate("GENERATE", string.Join(',', cardNums));                
            }
            if(IsClient)
            {
                cardStrings = value.Split(',');
                for(int i =0;i<25;i++)
                {
                    cardNums[i] = int.Parse(cardStrings[i]);

                    if(cardTiles[24] == null)
                    {
                        cardTiles[i] = GameObject.Instantiate(numberTile);
                        cardTiles[i].transform.SetParent(bingoCard.transform);
                    }
                    cardTiles[i].GetComponent<Text>().text = cardStrings[i];
                }                
            }
        }
        if(flag == "WIN")
        {
            bingoCard.GetComponent<Image>().color = Color.blue;
        }
        if(flag == "HITS")
        {
            if(IsClient)
            {
                string[] hitList = value.Split(',');
                for (int i=0;i< 25;i++)
                {
                    cardHits[i] = bool.Parse(hitList[i]);
                    if(cardHits[i])
                        cardTiles[i].GetComponent<Text>().color = Color.blue;
                }           
            }
        }     
    }
    public override void NetworkedStart()
    {
        IsDirty = true;
        cardTiles = new GameObject[25];
        bingoController = GameObject.Find("BingoMaster");
        if (IsLocalPlayer)
            SendCommand("GENERATE", "");
        if (!IsLocalPlayer)
        {
            NameField.interactable = false;
            readyButton.interactable = false;
        }        
    }

    public void UI_SetName(string s)
    {
        IsDirty = true;
        SendCommand("NAME", s);
    }

    public void UI_Ready(bool ready)
    {
        IsDirty = true;
        SendCommand("READY", ready.ToString());
    }
    public override IEnumerator SlowUpdate()
    {
        while(IsConnected)
        {

            while(IsServer)
            {
                if (IsDirty)
                {
                    SendUpdate("NAME", pName);
                    SendUpdate("READY", ready.ToString());
                    SendUpdate("GENERATE", string.Join(',', cardNums));
                    SendUpdate("HITS", string.Join(',', cardHits));
                    IsDirty = false;
                }
                yield return new WaitForSeconds(.1f);
                
            }
           
            yield return new WaitForSeconds(.1f);
        }
    }
    public IEnumerator End()
    {

        yield return new WaitForSeconds(5);
        StartCoroutine(MyCore.DisconnectServer());
    }
    // Start is called before the first frame update
    void Start()
    {
        
        GameObject temp = GameObject.Find("GameCanvas");
        if(temp == null)
        {
            throw new System.Exception("ERROR: Could not find game canvas on the scene.");
        }
        else
        {
            this.transform.SetParent(temp.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
