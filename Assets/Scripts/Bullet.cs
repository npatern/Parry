using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float obsoleteDamage = 1;
    public DamageEffects damage;
    public float multiplier = 1;
    public bool DestroyAfterDamage = true;
    public bool isDamaging = true;
    public ParticleSystem ParticlesToKill;
    public GameObject ParticlesToSpawn;
    public StatusController ownerStatusController;
    private Rigidbody rb;
    private Vector3 ShooterPosition;
    [SerializeReference]
    public ItemWeaponWrapper item;
    public float SoundRange = 2;
    public Sound.TYPES soundType = Sound.TYPES.danger;
    public GameObject destroyObject = null;
    public bool primed = false;
    public float primeTimer = 1;
    public LayerMask layerMask;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        layerMask = LayerMask.GetMask("Blockout", "Default", "Entity", "Damage");
        //ParticlesToKill = GetComponentInChildren<ParticleSystem>();
    }
    private void Start()
    {
        ShooterPosition = transform.position;
    }
    private void FixedUpdate()
    {
        if (primed)
        {
            primeTimer -= Time.fixedDeltaTime;
            if (primeTimer <= 0)
                DestroyBullet();
        }
        if (item == null || isDamaging==false) return;
         
        if (item.CastDamage(damage, multiplier))
        {
            isDamaging = false;
            RemoveParticles();
            StopBeingBullet();
        }
    }
    void StopBeingBullet()
    {
        if (!primed)
        {
            Destroy(GetComponent<Bullet>());
        }
    }
    void HandleBulletHit(Collider collision)
    {
        bool bounced = false;
        if (collision.gameObject.GetComponent<StatusController>() != null)
        {
            StatusController statusController = collision.gameObject.GetComponent<StatusController>();
             
            bounced = !statusController.TryTakeDamage(damage,  ShooterPosition, multiplier, GetComponent<StatusController>());
        }
        else
        {
            Debug.DrawRay(transform.position, Vector3.up, Color.blue, .5f);
            Sound sound = new Sound(transform.position, SoundRange, soundType);
            Sounds.MakeSound(sound);
        }
        if (bounced)
        {
            rb.velocity *= -1;
            //transform.localScale *= 1.5f;
            multiplier *= 2;
        }
        else if (DestroyAfterDamage)
        {
            DestroyBullet();
        }
        else
        {
            RemoveParticles();
            Destroy(this);
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        if ((layerMask.value & (1 << collision.gameObject.layer)) != 0)
        {

        }
        else
        {
            return;
        }
        if (item == null)
        {
            HandleBulletHit(collision);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if ((layerMask.value & (1 << collision.gameObject.layer)) != 0)
        {

        }
        else
        {
            return;
        }

        if (item == null) 
            HandleBulletHit(collision.collider);
        else
        {
            Debug.DrawRay(transform.position, Vector3.up, Color.red,.5f);
            Sound sound = new Sound(transform.position, SoundRange, soundType);
            Sounds.MakeSound(sound);
            isDamaging = false;
            RemoveParticles();
            StopBeingBullet();
        }      
    }
    public void RemoveParticles()
    {
        if (ParticlesToKill != null)
        {
            ParticlesToKill.transform.parent = null;
            ParticlesToKill.Stop();
            Destroy(ParticlesToKill.gameObject, 10);
        }
    }
    public void DestroyBullet()
    {
        Debug.Log("bullet destroyed");
        RemoveParticles();
        if (ParticlesToSpawn != null)
        {
            Instantiate(ParticlesToSpawn, transform.position, transform.rotation, GameController.Instance.transform);
        }
        if (GetComponent<StatusController>() != null) if (GetComponent<StatusController>().IsDeaf()) return;

        Debug.DrawRay(transform.position, Vector3.left, Color.green, .5f);
        
        Sound sound = new Sound( transform.position, SoundRange, soundType);
        Sounds.MakeSound(sound);
        if (destroyObject != null) Instantiate(destroyObject, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}