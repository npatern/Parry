using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "Weapon Item ", menuName = "ScriptableObjects/Items", order = 2)]
public class ItemWeaponScriptableObject : ScriptableObject
{
    //later will be moved to Item>>
    public string ItemName = "Weapon";
    public string ID = "weapon";
    public string Description = "You can hack'n slash with it.";
    public bool Stackable = false;
    public bool Big = false;
    public GameObject weaponObject;
    public Image icon;
    public bool emptyhanded = false;
    //weapon specific>>
    public AttackPattern attackPattern;


    public float AttackDistance = 3f;
    public float Damage = 10;
    public float BulletDamage = 10;
    public Bullet bullet;
    public int stack;

    public AttackScriptableObject LightAttack;
    public AttackScriptableObject HeavyAttack;
    public AttackScriptableObject Parry;
    public AttackScriptableObject Equip;
    public AttackScriptableObject Dequip;
    public AttackScriptableObject Stunned;
    public AttackScriptableObject Attacked;
    public AttackScriptableObject Throw;
}
[System.Serializable]
public class ItemWeaponWrapper
{
    public string name;
    public Sprite icon;
    public ItemWeaponScriptableObject itemType;
    public string ItemName;
    public string ID;
    public string Description;
    public bool Stackable;
    public int stack = 1;
    public bool Big;
    public GameObject weaponObject;
    public Transform CurrentWeaponObject = null;
    public Pickable pickable = null; 
    //weapon specific>>
    public AttackPattern attackPattern;
    public float AttackDistance = 3f;
    public float Damage = 10;
    public float BulletDamage = 10;
    public Bullet Bullet;

    public bool emptyhanded = false;
    public ItemWeaponWrapper(ItemWeaponScriptableObject scriptableObject)
    {
        itemType = scriptableObject;
        
        Damage = scriptableObject.Damage;
        ItemName = scriptableObject.ItemName;
        
        name = scriptableObject.ItemName;
        ID = scriptableObject.ID;
        Description = scriptableObject.Description;
        Stackable = scriptableObject.Stackable;
        stack = scriptableObject.stack;
        Big = scriptableObject.Big;
        attackPattern = scriptableObject.attackPattern;
        weaponObject = scriptableObject.weaponObject;
        AttackDistance = scriptableObject.AttackDistance;
        Damage = scriptableObject.Damage;
        BulletDamage = scriptableObject.BulletDamage;
        Bullet = scriptableObject.bullet;
         emptyhanded = scriptableObject.emptyhanded;
        RefreshIcon();
    }
    public Transform SpawnWeaponObjectAsCurrentObject(Transform parentTransform = null)
    {
        CurrentWeaponObject = SpawnWeaponObject(parentTransform);
        return CurrentWeaponObject;
    }
    public Transform SpawnWeaponObject(Transform parentTransform = null)
    {
        return GameObject.Instantiate(weaponObject, parentTransform).transform;
    }
    public void OBSOLETEMakePickable()
    {
        if (CurrentWeaponObject == null) SpawnWeaponObjectAsCurrentObject();

        CurrentWeaponObject.GetComponent<Collider>().enabled = true;
        CurrentWeaponObject.GetComponent<Collider>().isTrigger = false;
        pickable = GameObject.Instantiate(GameController.Instance.ListOfAssets.PickableTemplate, CurrentWeaponObject.position, CurrentWeaponObject.rotation).GetComponent<Pickable>();
        CurrentWeaponObject.transform.parent = pickable.transform;
        pickable.weaponObject = CurrentWeaponObject.gameObject;
        pickable.weaponWrapper = this;
    }
    
    public void OBSOLETERemovePickable(Transform newParent = null)
    {
        if (CurrentWeaponObject == null) return;
        if (pickable == null) return;
        CurrentWeaponObject.GetComponent<Collider>().enabled = false;
        CurrentWeaponObject.GetComponent<Collider>().isTrigger = true;
        CurrentWeaponObject.transform.parent = newParent;

        GameObject.Destroy(pickable.gameObject);
    }
    public void RemovePickable(Transform newParent = null, bool removeRigidBody = false)
    {
        if (CurrentWeaponObject == null) return;
        if (pickable == null) return;
        
        CurrentWeaponObject.transform.parent = newParent;
        //GameObject.Destroy(CurrentWeaponObject.GetComponent<Rigidbody>());
        GameObject.Destroy(pickable.trigger);
        GameObject.Destroy(pickable);
        CurrentWeaponObject.GetComponent<Collider>().enabled = false;
        CurrentWeaponObject.GetComponent<Collider>().isTrigger = true;
        
        //CurrentWeaponObject.GetComponent<Rigidbody>().isKinematic = true;
        if (removeRigidBody) RemoveRigidBody();
    }
    public void AddRigidBody()
    {
        if (CurrentWeaponObject == null) return;
        if (CurrentWeaponObject.GetComponent<Rigidbody>() != null) return;
        Rigidbody rb = CurrentWeaponObject.gameObject.AddComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }
    public void RemoveRigidBody()
    {
        if (CurrentWeaponObject == null) return;
        GameObject.Destroy(CurrentWeaponObject.GetComponent<Rigidbody>());
    }
    public Pickable MakePickable()
    {
        if (CurrentWeaponObject == null) SpawnWeaponObjectAsCurrentObject();
        if (CurrentWeaponObject.GetComponent<Pickable>() != null) return CurrentWeaponObject.GetComponent<Pickable>();

        CurrentWeaponObject.GetComponent<Collider>().enabled = true;
        CurrentWeaponObject.GetComponent<Collider>().isTrigger = false;
        pickable = CurrentWeaponObject.gameObject.AddComponent<Pickable>();
        CurrentWeaponObject.transform.parent = null;
        pickable.weaponObject = CurrentWeaponObject.gameObject;
        pickable.weaponWrapper = this;
        AddRigidBody();
        return pickable;
    }
    
    
    public void DestroyPhysicalPresence()
    {
        if (CurrentWeaponObject != null) CurrentWeaponObject.parent = null;
        if (pickable != null) GameObject.Destroy(pickable.gameObject);
        if (CurrentWeaponObject != null) GameObject.Destroy(CurrentWeaponObject.gameObject);
    }
    public void MergeToMe(ItemWeaponWrapper otherObject)
    {
        stack += otherObject.stack;
        otherObject.DestroyPhysicalPresence();
    }
    public void RefreshIcon()
    {

        Transform model = SpawnWeaponObject(IconMaker.Instance.transform);
        model.localPosition = Vector3.zero;
        float size = Vector3.Magnitude( model.GetComponent<WeaponModel>().StartPoint.localPosition);
        icon = GetIcon(size);
        model.gameObject.SetActive(false);
        GameObject.Destroy(model.gameObject);
    }
    public Sprite GetIcon(float size = 1f)
    {
        Camera cam = IconMaker.Instance.iconCamera;
        cam.orthographicSize = size+.3f;
        int resX = Screen.width;
        int resY = Screen.height;

        int clipX = 0;
        int clipY = 0;
        if (resX > resY) clipX = resX - resY;
        else if (resX < resY) clipY = resY - resX;

        //Initialize
        Texture2D texture = new Texture2D(resX-clipX, resY-clipY, TextureFormat.RGBA32, false);
        RenderTexture rt = new RenderTexture(resX, resY, 24);
        
        cam.targetTexture = rt;
        RenderTexture.active = rt;

        //Grab icon and make texture
        cam.Render();
        texture.ReadPixels(new Rect(clipX/2,clipY/2,resX,resY),0,0);
        texture.Apply();

        //cleanup:
        cam.targetTexture = null;
        RenderTexture.active = null;
        GameObject.Destroy(rt);

        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0,0));
    }
}
