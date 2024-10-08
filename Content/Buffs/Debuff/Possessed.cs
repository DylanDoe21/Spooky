using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Buffs.Debuff
{
	public class Possessed : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;  
            Main.pvpBuff[Type] = true;  
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = false;
        }

		public override void Update(Player player, ref int buffIndex)
		{
			if (Main.rand.NextBool(75))
			{
				player.velocity.X = Main.rand.Next(-10, 10);
				player.velocity.Y = Main.rand.Next(-10, 10);
			}
		}
	}
}
