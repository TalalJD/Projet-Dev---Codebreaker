using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
#if UNITY_EDITOR
using UnityEditor; // pour AssetDatabase.LoadAssetAtPath en mode Éditeur
#endif

public class EnnemiTakeDamage
{
    GameObject _playerGO;
    Player _player;

    GameObject _enemyGO;
    StaticEnnemy _enemy;

    [SetUp]
    public void Setup()
    {
        // --- PLAYER ---
        _playerGO = new GameObject("Player");
        _playerGO.tag = "Player";

        var prb = _playerGO.AddComponent<Rigidbody2D>();
        prb.bodyType = RigidbodyType2D.Kinematic;
        _playerGO.AddComponent<BoxCollider2D>();

        _player = _playerGO.AddComponent<Player>();
        _player.WeaponInventory = new List<ScriptableObject>();

        // WeaponHolder requis par Player.EquipWeapon()
        var holderGO = new GameObject("WeaponHolder");
        holderGO.transform.SetParent(_playerGO.transform, false);
        _player.WeaponHolder = holderGO.transform;

        // Charger l'arme (WeaponInfo)
        WeaponInfo gunInfo = null;


            gunInfo = AssetDatabase.LoadAssetAtPath<WeaponInfo>("Assets/Weapons/Gun.asset");

        Assert.IsNotNull(gunInfo);

        // Ajoute et équipe (utilise la logique du Player)
        _player.AddItemToInventory(gunInfo);
        _player.CycleWeaponInventory();

        Assert.IsNotNull(_player.SelectedWeaponInfo);
        Assert.IsNotNull(_player.SelectedWeapon);
        Assert.AreEqual(_player.WeaponHolder, _player.SelectedWeapon.transform.parent);

        // --- ENNEMI ---

        // Charger le prefab StaticEnnemy

        var enemyPrefab = AssetDatabase.LoadAssetAtPath<StaticEnnemy>("Assets/Prefabs/EnnemyPrefab/EnnemyStatic.prefab");


        Assert.IsNotNull(enemyPrefab);

        _enemy = Object.Instantiate(enemyPrefab);
        _enemyGO = _enemy.gameObject;
        _enemyGO.transform.position = new Vector2(3f, 0f);

        // Sécurise la physique si le prefab n'en a pas
        if (!_enemyGO.TryGetComponent<Rigidbody2D>(out var erb))
        {
            erb = _enemyGO.AddComponent<Rigidbody2D>();
        }
        erb.bodyType = RigidbodyType2D.Dynamic;
        erb.gravityScale = 0f;
        erb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        if (!_enemyGO.TryGetComponent<Collider2D>(out var ecol))
        {
            ecol = _enemyGO.AddComponent<CircleCollider2D>();
        }
        ecol.isTrigger = false;

        _enemy.SendMessage("Start");

        // Désactive le comportement de l'ennemi pour éviter ses propres tirs
        _enemy.enabled = false;

        Assert.Greater(_enemy.CurrentHealth, 0f);
    }

    [TearDown]
    public void Teardown()
    {
        if (_playerGO) Object.DestroyImmediate(_playerGO);
        if (_enemyGO) Object.DestroyImmediate(_enemyGO);
    }

    [UnityTest]
    public IEnumerator Enemy_LosesHealth_When_BaseGun_Shoots()
    {
        // Positionne le joueur à gauche pour que la balle aille sur l'ennemi
        _playerGO.transform.position = new Vector2(-3f, 0f);

        float hpBefore = _enemy.CurrentHealth;

        // Simule un tir 
        _player.SelectedWeapon.Attack();

        // Attends la collision de la bullet avec l'ennemi
        float timeout = 2.0f;
        float t = 0f;
        bool damaged = false;

        while (t < timeout)
        {
            t += Time.deltaTime;

            if (_enemy == null) { damaged = true; break; } 
            if (_enemy.CurrentHealth < hpBefore) { damaged = true; break; }

            yield return null;
        }

        Assert.IsTrue(damaged);
    }

}
