using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class EnemyDamagesPlayer
{
    private GameObject _playerGO;
    private Player _player;

    private GameObject _enemyGO;
    private StaticEnnemy _enemy;

    [SetUp]
    public void Setup()
    {
        // -------- PLAYER --------
        _playerGO = new GameObject("Player");
        _playerGO.tag = "Player";

        var prb = _playerGO.AddComponent<Rigidbody2D>();
        prb.bodyType = RigidbodyType2D.Kinematic; 
        var pcol = _playerGO.AddComponent<BoxCollider2D>();
        pcol.isTrigger = false;

        _player = _playerGO.AddComponent<Player>();
        _playerGO.transform.position = new Vector2(-3f, 1.5f); // aligné sur la même hauteur que l’ennemi

        _player.SendMessage("Start"); // initialise currentHealth = maxHealth


        var enemyPrefab = AssetDatabase.LoadAssetAtPath<StaticEnnemy>("Assets/Prefabs/EnnemyPrefab/EnnemyStatic.prefab");

        Assert.IsNotNull(enemyPrefab);

        _enemy = Object.Instantiate(enemyPrefab);
        _enemyGO = _enemy.gameObject;
        _enemyGO.transform.position = new Vector2(3f, 1.5f); // face au joueur

        // Laisse l’ennemi actif pour qu’il tire réellement
        _enemy.enabled = true;

        // Vérifie que l’ennemi a bien ses assets assignés
        Assert.IsNotNull(_enemy._ennemyInfo);
        Assert.IsNotNull(_enemy);
        Assert.IsNotNull(_enemyGO);
    }

    [TearDown]
    public void Teardown()
    {
        if (_enemyGO) Object.DestroyImmediate(_enemyGO);
        if (_playerGO) Object.DestroyImmediate(_playerGO);
    }

    [UnityTest]
    public IEnumerator Enemy_Shoots_And_Player_Takes_Damage()
    {
        // HP initial du joueur
        int hpBefore = _player.currentHealth;
        Assert.Greater(hpBefore, 0);

        // On attend la durée suffisante pour que l’ennemi tire et que la bullet touche le joueur
        float timeout = 3.0f;
        float t = 0f;
        bool tookDamage = false;

        while (t < timeout)
        {
            t += Time.deltaTime;

            if (_player == null) break;
            if (_player.currentHealth < hpBefore)
            {
                tookDamage = true;
                break;
            }

            yield return null;
        }

        Assert.IsTrue(tookDamage);
    }
}
