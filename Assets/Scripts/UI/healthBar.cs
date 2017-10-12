using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class healthBar : MonoBehaviour
{

    Character player;
    private int newHealth;
    private int prevHealth;
    GameObject heart;

    List<GameObject> heartclones = new List<GameObject>();


    void Start()
    {
        
        player = GameObject.Find("PlayerCharacter").GetComponent<Character>();
        heart = GameObject.Find("Heart");
        newHealth = player.HealthPoints;
        
        HealthBarUpdate();


    }


    void Update()
    {
        newHealth = player.HealthPoints;
        if (newHealth!=prevHealth)
        {
            HealthBarUpdate();
        }

    }
    void HealthBarUpdate()
    {

        foreach (GameObject clone in heartclones)
        {
            GameObject.Destroy(clone);
        }

		for (int i = 0; i < newHealth; i++)
        {
            GameObject heart1 = Instantiate(heart, transform.position, transform.rotation) as GameObject;
            heart1.transform.SetParent(GameObject.Find("Health").transform);
            heart1.transform.position += Vector3.right * i * 32;
            heartclones.Add(heart1);
            //Debug.Log("heart spawned");
            prevHealth = newHealth;
            
        }
    }
}
