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

namespace GTC.Champions
{
	public class Ashe
	{
		Menu menu;
		
		void CreateMenu()
		{
			menu = MainMenu.AddMenu("GTC Ashe", "gtcashe");
			menu.Add("key", new KeyBind("Combo Key", false, KeyBind.BindTypes.HoldActive, ' '));
		}
		
		void Combo()
		{
			if (menu["key"].Cast<KeyBind>().CurrentValue)
			{
				Orbwalker.DisableAttacking = true;
				Orbwalker.DisableMovement = true;
				if (Game.Time * 1000 > lastaa + (Player.Instance.AttackDelay * 1000) - (Game.Ping*2.15f))
				{
					if (Target != null)
					{
						Player.IssueOrder(GameObjectOrder.AttackUnit, Target);
					}
					else if (Game.Time * 1000 > lastaa + (Player.Instance.AttackCastDelay * 1000) - (Game.Ping/2.15f) + (Player.Instance.AttackSpeedMod*10))
					{
						Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
					}
				}
				else if (Game.Time * 1000 > lastaa + (Player.Instance.AttackCastDelay * 1000) - (Game.Ping/2.15f) + (Player.Instance.AttackSpeedMod*10))
				{
					Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
					if (qready)
					{
						Player.CastSpell(SpellSlot.Q);
					}
				}
			}
			else
			{
				Orbwalker.DisableAttacking = false;
				Orbwalker.DisableMovement = false;
			}
		}

		void Game_OnTick(EventArgs args)
		{
			Combo();
		}
		
		bool qready;
		
		float lastaa;
		
		public Ashe()
		{
			CreateMenu();
			Game.OnTick += Game_OnTick;
			GameObject.OnCreate += GameObject_OnCreate;
			Obj_AI_Base.OnBasicAttack += Obj_AI_Base_OnBasicAttack;
		}

		void Obj_AI_Base_OnBasicAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
		{
			if (sender.IsMe)
			{
				lastaa = Game.Time * 1000;
			}
		}
		
		void GameObject_OnCreate(GameObject sender, EventArgs args)
		{
			if (sender.Name == "Ashe_Base_Q_ready.troy")
			{
				qready = true;
			}
			if (sender.Name == "Ashe_Base_Q_buf.troy")
			{
				lastaa = 0f;
				qready = false;
			}
		}
		
		AttackableUnit Target
		{
			get
			{
				int health = int.MaxValue;
				AttackableUnit t = null;
				foreach (var target in EntityManager.Heroes.Enemies.Where(x => x.IsValidTarget(Player.Instance.AttackRange + Player.Instance.BoundingRadius + x.BoundingRadius)))
				{
					int hp = (int)(target.Health * (100 / (100 + target.Armor)));
					if (hp < health)
					{
						health = hp;
						t = target;
					}
				}
				return t;
			}
		}
	}
}
