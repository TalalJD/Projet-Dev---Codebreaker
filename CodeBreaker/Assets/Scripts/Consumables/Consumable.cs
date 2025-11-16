using UnityEngine;

public abstract class Consumable : MonoBehaviour
{
    [SerializeField] protected ConsumableInfo _consumableInfo;
    [SerializeField] protected SpriteRenderer sprite;

    public abstract void UseConsumable();

    public virtual void AssignConsumable(ConsumableInfo consumableInfo)
    {
        _consumableInfo = consumableInfo;
        sprite.sprite = _consumableInfo.consumableSprite;

    }
}
