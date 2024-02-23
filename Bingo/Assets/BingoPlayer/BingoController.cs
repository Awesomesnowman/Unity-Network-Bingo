using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NETWORK_ENGINE;

public class BingoController : NetworkComponent
{
    public GameObject[] bingoPlayers;
    public bool isPlaying = false;
    public List<int> rolledNumber;
    public int rand;
    public override void HandleMessage(string flag, string value)
    {
        
    }

    public override void NetworkedStart()
    {
    }

    public override IEnumerator SlowUpdate()
    {
        while(MyCore.IsConnected)
        {
            while(IsServer)
            {
                if(rolledNumber.Count == 0)
                    CheckReady();
                if (isPlaying == true)
                {
                    rand = ((int)Random.Range(1, 75));
                    for (int i=0;i<bingoPlayers.Length;i++)
                    {
                        BingoPlayer temp = bingoPlayers[i].GetComponent<BingoPlayer>();
                        
                        if (!rolledNumber.Contains(rand))
                        {
                            for (int j = 0; j < 25; j++)
                            {
                                if (rand == temp.cardNums[j])
                                {
                                    temp.IsDirty = true;
                                    temp.cardHits[j] = true;
                                    temp.cardTiles[j].GetComponent<Text>().color = Color.blue;
                                    break;
                                }
                            }
                            CheckWin(temp);
                        }                        
                    }
                    if (!rolledNumber.Contains(rand))
                        rolledNumber.Add(rand);
                }
                    yield return new WaitForSeconds(MyId.UpdateFrequency);                    
            }
            yield return new WaitForSeconds(MyId.UpdateFrequency);
        }
    }
    public void CheckWin(BingoPlayer temp)
    {
        for (int j = 0; j < 25; j += 5)
        {
            if (temp.cardHits[j] && temp.cardHits[j + 1] && temp.cardHits[j + 2] && temp.cardHits[j + 3] && temp.cardHits[j + 4])
            {
                isPlaying = false;
                StartCoroutine(End());
                temp.StartCoroutine(temp.End());
                temp.SendUpdate("WIN", "");
            }

        }
        for (int j = 0; j < 5; j++)
        {
            if (temp.cardHits[j] && temp.cardHits[j + 5] && temp.cardHits[j + 10] && temp.cardHits[j + 15] && temp.cardHits[j + 20])
            {
                isPlaying = false;
                StartCoroutine(End());
                temp.StartCoroutine(temp.End());

                temp.SendUpdate("WIN", "");
            }
        }
        if (temp.cardHits[0] && temp.cardHits[6] && temp.cardHits[12] && temp.cardHits[18] && temp.cardHits[24])
        {
            temp.StartCoroutine(temp.End());

            isPlaying = false;
            StartCoroutine(End());
            temp.SendUpdate("WIN", "");
        }
        if (temp.cardHits[4] && temp.cardHits[8] && temp.cardHits[12] && temp.cardHits[16] && temp.cardHits[20])
        {
            temp.StartCoroutine(temp.End());

            isPlaying = false;
            StartCoroutine(End());
            temp.SendUpdate("WIN", "");
        }
    }
    public IEnumerator End()
    {

        yield return new WaitForSeconds(5);
        StartCoroutine(MyCore.DisconnectServer());
    }
    public void CheckReady()
    {
        bingoPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject card in bingoPlayers)
        {
            if (card.GetComponent<BingoPlayer>().ready)
            {
                isPlaying = true;
            }
            else
            {
                isPlaying = false;
                break;
            }

        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
