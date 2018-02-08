using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    public float m_StartingHealth = 100f;          
    public Slider m_Slider;                        // slider UI element
    public Image m_FillImage;                      
    public Color m_FullHealthColor = Color.green;  
    public Color m_ZeroHealthColor = Color.red;    
    public GameObject m_ExplosionPrefab;           // allows explosion to be instantiated at runtime
    
    private AudioSource m_ExplosionAudio;          // references to audio source on instantiated explosion prefab
    private ParticleSystem m_ExplosionParticles;   
    private float m_CurrentHealth;  
    private bool m_Dead;            


    private void Awake()
    {
        // spawn an explosion prefab, get its components
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();
        // deactivate the explosion particles for now;
        //    performance-wise, better to instantiate system now than create/destroy, which can cause perf spikes bc garbage collectors
        //    not always simplest tho, bc sometimes don't know how many you'll need (object pooling is a thing tho)
        m_ExplosionParticles.gameObject.SetActive(false);
    }


    private void OnEnable()
    {
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;
        SetHealthUI();
    }


    public void TakeDamage(float amount)
    {
        // public function so the shells can deal the damage
        // Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.
        m_CurrentHealth -= amount;
        SetHealthUI();
        
        if (m_CurrentHealth <= 0f && !m_Dead)
        {
            // check if we just died
            OnDeath();
        }
    }

    public void RegainHealth(float amount)
    {
        // public function so health packs can heal damage
        m_CurrentHealth += amount;

        // ensure player cannot heal past max health
        if (m_CurrentHealth > m_StartingHealth)
        {
            m_CurrentHealth = m_StartingHealth;
        }

        SetHealthUI();
    }


    private void SetHealthUI()
    {
        // Adjust the value and colour of the slider.
        m_Slider.value = m_CurrentHealth;
        // lerp --> linear interpolation between 2 colors, based on fraction of health remaining
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
    }


    private void OnDeath()
    {
        // Play the effects for the death of the tank and deactivate it.
        m_Dead = true;

        // move particles to correct spot, activate them, and play them
        m_ExplosionParticles.transform.position = transform.position;
        m_ExplosionParticles.gameObject.SetActive(true);
        m_ExplosionParticles.Play();
        
        // play explosion sound
        m_ExplosionAudio.Play();

        // deactivate tank object
        gameObject.SetActive(false);
    }
}