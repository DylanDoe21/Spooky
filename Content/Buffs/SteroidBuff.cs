using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.Buffs
{
	public class SteroidBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetDamage<GenericDamageClass>() += 0.2f;

			if (player.buffTime[buffIndex] <= 1)
            {
                player.AddBuff(ModContent.BuffType<SteroidWeakness>(), 18000);
            }
		}

		public override bool RightClick(int buffIndex)
		{
			return false;
		}
    }
}
