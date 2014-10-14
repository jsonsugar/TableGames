﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ivony.Data;
using System.Web.Http.ModelBinding;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Runtime.Remoting.Messaging;

namespace Ivony.TableGame.WebHost
{


  /// <summary>
  /// 玩家宿主，登陆用户在系统中的宿主对象
  /// </summary>
  public partial class PlayerHost : IPlayerHost
  {


    public Guid Guid
    {
      get;
      private set;
    }



    private PlayerHost( Guid id )
    {
      Guid = id;
      SyncRoot = new object();
      _console = new PlayerConsole( this );
    }


    public static PlayerHost CreatePlayerHost()
    {

      lock ( _sync )
      {

        var host = new PlayerHost( Guid.NewGuid() );
        hosts.Add( host.Guid, host );
        return host;

      }
    }

    private static object _sync = new object();
    private static Hashtable hosts = new Hashtable();


    public static PlayerHost GetPlayerHost( Guid userId )
    {
      lock ( _sync )
      {
        return hosts[userId] as PlayerHost;
      }

    }


    private PlayerConsole _console;

    /// <summary>
    /// 获取玩家控制台，用于给玩家显示消息
    /// </summary>
    public PlayerConsoleBase Console
    {
      get { return _console; }
    }



    /// <summary>
    /// 若已经加入某个游戏，则获取游戏中的玩家对象
    /// </summary>
    public GamePlayer Player { get; private set; }


    public GamePlayer GetPlayer()
    {
      return Player;
    }

    /// <summary>
    /// 玩家已经加入游戏
    /// </summary>
    /// <param name="player"></param>
    public void JoinedGame( GamePlayer player )
    {

      lock ( _sync )
      {
        if ( Player != null )
          throw new InvalidOperationException( "玩家当前已经在另一个游戏，无法加入游戏" );

        Player = player;
      }
    }


    /// <summary>
    /// 玩家已经从游戏中释放
    /// </summary>
    public void QuitGame()
    {
      lock ( _sync )
      {
        Player.Release();
        Player = null;
      }
    }



    /// <summary>
    /// 获取是否正在游戏
    /// </summary>
    public bool Gaming
    {
      get { return Player != null; }
    }




    protected object SyncRoot { get; private set; }


    private class PlayerConsole : PlayerConsoleBase
    {

      public PlayerHost PlayerHost { get; private set; }

      public PlayerConsole( PlayerHost host )
      {
        PlayerHost = host;
      }

      public override void WriteMessage( GameMessage message )
      {
        PlayerHost._messages.Add( message );
      }

      public override async Task<string> ReadLine( string prompt, CancellationToken token )
      {
        return await WaitResponse( prompt, token ).ConfigureAwait( false );
      }

      private Task<string> WaitResponse( string prompt, CancellationToken token )
      {
        return Responding.CreateResponding( PlayerHost, prompt, token ).RespondingTask;
      }


      public override Task<IOption> Choose( string prompt, IOption[] options, CancellationToken token )
      {
        return ChooseResponding.CreateResponding( PlayerHost, prompt, options, token ).RespondingTask;
      }

    }




    private List<GameMessage> _messages = new List<GameMessage>();


    private int index = 0;

    internal void SetMessageIndex( int messageIndex )
    {
      index = messageIndex;
    }


    internal int LastMesageIndex
    {
      get;
      private set;
    }

    public GameMessage[] GetMessages()
    {
      lock ( SyncRoot )
      {
        LastMesageIndex = _messages.Count;
        if ( index > LastMesageIndex )
          return new GameMessage[0];

        return _messages.GetRange( index, LastMesageIndex - index ).ToArray();
      }
    }



    public override string ToString()
    {
      return Guid.ToString();
    }



    public OptionEntity[] GetOptions()
    {
      lock ( SyncRoot )
      {
        var responding = _responding as ChooseResponding;

        if ( responding == null )
          return null;

        return responding.Options.Select( item => new OptionEntity( item ) ).ToArray();
      }
    }
  }



}
