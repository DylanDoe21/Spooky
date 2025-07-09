using Terraria;
using Terraria.ModLoader;

using Spooky.Content.Projectiles.Pets;

namespace Spooky.Content.Buffs.Mounts
{
	public class TrapdoorSpiderMountBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.mount.SetMount(ModContent.MountType<TrapdoorSpiderMount>(), player);
			player.buffTime[buffIndex] = 2;
		}
	}
}