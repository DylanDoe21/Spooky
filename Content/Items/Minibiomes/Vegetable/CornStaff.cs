using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Content.Projectiles.Minibiomes.Vegetable;

namespace Spooky.Content.Items.Minibiomes.Vegetable
{
    public class CornStaff : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 60;
            Item.mana = 10;
            Item.DamageType = DamageClass.Summon;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.channel = true;
            Item.width = 58;
            Item.height = 58;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 2;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 15);
            Item.UseSound = SoundID.Item8;
            Item.shoot = ModContent.ProjectileType<CornSentry>();
            Item.shootSpeed = 0f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int sentry = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI);
            if (Main.projectile.IndexInRange(sentry))
            {
                Main.projectile[sentry].originalDamage = Item.damage;
            }
            
            player.UpdateMaxTurrets();

            return false;
        }

        public override bool CanUseItem(Player player)
        {
            //dont allow this item to be used if your cursor is inside tiles
            Vector2 mouse = Main.MouseWorld;
            List<Vector2> tiles = new List<Vector2>();
            for (int i = -2; i < 2; i++)
            {
                for (int j = -3; j < 3; j++)
                {
                    Tile tile = Framing.GetTileSafely(i + (int)(mouse.X / 16), j + (int)(mouse.Y / 16));
                    if (tile.HasTile && Main.tileSolid[tile.TileType])
                    {
                        tiles.Add(new Vector2(i + (int)(mouse.X / 16), j + (int)(mouse.Y / 16)));
                    }
                }
            }

            if (tiles.Count > 0)
            {
                return false;
            }

            return true;
        }
    }
}