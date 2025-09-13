using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SpeedBuffTest
{
    private GameObject _playerGameObject;
    private Player _playerScript;
    private PhysicsInfo _playerPhysicsInfo;
    private GameObject _speedBuffGameObject;
    private SpeedBuff _speedBuffScript;
    private BoxCollider2D _playerCollider;

    [SetUp]
    public void Setup()
    {
        // Cree le joueur
        _playerGameObject = new GameObject("Player");
        _playerCollider = _playerGameObject.AddComponent<BoxCollider2D>();
        _playerGameObject.tag = "Player";
        _playerScript = _playerGameObject.AddComponent<Player>();

        _playerPhysicsInfo = ScriptableObject.CreateInstance<PhysicsInfo>();
        _playerScript.PhysicsInfo = _playerPhysicsInfo;
        _playerPhysicsInfo.MaxSpeed = 10f;
        _playerPhysicsInfo.Acceleration = 20f;

        // Cree le buff de vitesse
        _speedBuffGameObject = new GameObject("SpeedBuff");
        var buffCollider = _speedBuffGameObject.AddComponent<BoxCollider2D>();
        buffCollider.isTrigger = true;
        _speedBuffScript = _speedBuffGameObject.AddComponent<SpeedBuff>();
        _speedBuffScript.duration = 0.5f;

        // assigner les serialized fields manuellement
        _speedBuffScript.GetType().GetField("_boxCollider", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(_speedBuffScript, buffCollider);

        _speedBuffScript.GetType().GetField("_player", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(_speedBuffScript, _playerScript);
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(_playerGameObject);
        if (_speedBuffGameObject != null) Object.DestroyImmediate(_speedBuffGameObject);
    }

    [UnityTest]
    public IEnumerator SpeedBuff_AppliedViaDirectTrigger()
    {
        // stoquage des valeures initiale pour la verification
        float initialMaxSpeed = _playerScript.PhysicsInfo.MaxSpeed;
        float initialAcceleration = _playerScript.PhysicsInfo.Acceleration;

        // appel directe de la methode ontrigger du buff de vitesse
        _speedBuffScript.OnTriggerEnter2D(_playerCollider);

        // attendre 1 frame avant de continuer
        yield return null;

        // verifier si le buff s'applique
        Assert.Greater(_playerScript.PhysicsInfo.MaxSpeed, initialMaxSpeed, "la vitesse max n'a pas augmenter");
        Assert.Greater(_playerScript.PhysicsInfo.Acceleration, initialAcceleration, "l'acceleration n'a pas augmenter");

        // attendre pour la fin du buff + petite marge d'erreur
        yield return new WaitForSeconds(_speedBuffScript.duration + 0.1f);

        // les valeures doivent se reinisialiser
        Assert.AreEqual(initialMaxSpeed, _playerScript.PhysicsInfo.MaxSpeed, "la vitesse max ne c'est pas reinisialiser");
        Assert.AreEqual(initialAcceleration, _playerScript.PhysicsInfo.Acceleration, "l'acceleration ne c'est pas reinisialiser ");

        // verifier si le gameobject du buff c'est bien detruit
        Assert.IsTrue(_speedBuffGameObject == null || _speedBuffGameObject.Equals(null), "le game object speedbuff ne c'est pas detruit");
    }
}
