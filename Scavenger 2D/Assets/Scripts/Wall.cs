using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {

    public Sprite dmgSprite;
    public int hp = 4;

    public AudioClip chopSound1;
    public AudioClip chopSound2;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();    
    }

    public void DamageWall(int loss)
    {
        SoundManager.instance.RandomizeSFX(chopSound1, chopSound2);
        spriteRenderer.sprite = dmgSprite;      //set sprite to damaged sprite for visual feedback
        hp -= loss;                             //subract loss from walls hp, if hp <= 0...
        if (hp <= 0)
            gameObject.SetActive(false);
    }    
}
