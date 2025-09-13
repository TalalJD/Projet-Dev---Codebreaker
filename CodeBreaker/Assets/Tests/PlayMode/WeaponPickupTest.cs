using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class WeaponPickUpTest
{
    private GameObject _playerGameObject;
    private Player _playerScript;
    private GameObject _pickupGameObject;
    private ItemPickUp _pickupScript;
    private WeaponInfo _weaponInfo;

    [SetUp]
    public void Setup()
    {
        // Cree le joueur
        _playerGameObject = new GameObject("Player");
        _playerGameObject.tag = "Player"; //bien s'assurer d'avoir le tag pour la logique de pickup
        _playerScript = _playerGameObject.AddComponent<Player>();
        _playerScript.Inventory = new System.Collections.Generic.List<ScriptableObject>(); //initialiser la liste d'inventaire

        //ajouter un rigid body et un boxCollider au joueur pour le fonctionnement du test
        var rb = _playerGameObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        _playerGameObject.AddComponent<BoxCollider2D>();

        // Cree l'objet de pickup
        _pickupGameObject = new GameObject("WeaponPickUp");
        var collider = _pickupGameObject.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        _pickupScript = _pickupGameObject.AddComponent<ItemPickUp>();

        // Assigner le weaponInfo scriptable object au pickup
        _weaponInfo = ScriptableObject.CreateInstance<WeaponInfo>();
        _pickupScript.objectRecup = _weaponInfo;

        // placer le joueur et le pickup a la meme position
        _playerGameObject.transform.position = Vector3.zero;
        _pickupGameObject.transform.position = Vector3.zero;
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(_playerGameObject);
        Object.DestroyImmediate(_pickupGameObject);
        Object.DestroyImmediate(_weaponInfo);
    }

    [UnityTest]
    public IEnumerator PlayerCanPickUpWeapon()
    {
        // simuler manuellement le trigger enter
        var playerCollider = _playerGameObject.GetComponent<Collider2D>();
        _pickupScript.OnTriggerEnter2D(playerCollider);

        // Simuler l'activation de la methode pickup manuellement au lieu d'avoir un input
        var pickupMethod = typeof(ItemPickUp)
            .GetMethod("Pickup", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        pickupMethod.Invoke(_pickupScript, null);

        yield return null; // attendre une frame pour s'assurer que la methode a rouler

        // verifie si le scriptable object est bien dans la liste
        Assert.Contains(_weaponInfo, _playerScript.Inventory, "l'arme n'est pas dans l'inventaire");

        // verifier que l'objet de pickup s'est bien detruit
        Assert.IsTrue(_pickupGameObject == null || _pickupGameObject.Equals(null), "le pickup object ne s'est pas detruit");
    }
}
