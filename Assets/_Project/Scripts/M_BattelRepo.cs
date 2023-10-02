using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_BattelRepo : MonoBehaviour
{
    public static M_BattelRepo instance;
    public GameObject vfx_DonutExplosive;
    public GameObject vfx_PlayerDeath;
    public Sprite[] playerPortrait;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
