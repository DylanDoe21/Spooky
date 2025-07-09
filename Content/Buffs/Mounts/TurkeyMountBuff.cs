using Terraria;
using Terraria.ModLoader;

using Spooky.Content.Projectiles.Pets;

namespace Spooky.Content.Buffs.Mounts
{
	public class TurkeyMountBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoSave[Type] = false;
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.mount.SetMount(ModContent.MountType<TurkeyMount>(), player);
			player.buffTime[buffIndex] = 2;
		}
	}
}