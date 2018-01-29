using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask;                      // used to only affect the player layer (only tanks), no other layer
    public ParticleSystem m_ExplosionParticles;       
    public AudioSource m_ExplosionAudio;              
    public float m_MaxDamage = 100f;                  // can never hit tank exactly bc of collider, but will do lots of damage
    public float m_ExplosionForce = 1000f;            // explosive force
    public float m_MaxLifeTime = 2f;                  // shells should go away after 2 seconds
    public float m_ExplosionRadius = 5f;              // how far from shell will explosions be felt


    private void Start()
    {
        // get rid of shell after max lifetime expires
        Destroy(gameObject, m_MaxLifeTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        // called whenever shell intersects with a collider
        // Find all the tanks in an area around the shell and damage them.
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);

        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();
            if (!targetRigidbody)
                continue;

            // force added, that emanates from the shell's position in specified radius
            targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

            // this `targetRigidbody` is component of a game object, so we can get the object's other components still, e.g. its script
            TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();
            if (!targetHealth)
                continue;
            // cause tank to take damage
            float damage = CalculateDamage(targetRigidbody.position);
            targetHealth.TakeDamage(damage);
        }

        // play animation + audio
        m_ExplosionParticles.transform.parent = null; // detach particles from parent
        m_ExplosionParticles.Play();
        m_ExplosionAudio.Play();

        // remove particple system, then the game object itself
        Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.duration);    // TODO: what is 'main.duration'?
        Destroy(gameObject);
    }


    private float CalculateDamage(Vector3 targetPosition)
    {
        // Calculate the amount of damage a target should take based on it's position.
        Vector3 explosionToTarget = targetPosition - transform.position;
        float explosionDistance = explosionToTarget.magnitude;
        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;
        float damage = relativeDistance * m_MaxDamage;
        return Mathf.Max(0f, damage);    // min 0 dmg for edge case where tank's collider is inside overlap sphere BUT position isn't
    }
}