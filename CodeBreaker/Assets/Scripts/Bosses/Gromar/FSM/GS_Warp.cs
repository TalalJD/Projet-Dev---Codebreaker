using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GS_Warp : GromarState
{
    SpriteRenderer[] sprites;
    public override void OnEnter()
    {
        sprites = gromar.GetComponentsInChildren<SpriteRenderer>();
        WarpPosition();
        Machine.Set<GS_AttackState>(); return;
    }
   public void WarpPosition()
    {
        DisableOrEnableSprites(false);
        gromar.transform.position = gromar.GetRandomMapPoint().position;
        DisableOrEnableSprites(true);
        
    }

    public void DisableOrEnableSprites(bool choice)
    {
        foreach (var sprite in sprites)
        {
            sprite.enabled = choice;
        }
    }
}
