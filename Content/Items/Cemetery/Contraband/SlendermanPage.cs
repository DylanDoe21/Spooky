using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;
using Spooky.Content.Projectiles.Cemetery;

namespace Spooky.Content.Items.Cemetery.Contraband
{
    public class SlendermanPage : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 42;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 40);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SpookyPlayer>().SlendermanPage = true;

            if (player.ownedProjectileCounts[ModContent.ProjectileType<SlendermanTentacle>()] < 4 && Main.myPlayer == player.whoAmI)
            {
                bool[] spawnedTentacle = new bool[4];
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile projectile = Main.projectile[i];
                    if (projectile.active && projectile.type == ModContent.ProjectileType<SlendermanTentacle>() && projectile.owner == Main.myPlayer && projectile.ai[1] >= 0f && projectile.ai[1] < 4f)
                    {
                        spawnedTentacle[(int)projectile.ai[1]] = true;
                    }
                }

                for (int i = 0; i < 4; i++)
                {
                    if (!spawnedTentacle[i])
                    {
                        Vector2 vel = new Vector2(Main.rand.Next(-13, 14), Main.rand.Next(-13, 14)) * 0.25f;
                        Projectile.NewProjectile(null, player.Center, vel, ModContent.ProjectileType<SlendermanTentacle>(), 55, 0f, Main.myPlayer, Main.rand.Next(120), i + 3);
                    }
                }
            }
        }
    }
}