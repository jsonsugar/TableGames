﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.TableGame
{
  /// <summary>
  /// 定义玩家宿主，负责与客户端直接通信的宿主对象
  /// </summary>
  public interface IPlayerHost
  {


    /// <summary>
    /// 玩家 ID
    /// </summary>
    Guid ID { get; }


    /// <summary>
    /// 玩家名称
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 玩家控制台，用于接收和发送消息
    /// </summary>
    PlayerConsoleBase Console { get; }


    /// <summary>
    /// 当玩家加入了某个游戏后，将调用此方法
    /// </summary>
    /// <param name="player">在游戏中的玩家对象</param>
    void OnJoinedGame( GamePlayerBase player );


    /// <summary>
    /// 指示玩家应立即退出当前游戏
    /// </summary>
    bool TryQuitGame();


    /// <summary>
    /// 获取当前在游戏的玩家对象（如果有的话）
    /// </summary>
    /// <returns>玩家对象</returns>
    GamePlayerBase Player { get; }



    /// <summary>
    /// 判断客户端是否支持某个特性
    /// </summary>
    /// <param name="feature">要判断是否支持的特性</param>
    bool Support( string feature );

  }
}
