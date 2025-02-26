using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Projectiles.Cemetery;

namespace Spooky.Content.Items.Cemetery.Contraband
{
    public class BackroomsCorpse : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 52;
            Item.accessory = true;
            Item.rare = ItemRarityID.Lime;  
            Item.value = Item.buyPrice(gold: 50);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SpookyPlayer>().BackroomsCorpse = true;

            bool NotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<BackroomsCorpseHead>()] <= 0;
			if (NotSpawned && player.whoAmI == Main.myPlayer)
			{
				Projectile.NewProjectile(null, player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<BackroomsCorpseHead>(), 0, 0f, player.whoAmI, 0f, 0f);
			}
        }
    }
}