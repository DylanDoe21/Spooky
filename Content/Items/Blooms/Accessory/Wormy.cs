using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Projectiles.Blooms;

namespace Spooky.Content.Items.Blooms.Accessory
{
    public class Wormy : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 36;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 2);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        { 
            player.GetModPlayer<BloomBuffsPlayer>().Wormy = true;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<WormyHead>()] <= 0)
			{
                Projectile.NewProjectile(null, player.Center, Vector2.Zero, ModContent.ProjectileType<WormyHead>(), 25, 3, player.whoAmI);
            }
		}
    }
}