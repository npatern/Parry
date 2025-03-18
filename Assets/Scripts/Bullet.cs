using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Damage = 1;
    public ParticleSystem ParticlesToKill;
    public bool DestroyAfterDamage = true;
    public GameObject ParticlesToSpawn;
    public StatusController ownerStatusController;
    private Rigidbody rb;
    private Vector3 ShooterPosition;

    public float SoundRange = 2;
    public Sound.TYPES soundType = Sound.TYPES.danger;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        ShooterPosition = transform.position;
    }
    private void OnTriggerEnter(Collider collision)
    {
        bool bounced = false;
        if (collision.gameObject.GetComponent<StatusController>() != null)
        {
            StatusController statusController = collision.gameObject.GetComponent<StatusController>();
            bounced = !statusController.TryTakeDamage(GetComponent<StatusController>(),Damage,ShooterPosition);
        }
        if (bounced)
        {
            rb.velocity *= -1;
            transform.localScale *= 1.5f;
            Damage *= 2;
        }
        else if (DestroyAfterDamage)
            DestroyBullet();
    }
    public void DestroyBullet()
    {
        Debug.Log("bullet destroyed");
        if (ParticlesToKill != null)
        {
            ParticlesToKill.transform.parent = null;
            ParticlesToKill.Stop();
        }
        if (ParticlesToSpawn != null)
        {
            Instantiate(ParticlesToSpawn, transform.position, transform.rotation, GameController.Instance.transform);
        }
        if (GetComponent<StatusController>() != null) if (GetComponent<StatusController>().IsDeaf()) return;
        
        Sound sound = new Sound(GetComponent<StatusController>(), SoundRange, soundType);
        Sounds.MakeSound(sound);
        Destroy(gameObject);
    }
}