﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.TableGame.Basics
{
  internal interface IBasicGame
  {

    CardDealer CardDealer { get; }


    void ReleasePlayer( GamePlayer player );
  }
}
