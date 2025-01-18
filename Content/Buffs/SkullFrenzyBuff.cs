using Terraria;
using Terraria.ModLoader;

namespace Spooky.Content.Buffs
{
	public class SkullFrenzyBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			//full skull frenzy charge grants 20% increased damage
			//must be given here since the skull frenzy charge is reset in the modplayer once its full
			player.GetDamage(DamageClass.Generic) += 0.2f;

			player.GetAttackSpeed(DamageClass.Melee) += 0.75f;
			player.GetAttackSpeed(DamageClass.Ranged) += 0.75f;
			player.GetAttackSpeed(DamageClass.Magic) += 0.75f;
			player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) += 0.75f;
		}
	}
}
