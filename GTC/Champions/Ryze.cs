using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace GTC.Champions
{
	public class Ryze
	{
		Menu menu;
		
		readonly Spell.Skillshot Q = new Spell.Skillshot(SpellSlot.Q, 900, SkillShotType.Linear, 250, 1400, 55) { AllowedCollisionCount = int.MaxValue, MinimumHitChance = HitChance.Medium };
		
		readonly Spell.Skillshot Q2 = new Spell.Skillshot(SpellSlot.Q, 900, SkillShotType.Linear, 250, 1400, 70) { AllowedCollisionCount = 0, MinimumHitChance = HitChance.High };
		
		readonly Spell.Targeted W = new Spell.Targeted(SpellSlot.W, 600);
		
		readonly Spell.Targeted E = new Spell.Targeted(SpellSlot.E, 600);
		
		readonly Spell.Active R = new Spell.Active(SpellSlot.R, 600);
		
		float num, cannext;
		
		bool canq, passive;
		
		int stacks;
		
		public Ryze()
		{
			menu = MainMenu.AddMenu("GTC Ryze", "gtcryze");
			menu.AddGroupLabel("Addon made by Funboxxx");
			menu.AddGroupLabel("BEST COMBO IS WHEN YOU HAVE 1 STACK AND QWER READY");
			menu.AddSeparator();
			menu.Add("next", new Slider("Delay Between Spells", 175, 150, 250));
			menu.AddSeparator();
			menu.Add("autoq", new CheckBox("Auto Q"));
			menu.AddSeparator();
			menu.Add("mana", new Slider("Auto Q min. % Mana", 75));
			Game.OnTick += Game_OnTick;
			Obj_AI_Base.OnSpellCast += Obj_AI_Base_OnSpellCast;
			Spellbook.OnCastSpell += Spellbook_OnCastSpell;
			GameObject.OnCreate += GameObject_OnCreate;
			GameObject.OnDelete += GameObject_OnDelete;
		}
		
		void Game_OnTick(EventArgs args)
		{
			try
			{
				if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
				{
					if (!Q.IsReady() && !W.IsReady() && !E.IsReady() && !R.IsReady())
					{
						Orbwalker.DisableAttacking = false;
						return;
					}
					Combo();
				}
				else
				{
					Orbwalker.DisableAttacking = false;
					if (menu["autoq"].Cast<CheckBox>().CurrentValue && Orbwalker.CanMove)
					{
						AutoQ();
					}
				}
			}
			catch (Exception e)
			{
				Chat.Print(e);
			}
		}
		
		void AutoQ()
		{
			if (!Q.IsReady() || Player.Instance.Mana < Player.Instance.MaxMana * (menu["mana"].Cast<Slider>().CurrentValue / 100f))
			{
				return;
			}
			var t = TargetSelector.GetTarget(900, DamageType.Magical);
			if (t == null)
			{
				return;
			}
			var pred2 = Q2.GetPrediction(t);
			if (!pred2.CastPosition.IsZero)
			{
				Q.Cast(pred2.CastPosition);
			}
		}
		
		void CastQ()
		{
			if (!Q.IsReady() || Game.Time * 1000 < cannext + menu["next"].Cast<Slider>().CurrentValue)
			{
				return;
			}
			var t = TargetSelector.GetTarget(900, DamageType.Magical);
			if (t == null)
			{
				return;
			}
			if ((!passive && stacks > 2) || passive)
			{
				var pred = Q.GetPrediction(t);
				if (!pred.CastPosition.IsZero)
				{
					if (Orbwalker.CanMove)
					{
						Orbwalker.DisableAttacking = true;
					}
					Q.Cast(pred.CastPosition);
				}
			}
			var pred2 = Q2.GetPrediction(t);
			if (!pred2.CastPosition.IsZero)
			{
				if (Orbwalker.CanMove)
				{
					Orbwalker.DisableAttacking = true;
				}
				Q.Cast(pred2.CastPosition);
			}
		}
		
		void CastW()
		{
			if (!W.IsReady() || Game.Time * 1000 < cannext + menu["next"].Cast<Slider>().CurrentValue)
			{
				return;
			}
			if (Orbwalker.CanMove)
			{
				Orbwalker.DisableAttacking = true;
			}
			var t = TargetSelector.GetTarget(600, DamageType.Magical);
			if (t == null)
			{
				return;
			}
			W.Cast(t);
		}
		
		void CastE()
		{
			if (!E.IsReady() || Game.Time * 1000 < cannext + menu["next"].Cast<Slider>().CurrentValue)
			{
				return;
			}
			if (Orbwalker.CanMove)
			{
				Orbwalker.DisableAttacking = true;
			}
			var t = TargetSelector.GetTarget(600, DamageType.Magical);
			if (t == null)
			{
				return;
			}
			E.Cast(t);
		}
		
		void CastR()
		{
			if (!R.IsReady() || Game.Time * 1000 < cannext + menu["next"].Cast<Slider>().CurrentValue)
			{
				return;
			}
			if (Orbwalker.CanMove)
			{
				Orbwalker.DisableAttacking = true;
			}
			var t = TargetSelector.GetTarget(600, DamageType.Magical);
			if (t == null)
			{
				return;
			}
			R.Cast();
		}
		
		void Combo()
		{
			if (Game.Time * 1000 < cannext + menu["next"].Cast<Slider>().CurrentValue)
			{
				return;
			}
			if (passive)
			{
				if (canq)
				{
					CastQ();
				}
				else
				{
					if (R.IsReady())
					{
						CastR();
					}
					else if (W.IsReady())
					{
						CastW();
					}
					else if (E.IsReady())
					{
						CastE();
					}
				}
			}
			else
			{
				if (stacks == 0)
				{
					if (Q.IsReady() && W.IsReady() && E.IsReady() && R.IsReady())
					{
						CastQ();
						return;
					}
					CastW();
					CastE();
					CastQ();
				}
				if (stacks == 1)
				{
					if (W.IsReady() && E.IsReady() && R.IsReady())
					{
						CastR();
						return;
					}
					CastW();
					CastE();
					CastQ();
					
				}
				if (stacks == 2)
				{
					if (R.IsReady() && (E.IsReady() || W.IsReady()))
					{
						CastR();
						return;
					}
					if (W.IsReady() && E.IsReady())
					{
						CastE();
						return;
					}
					CastW();
					CastQ();
				}
				if (stacks == 3)
				{
					if (R.IsReady() && (Q.IsReady() || W.IsReady()))
					{
						CastR();
						return;
					}
					CastE();
					CastQ();
					CastW();
				}
				if (stacks == 4)
				{
					CastR();
					CastW();
					CastQ();
					CastE();
				}
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
				if (args.Slot == SpellSlot.Q)
				{
					cannext = Game.Time * 1000;
				}
				if (args.Slot == SpellSlot.W)
				{
					cannext = Game.Time * 1000;
				}
				if (args.Slot == SpellSlot.E)
				{
					cannext = Game.Time * 1000;
				}
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
				stacks = 0;
			}
		}
	}
}
