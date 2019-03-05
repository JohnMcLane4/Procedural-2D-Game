using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {

    public Sprite dmgSprite;
    public int hp = 4;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();    
    }

    public void DamageWall(int loss)
    {
        spriteRenderer.sprite = dmgSprite;      //set sprite to damaged sprite for visual feedback
        hp -= loss;                             //subract loss from walls hp, if hp <= 0...
        if (hp <= 0)
            gameObject.SetActive(false);
    }    
}
