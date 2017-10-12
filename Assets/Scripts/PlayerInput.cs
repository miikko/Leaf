using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour
{
    Scene scene;
    public Vector2 directionalInput;
    Player player;
    public  static bool isAlive = true;
    AttackCube attacker;

    void Start()
    {
        scene = SceneManager.GetActiveScene();
        Debug.Log(scene.name);
        player = GetComponent<Player>();
        attacker = GameObject.Find("PlayerCharacter/AttackBox").GetComponent<AttackCube>();
    }

    void Update()
    {
        if (isAlive)
        {
            directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            player.SetDirectionalInput(directionalInput);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                player.OnJumpInputDown();
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                player.OnJumpInputUp();
            }
            if (Input.GetKeyDown(KeyCode.J))
            {
                attacker.Attack();
            }
            if (Input.GetKeyDown(KeyCode.P))
            {

            }
        }
    }
    void OnGUI()
    {
        Event e = Event.current;
        if (e.type == EventType.KeyDown && e.control && e.keyCode == KeyCode.L)
        {
            // CTRL + L
            if (scene.name == "mainscene")
            {
                SceneManager.LoadScene(sceneName: "demoscene");
            }
            else if (scene.name == "demoscene")
            {
                SceneManager.LoadScene(sceneName: "mainscene");
            }
        }
    }
}