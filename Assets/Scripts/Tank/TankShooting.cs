using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;       
    public Rigidbody m_Shell;            
    public Transform m_FireTransform;    
    public Slider m_AimSlider;           
    public AudioSource m_ShootingAudio;  
    public AudioClip m_ChargingClip;     
    public AudioClip m_FireClip;         
    public float m_MinLaunchForce = 15f; 
    public float m_MaxLaunchForce = 30f; 
    public float m_MaxChargeTime = 0.75f;   // time it takes to charge up

    private string m_FireButton;            // input button, stored based on player number
    private float m_CurrentLaunchForce;     // how powerful the shot is
    private float m_ChargeSpeed;            // how fast the arrow should charge
    private bool m_Fired;                   // only shoot once per fire button


    private void OnEnable()
    {
        // reset to minimum, aren't charging a shot when you die
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
    }


    private void Start()
    {
        // calculate fire button, based on player # (for player 1, it's space)
        m_FireButton = "Fire" + m_PlayerNumber;
        // how fast the arrow should charge
        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
    }


    private void Update()
    {
        // Track the current state of the fire button and make decisions based on the current launch force.
        m_AimSlider.value = m_MinLaunchForce;

        if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
        {
            // at max charge, not yet fired
            m_CurrentLaunchForce = m_MaxLaunchForce;
            Fire();
        }
        else if (Input.GetButtonDown(m_FireButton))
        {
            // have we pressed fire for the first time?
            m_Fired = false;
            m_CurrentLaunchForce = m_MinLaunchForce;

            m_ShootingAudio.clip = m_ChargingClip;
            m_ShootingAudio.Play();
        }
        else if (Input.GetButton(m_FireButton) && !m_Fired)
        {
            // holding fire button, not yet fired
            m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;

            m_AimSlider.value = m_CurrentLaunchForce;
        }
        else if (Input.GetButtonUp(m_FireButton) && !m_Fired)
        {
            // released fire button, haven't fired yet
            Fire();
        }
    }


    private void Fire()
    {
        // Instantiate and launch the shell.
        m_Fired = true;
        // `as Rigidbody` specifies this object as a Rigidbody, not just an object
        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;
        // launch shell in fire transform's forward direction, with magnitude of current launch force
        shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;

        // play the firing audio clip
        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        // once again, reset launch force just to be safe
        m_CurrentLaunchForce = m_MinLaunchForce;
    }
}