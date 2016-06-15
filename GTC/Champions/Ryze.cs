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
		
		void CreateMenu()
		{
			menu = MainMenu.AddMenu("GTC Ryze", "gtcryze");
			menu.Add("key", new KeyBind("Combo Key", false, KeyBind.BindTypes.HoldActive, ' '));
		}
		
		float lastpasmove;
		
		void Combo()
		{
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
						if (Target != null)
						{
							Player.IssueOrder(GameObjectOrder.AttackUnit, Target);
						}
						else if (Game.Time * 1000 > lastaa + (Player.Instance.AttackCastDelay * 1000) - (Game.Ping / 2.15f) + (Player.Instance.AttackSpeedMod * 10))
						{
							Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
							if (Q2.IsReady() && QTarget != null)
							{
								Q2.Cast(QTarget);
							}
						}
					}
					else if (Game.Time * 1000 > lastaa + (Player.Instance.AttackCastDelay * 1000) - (Game.Ping / 2.15f) + (Player.Instance.AttackSpeedMod * 10))
					{
						Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
						if (stacks < 3)
						{
							if (WERTarget != null)
							{
								if (W.IsReady())
								{
									W.Cast(WERTarget);
								}
								else if (E.IsReady())
								{
									E.Cast(WERTarget);
								}
								else if (Q2.IsReady())
								{
									Q2.Cast(WERTarget);
								}
							}
							else if (Q2.IsReady() && QTarget != null)
							{
								Q2.Cast(QTarget);
							}
						}
						if (stacks == 3)
						{
							if (WERTarget != null)
							{
								if (R.IsReady())
								{
									if (W.IsReady())
									{
										W.Cast(WERTarget);
									}
									else if (E.IsReady())
									{
										E.Cast(WERTarget);
									}
									else if (Q.IsReady())
									{
										Q.Cast(WERTarget);
									}
								}
								else if (W.IsReady())
								{
									if (R.IsReady())
									{
										R.Cast();
									}
									else if (E.IsReady())
									{
										E.Cast(WERTarget);
									}
									else if (Q.IsReady())
									{
										Q.Cast(WERTarget);
									}
									else
									{
										W.Cast(WERTarget);
									}
								}
								else
								{
									if (E.IsReady())
									{
										E.Cast(WERTarget);
									}
									else if (Q.IsReady())
									{
										Q.Cast(WERTarget);
									}
								}
							}
							else if (Q2.IsReady() && QTarget != null)
							{
								Q2.Cast(QTarget);
							}
						}
						if (stacks == 4)
						{
							if (WERTarget != null)
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
								else if (Q.IsReady())
								{
									Q.Cast(WERTarget);
								}
							}
							else if (Q2.IsReady() && QTarget != null)
							{
								Q2.Cast(QTarget);
							}
						}
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
		
		float lastaa;
		
		public Ryze()
		{
			CreateMenu();
			Game.OnTick += Game_OnTick;
			GameObject.OnCreate += GameObject_OnCreate;
			Obj_AI_Base.OnBasicAttack += Obj_AI_Base_OnBasicAttack;
			GameObject.OnDelete += GameObject_OnDelete;
			Spellbook.OnCastSpell += Spellbook_OnCastSpell;
			Obj_AI_Base.OnSpellCast += Obj_AI_Base_OnSpellCast;
		}
		
		bool canq;
		
		readonly Spell.Skillshot Q = new Spell.Skillshot(SpellSlot.Q, 900, SkillShotType.Linear, 250, 1400, 55) { AllowedCollisionCount = int.MaxValue, MinimumHitChance = HitChance.Low };
		
		readonly Spell.Skillshot Q2 = new Spell.Skillshot(SpellSlot.Q, 900, SkillShotType.Linear, 250, 1400, 55) { AllowedCollisionCount = 0, MinimumHitChance = HitChance.High };
		
		readonly Spell.Targeted W = new Spell.Targeted(SpellSlot.W, 600);
		
		readonly Spell.Targeted E = new Spell.Targeted(SpellSlot.E, 600);
		
		readonly Spell.Active R = new Spell.Active(SpellSlot.R, 600);
		
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
		
		void Obj_AI_Base_OnBasicAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
		{
			if (sender.IsMe)
			{
				lastaa = Game.Time * 1000;
			}
		}
		
		bool passive;
		int stacks;
		float num;
		
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
