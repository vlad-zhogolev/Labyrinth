using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.UI;

public class ConnectedPlayer : MonoBehaviour
{

    public RectTransform prefab;
    public RectTransform content;

    public void UpdateContent(Player[] players)
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        if (players != null)
        {
            GameObject instance;
            foreach (var player in players)
            {
                instance = GameObject.Instantiate(prefab.gameObject) as GameObject;
                instance.transform.SetParent(content, false);
                instance.GetComponentInChildren<Text>().text = player.NickName;
            }
        }
    }
}
