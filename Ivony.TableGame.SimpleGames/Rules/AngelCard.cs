﻿using Ivony.TableGame.CardGames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.TableGame.SimpleGames.Rules
{
  public class AngelCard : SimpleGameCard
  {


    public override async Task UseCard( SimpleGamePlayer user, SimpleGamePlayer target )
    {
      user.SetEffect( new CardEffect() );
      AnnounceSpecialCardUsed( user );

      user.PlayerHost.WriteMessage( "天使保护你，下一次攻击将变成治疗" );
    }

    public override string Name
    {
      get { return "天使"; }
    }

    public override string Description
    {
      get { return "天使卡牌让你下次遭受的攻击成为治疗，加上相应的 HP"; }
    }


    public class CardEffect : SimpleGameEffect, IBlessEffect
    {
      public override string Name
      {
        get { return "天使"; }
      }

      public override string Description
      {
        get { return "天使将使得对你的攻击转化为治疗"; }
      }



      protected override async Task OnAttack( AttackEvent attackEvent )
      {
        var player = attackEvent.RecipientPlayer;
        attackEvent.AnnounceAttackIneffective();
        player.HealthPoint += attackEvent.AttackPoint;
        player.PlayerHost.WriteWarningMessage( "天使保护你，攻击变为治疗效果，增加 {0} 点 HP", attackEvent.AttackPoint, player.HealthPoint );

        player.Effects.RemoveEffect( this );
        attackEvent.Handled = true;
      }


      public override string ToString()
      {
        return "A";
      }

    }

  }
}
