Rabbit [![Build status](https://ci.appveyor.com/api/projects/status/6fxlb8bkqp18cg3c/branch/master)](https://ci.appveyor.com/project/Decagon/rabbit/branch/master) [![Issue Stats](http://www.issuestats.com/github/decagon/rabbit/badge/pr)](http://www.issuestats.com/github/decagon/rabbit) [![Issue Stats](http://www.issuestats.com/github/decagon/rabbit/badge/issue)](http://www.issuestats.com/github/decagon/rabbit)
======

Rabbit allows [Everybody Edits](http://everybodyedits.com) and PlayerIO developers to seamlessly integrate all supported types (Armor Games, Kongregate, Mousebreaker, Facebook, email, and standard authentication) into one login interface. 

Want to use Rabbit in your project? It's simple!

```csharp
Connection conn = new RabbitAuth().LogOn(EmailOrTokenOrUserName, RoomID, Password);
```



The [PlayerIOClient (v3.0.14)](https://gamesnet.yahoo.com/download/) is a dependency. 

###Wiki

Have more questions? Feel free to consult the [Wiki](https://github.com/Decagon/Rabbit/wiki).


###Bugs

If you find a bug in Rabbit, feel free to let me know in the GitHub issue tracker or, if you don't have an account, by decagongithub@gmail.com.

###NuGet
Rabbit is available on NuGet: [EE-Rabbit](http://www.nuget.org/packages/EE-Rabbit/).


###Credits
NuGet package icon courtesy of [jcapaldi on Flickr](https://flic.kr/p/cVkan9).

[Yonom](https://github.com/Yonom), author of [Cupcake](https://github.com/Yonom/CupCake), significantly [helped](https://github.com/Decagon/Rabbit/commits/master?author=Yonom)!

###PlayerIO

While Rabbit was specifically for the Everybody Edits game, it can be easily modified to support other types of PlayerIO-based games, by changing the game key in Config.cs and a few methods. For more information about integrating Rabbit into your game, visit the wiki.
