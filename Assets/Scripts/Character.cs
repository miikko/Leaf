using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
	
	public int HealthPoints;
    public int MaxHp;
	public bool Friendly;
	public string Weapon;
	public bool Interactable;
    public int spawnLength = 3;
    public bool isBoss;

    public Vector3 spawnPoint;
    GameObject obj;
    GameObject pc;
    Player playerChar;
    Animator anim;
	AttackCube attackCube;
	Character playableChar;

	// Use this for initialization
	void Start () {
        obj = gameObject;
        anim = GetComponent<Animator>();
        playerChar = GetComponent<Player>();
        pc = GameObject.Find("PlayerCharacter");
        if (!playerChar)
        {
			attackCube = GameObject.Find("Enemy1/AttackBox").GetComponent<AttackCube>();
			playableChar = pc.GetComponent<Character>();
        }
        if (playerChar)
        {
            spawnPoint = new Vector3(16, -5.7f, 0);
            pc.transform.position = spawnPoint;
        }
	}

    // Update is called once per frame

	void Update () {
		if (HealthPoints <= 0) {
            if (!this.playerChar)
            {
                NpcDie(this.obj);
            }
            else if (this.playerChar && PlayerInput.isAlive)
            {
                PlayerDie();
            }
         }

	}

	public void ChangeHP(int howMuch) {
        int temp = HealthPoints;
		if (HealthPoints + howMuch <= MaxHp) {
			HealthPoints += howMuch;
            if (temp > HealthPoints) // if char lost health, play animation
            {
                anim.SetTrigger("Hurt");
            }
		} else if (HealthPoints + howMuch > MaxHp) {
			HealthPoints = MaxHp;
		}
	}

	public void ChangeMaxHP(int howMuch) {
		MaxHp += howMuch;
		if (MaxHp < HealthPoints) {
			HealthPoints = MaxHp;
		}
	}

    
    public void NpcDie(GameObject obj)
    {
        Destroy(obj, 1f);
    }

    public void PlayerDie()
    {
        PlayerInput.isAlive = false;
        playerChar.SetDirectionalInput(new Vector2(0, 0));
        anim.SetTrigger("Dead");
        Invoke("Respawn", spawnLength);
    }

    public void Respawn()
    {
        
        pc.transform.position = spawnPoint;
        anim.ResetTrigger("Dead");
        anim.SetTrigger("Alive");
        anim.SetTrigger("Idle");
        ChangeHP(MaxHp);
        PlayerInput.isAlive = true;
    }
    
    public void Attack()
    {
		if (playerChar) {
			anim.Play("Attack");
		} else {
			anim.SetTrigger("EnemyAttack");
		}
    }

	public void doDamage() {
		if (attackCube.inRange) {
			playableChar.ChangeHP(-1);
		}
	}
}
