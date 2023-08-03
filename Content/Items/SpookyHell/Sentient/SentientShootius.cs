using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using Spooky.Content.Projectiles.Sentient;
using Spooky.Content.Tiles.SpookyHell.Furniture;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientShootius : ModItem, ICauldronOutput
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 35;
            Item.mana = 30;
			Item.DamageType = DamageClass.Summon;
            Item.sentry = true;
            Item.noMelee = true;
			Item.autoReuse = true;
            Item.width = 40;
            Item.height = 50;
            Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 2;
            Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 8);
            Item.UseSound = SoundID.Item82;
            Item.shoot = ModContent.ProjectileType<SentientShootiusSentry>();
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

            double currentdist = 9999;
            if (tiles.Count > 0)
            {
                return false;
            }

            return true;
        }
    }
}