using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Projectiles.Cemetery;

namespace Spooky.Content.Items.Cemetery.Contraband
{
    public class SmileDogPicture : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 48;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;  
            Item.value = Item.buyPrice(gold: 20);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SpookyPlayer>().SmileDogPicture = true;

            bool NotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<SmilingDog>()] <= 0;
			if (NotSpawned && player.whoAmI == Main.myPlayer)
			{
				Projectile.NewProjectile(null, player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<SmilingDog>(), 30, 1f, player.whoAmI);
			}
        }
    }
}