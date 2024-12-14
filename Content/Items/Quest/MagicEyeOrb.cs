using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Items.Quest
{
	public class MagicEyeOrb : ModItem
	{
		public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 34;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;  
            Item.value = Item.buyPrice(gold: 10);
        }

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<SpookyPlayer>().MagicEyeOrb = true;

			bool NotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<GlassEye>()] <= 0;
			if (NotSpawned && player.whoAmI == Main.myPlayer)
			{
				//leave the source as null for right now
				Projectile.NewProjectile(null, player.position.X + (float)(player.width / 2), player.position.Y - 3, 0f, 0f, ModContent.ProjectileType<GlassEye>(), 0, 0f, player.whoAmI);
			}
		}
	}
}
