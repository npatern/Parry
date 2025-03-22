using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Damage = 1;
    public bool DestroyAfterDamage = true;

    public ParticleSystem ParticlesToKill;
    public GameObject ParticlesToSpawn;
    public StatusController ownerStatusController;
    private Rigidbody rb;
    private Vector3 ShooterPosition;
    [SerializeReference]
    public ItemWeaponWrapper item;
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
    void HandleHit(Collider collision)
    {
        bool bounced = false;
        if (collision.gameObject.GetComponent<StatusController>() != null)
        {
            StatusController statusController = collision.gameObject.GetComponent<StatusController>();
             
            bounced = !statusController.TryTakeDamage(Damage, GetComponent<StatusController>(), ShooterPosition);
        }
        if (bounced)
        {
            rb.velocity *= -1;
            //transform.localScale *= 1.5f;
            Damage *= 2;
        }
        else if (DestroyAfterDamage)
        {
            DestroyBullet();
        }

        else
        {
            if (item != null)
            {
                //item.CurrentWeaponObject.transform.parent = null;
                //item.MakePickable();
                //item.pickable.GetComponent<Rigidbody>().velocity = rb.velocity;
                if (this.TryGetComponent<StatusController>(out StatusController status)){
                    Sound sound = new Sound(status, SoundRange, soundType);
                    Sounds.MakeSound(sound);
                }
                Destroy(GetComponent<Bullet>());
            }
            else
            {
                Destroy(this);
            }
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        HandleHit(collision);
    }
    private void OnColliderEnter(Collider collision)
    {
        HandleHit(collision);
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