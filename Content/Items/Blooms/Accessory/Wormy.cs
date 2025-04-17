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
                SpawnWorm(ModContent.ProjectileType<WormyHead>(), ModContent.ProjectileType<WormySegment>(), ModContent.ProjectileType<WormyTail>(), new Vector2(player.Center.X, player.Center.Y), player, 25, 3);
            }
		}

        public static void SpawnWorm(int head, int body, int tail, Vector2 spawnPos, Player player, int damage, float knockback)
        {
            Projectile.NewProjectile(null, spawnPos, Vector2.Zero, head, damage, knockback, player.whoAmI);
            Projectile.NewProjectile(null, spawnPos, Vector2.Zero, tail, damage, knockback, player.whoAmI);

            for (var i = 0; i < 9; i++)
            {
				//ai[0] = Body should animate and have the frames for the seasonal abilities
				//ai[1] = Current body segment frame
                Projectile.NewProjectile(null, spawnPos, Vector2.Zero, body, damage, knockback, player.whoAmI);
            }
        }
    }
}