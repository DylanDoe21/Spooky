using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Projectiles.Cemetery;

namespace Spooky.Content.Items.Cemetery.Contraband
{
    public class Local58Telescope : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 30;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 40);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SpookyPlayer>().Local58Telescope = true;

            bool NotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<Local58Moon>()] <= 0;
			if (NotSpawned && player.whoAmI == Main.myPlayer)
			{
				Projectile.NewProjectile(null, player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<Local58Moon>(), 55, 3f, player.whoAmI);
			}
        }
    }
}