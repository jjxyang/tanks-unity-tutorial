﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPackScript : MonoBehaviour {

    public LayerMask m_TankMask;                      // used to only affect the player layer (only tanks), no other layer
    //public AudioSource m_ExplosionAudio;            // TODO: add "health regain" sound effect + maybe particles??
    public float m_HealPower = 25f;                   // amount of health healed
    public float m_MaxLifeTime = 7f;                  // packs should go away after a while

	// Use this for initialization
	void Start () {
        // get rid of pack after max lifetime expires
        Destroy(gameObject, m_MaxLifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // called whenever pack intersects with a collider
        // TODO: figure out how to get health pack to only collide with tanks
        Rigidbody targetRigidbody = other.GetComponent<Rigidbody>();
        if (targetRigidbody)
        {
            TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();
            if (targetHealth)
            {
                // heal the tank
                targetHealth.RegainHealth(m_HealPower);
            }
            // remove game object
            Destroy(gameObject);
        }
    }
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (new Vector3 (45, 50, 0) * Time.deltaTime);
	}
}
