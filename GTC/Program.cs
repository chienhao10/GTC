using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Constants;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Notifications;
using EloBuddy.SDK.Rendering;
using EloBuddy.SDK.Spells;
using EloBuddy.SDK.ThirdParty;
using EloBuddy.SDK.ThirdParty.Glide;
using EloBuddy.SDK.Utils;
using GTC.Champions;

namespace GTC //God Tier Champions
{
	class Program
	{
		public static void Main(string[] args)
		{
			Loading.OnLoadingComplete += Loading_OnLoadingComplete;
		}

		static void Loading_OnLoadingComplete(EventArgs args)
		{
			if (Player.Instance.ChampionName == "Ashe")
			{
				new Ashe();
			}
			if (Player.Instance.ChampionName == "Ryze")
			{
				new Ryze();
			}
		}
	}
}
