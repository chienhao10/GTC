using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace GTC.Champions
{
	public class Katarina
	{
		public Katarina()
		{
			menu = MainMenu.AddMenu("GTC Katarina", "gtckatarina");
			menu.Add("key", new KeyBind("Combo Key", false, KeyBind.BindTypes.HoldActive, ' '));
			menu.Add("autoq", new CheckBox("Auto Q"));
			menu.Add("autow", new CheckBox("Auto W"));
			Game.OnTick += Game_OnTick;
			GameObject.OnDelete += GameObject_OnDelete;
		}
		
		Menu menu;
		
		readonly Spell.Targeted Q = new Spell.Targeted(SpellSlot.Q, 675);
		
		readonly Spell.Active W = new Spell.Active(SpellSlot.W, 375);
		
		readonly Spell.Targeted E = new Spell.Targeted(SpellSlot.E, 700);
		
		readonly Spell.Active R = new Spell.Active(SpellSlot.R, 550);
		
		float laste;
		
		bool hasr;
		
		void CastQ()
		{
			var qt = TargetSelector.GetTarget(675, DamageType.Magical);
			if (!Q.IsReady() || hasr || !qt.IsValidTarget() || qt.IsZombie)
			{
				return;
			}
			Q.Cast(qt);
		}
		
		void CastW()
		{
			var wt = TargetSelector.GetTarget(375, DamageType.Magical);
			if (!W.IsReady() || hasr || !wt.IsValidTarget() || wt.IsZombie || E.IsReady())
			{
				return;
			}
			W.Cast();
		}
		
		void CastE()
		{
			var et = TargetSelector.GetTarget(700, DamageType.Magical);
			if (!E.IsReady() || hasr || !et.IsValidTarget() || et.IsZombie || Q.IsReady())
			{
				return;
			}
			laste = Game.Time * 1000;
			Core.DelayAction(Orbwalker.ResetAutoAttack, 250);
			E.Cast(et);
		}
		
		void CastR()
		{
			var rt = TargetSelector.GetTarget(300, DamageType.Magical);
			if (!R.IsReady() || !rt.IsValidTarget() || rt.IsZombie || W.IsReady())
			{
				return;
			}
			Orbwalker.DisableAttacking = true;
			Orbwalker.DisableMovement = true;
			hasr = true;
			R.Cast();
		}
		
		void Move()
		{
			var t = TargetSelector.GetTarget(350, DamageType.Magical);
			if ((!t.IsValidTarget() || t.IsZombie) && Game.Time * 1000 > laste + 1000 || hasr)
			{
				return;
			}
			Orbwalker.MoveTo(t.Position);
		}
		
		void Game_OnTick(EventArgs args)
		{
			if (menu["key"].Cast<KeyBind>().CurrentValue)
			{
				CastQ();
				CastE();
				CastW();
				CastR();
				Move();
			}
			else
			{
				if (menu["autoq"].Cast<CheckBox>().CurrentValue)
				{
					CastQ();
				}
				if (menu["autow"].Cast<CheckBox>().CurrentValue)
				{
					CastW();
				}
			}
		}

		void GameObject_OnDelete(GameObject sender, EventArgs args)
		{
			if (sender.Name == "Katarina_Base_deathLotus_cas.troy")
			{
				hasr = false;
				Orbwalker.DisableAttacking = false;
				Orbwalker.DisableMovement = false;
			}
		}
	}
}
