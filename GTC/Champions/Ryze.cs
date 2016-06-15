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
	public class Ryze
	{
		Menu menu;
		
		readonly Spell.Skillshot Q = new Spell.Skillshot(SpellSlot.Q, 900, SkillShotType.Linear, 250, 1400, 55) { AllowedCollisionCount = int.MaxValue, MinimumHitChance = HitChance.Low };
		
		readonly Spell.Skillshot Q2 = new Spell.Skillshot(SpellSlot.Q, 900, SkillShotType.Linear, 250, 1400, 70) { AllowedCollisionCount = 0, MinimumHitChance = HitChance.High };
		
		readonly Spell.Targeted W = new Spell.Targeted(SpellSlot.W, 600);
		
		readonly Spell.Targeted E = new Spell.Targeted(SpellSlot.E, 600);
		
		readonly Spell.Active R = new Spell.Active(SpellSlot.R, 600);
		
		float lastaa, num, lastpasmove;
		
		bool canq, passive;
		
		int stacks;
		
		public Ryze()
		{
			menu = MainMenu.AddMenu("GTC Ryze", "gtcryze");
			menu.Add("key", new KeyBind("Combo Key", false, KeyBind.BindTypes.HoldActive, ' '));
			menu.Add("qmana", new Slider("Auto Q min. % Mana", 50, 5));
			Game.OnTick += Game_OnTick;
			Obj_AI_Base.OnBasicAttack += Obj_AI_Base_OnBasicAttack;
			Obj_AI_Base.OnSpellCast += Obj_AI_Base_OnSpellCast;
			Spellbook.OnCastSpell += Spellbook_OnCastSpell;
			GameObject.OnCreate += GameObject_OnCreate;
			GameObject.OnDelete += GameObject_OnDelete;
		}
		
		void Game_OnTick(EventArgs args)
		{
			Combo();
		}
		
		void Obj_AI_Base_OnBasicAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
		{
			if (sender.IsMe)
			{
				lastaa = Game.Time * 1000;
			}
		}
		
		void Obj_AI_Base_OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
		{
			if (sender.IsMe)
			{
				if (args.Slot == SpellSlot.Q)
				{
					canq = false;
				}
				if (args.Slot == SpellSlot.W)
				{
					canq = true;
				}
				if (args.Slot == SpellSlot.E)
				{
					canq = true;
				}
			}
		}

		void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
		{
			if (sender.Owner.IsMe)
			{
				if (args.Slot == SpellSlot.R)
				{
					canq = true;
				}
			}
		}
		
		void GameObject_OnCreate(GameObject sender, EventArgs args)
		{
			if (sender.Name == "Ryze_Base_P_stack_01.troy")
			{
				stacks = 1;
			}
			if (sender.Name == "Ryze_Base_P_stack_02.troy")
			{
				stacks = 2;
				num = Game.Time * 1000;
			}
			if (sender.Name == "Ryze_Base_P_stack_03.troy")
			{
				stacks = 3;
				num = Game.Time * 1000;
			}
			if (sender.Name == "Ryze_Base_P_Stack_04.troy")
			{
				stacks = 4;
				num = Game.Time * 1000;
			}
			if (sender.Name == "Ryze_Base_P_Buf.troy")
			{
				canq = true;
				passive = true;
				num = Game.Time * 1000;
			}
		}
		
		void GameObject_OnDelete(GameObject sender, EventArgs args)
		{
			if (sender.Name == "Ryze_Base_P_stack_01.troy" || sender.Name == "Ryze_Base_P_stack_02.troy" || sender.Name == "Ryze_Base_P_stack_03.troy" || sender.Name == "Ryze_Base_P_Stack_04.troy")
			{
				if ((Game.Time * 1000) - num > 50)
				{
					stacks = 0;
				}
			}
			if (sender.Name == "Ryze_Base_P_Buf.troy")
			{
				passive = false;
			}
		}
		
		void Combo()
		{
			if (Q.IsReady() && Player.Instance.Mana > Player.Instance.MaxMana * (menu["qmana"].Cast<Slider>().CurrentValue / 100))
			{
				if (QTarget != null)
				{
					Q2.Cast(QTarget);
				}
			}
			//try
			//{
			if (menu["key"].Cast<KeyBind>().CurrentValue)
			{
				Orbwalker.DisableAttacking = true;
				Orbwalker.DisableMovement = true;
				if (passive)
				{
					if (Game.Time * 1000 > lastpasmove + 250)
					{
						Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
						lastpasmove = Game.Time * 1000;
					}
					if (canq)
					{
						if (QTarget != null && Q.IsReady())
						{
							Q.Cast(QTarget);
						}
					}
					else if (WERTarget != null)
					{
						if (R.IsReady())
						{
							R.Cast();
						}
						else if (W.IsReady())
						{
							W.Cast(WERTarget);
						}
						else if (E.IsReady())
						{
							E.Cast(WERTarget);
						}
					}
				}
				else
				{
					if (Game.Time * 1000 > lastaa + (Player.Instance.AttackDelay * 1000) - (Game.Ping * 2.15f))
					{
						if (Target != null && ((stacks < 3 && !W.IsReady() && !E.IsReady()) || (stacks > 2 && !Q.IsReady() && !W.IsReady() && !E.IsReady())))
						{
							Player.IssueOrder(GameObjectOrder.AttackUnit, Target);
						}
						else if (Game.Time * 1000 > lastaa + (Player.Instance.AttackCastDelay * 1000) - (Game.Ping / 2.15f) + (Player.Instance.AttackSpeedMod * 10))
						{
							Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
							if (Q.IsReady())
							{
								if (QTarget != null)
								{
									Q2.Cast(QTarget);
								}
							}
						}
					}
					else if (Game.Time * 1000 > lastaa + (Player.Instance.AttackCastDelay * 1000) - (Game.Ping / 2.15f) + (Player.Instance.AttackSpeedMod * 10))
					{
						Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
					}
					CastSpells();
				}
			}
			else
			{
				Orbwalker.DisableAttacking = false;
				Orbwalker.DisableMovement = false;
			}
			//}
			//catch(Exception e)
			//{
			//Chat.Print(e);
			//}
		}
		
		void CastSpells()
		{
			bool qready = Q.IsReady();
			bool wready = W.IsReady();
			bool eready = E.IsReady();
			bool rready = R.IsReady();
			if (stacks < 3)
			{
				if (WERTarget != null)
				{
					if (wready)
					{
						W.Cast(WERTarget);
						wready = false;
					}
					else if (eready)
					{
						E.Cast(WERTarget);
						eready = false;
					}
					else if (qready)
					{
						Q2.Cast(WERTarget);
						qready = false;
					}
				}
				else if (qready && QTarget != null)
				{
					Q2.Cast(QTarget);
					qready = false;
				}
			}
			if (stacks == 3)
			{
				if (WERTarget != null)
				{
					if (R.IsReady())
					{
						if (wready)
						{
							W.Cast(WERTarget);
							wready = false;
						}
						else if (eready)
						{
							E.Cast(WERTarget);
							eready = false;
						}
						else if (qready)
						{
							Q.Cast(WERTarget);
							qready = false;
						}
					}
					else if (wready)
					{
						if (rready)
						{
							R.Cast();
							rready = false;
						}
						else if (eready)
						{
							E.Cast(WERTarget);
							eready = false;
						}
						else if (qready)
						{
							Q.Cast(WERTarget);
							qready = false;
						}
						else
						{
							W.Cast(WERTarget);
							wready = false;
						}
					}
					else
					{
						if (eready)
						{
							E.Cast(WERTarget);
							eready = false;
						}
						else if (qready)
						{
							Q.Cast(WERTarget);
							qready = false;
						}
					}
				}
				else if (qready && QTarget != null)
				{
					Q2.Cast(QTarget);
					qready = false;
				}
			}
			if (stacks == 4)
			{
				if (WERTarget != null)
				{
					if (rready)
					{
						R.Cast();
						rready = false;
					}
					else if (wready)
					{
						W.Cast(WERTarget);
						wready = false;
					}
					else if (eready)
					{
						E.Cast(WERTarget);
						eready = false;
					}
					else if (qready)
					{
						Q.Cast(WERTarget);
						qready = false;
					}
				}
				else if (qready && QTarget != null)
				{
					Q2.Cast(QTarget);
					qready = false;
				}
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
		
		AIHeroClient QTarget
		{
			get
			{
				int health = int.MaxValue;
				AIHeroClient t = null;
				foreach (var target in EntityManager.Heroes.Enemies.Where(x => x.IsValidTarget(900)))
				{
					int hp = (int)(target.Health * (100 / (100 + target.MagicShield)));
					if (hp < health)
					{
						health = hp;
						t = target;
					}
				}
				return t;
			}
		}
		
		AIHeroClient WERTarget
		{
			get
			{
				int health = int.MaxValue;
				AIHeroClient t = null;
				foreach (var target in EntityManager.Heroes.Enemies.Where(x => x.IsValidTarget(600)))
				{
					int hp = (int)(target.Health * (100 / (100 + target.MagicShield)));
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
