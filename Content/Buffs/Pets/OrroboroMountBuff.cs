using Terraria;
using Terraria.ModLoader;

using Spooky.Content.Projectiles.Pets;

namespace Spooky.Content.Buffs.Pets
{
	public class OrroboroMountBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Eye Worm");
			Description.SetDefault("The eye worm will help you travel");
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.mount.SetMount(ModContent.MountType<OrroboroMount>(), player);
			player.buffTime[buffIndex] = 2;
		}
	}
}