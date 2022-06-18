using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Buffs.Debuff
{
	public class Possessed : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Possessed");
			Description.SetDefault("A tiny ghost is controlling you!");
			Main.debuff[Type] = true;  
            Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			if (Main.rand.Next(75) == 0)
			{
				player.velocity.X = Main.rand.Next(-10, 10);
				player.velocity.Y = Main.rand.Next(-10, 10);
			}
		}
	}
}
