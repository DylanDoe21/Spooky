using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using Spooky.Content.Projectiles.Cemetery;

namespace Spooky.Content.Items.Cemetery
{
    public class SpiritHandStaff : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 15;
			Item.mana = 12;
			Item.DamageType = DamageClass.Magic;
            Item.noMelee = true;
			Item.autoReuse = true;
            Item.channel = true;
			Item.width = 42;
            Item.height = 48;
			Item.useTime = 15;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 0;
			Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 2);
			Item.UseSound = SoundID.DD2_GhastlyGlaivePierce;
			Item.shoot = ModContent.ProjectileType<SpiritHand>();
			Item.shootSpeed = 5f;
        }

        public override bool CanUseItem(Player player)
        {
            Vector2 mouse = Main.MouseWorld;
            List<Vector2> tiles = new List<Vector2>();
            for (int i = -12; i < 12; i++)
            {
                for (int j = -12; j < 12; j++)
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
                return true;
            }

            return false;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 mouse = Main.MouseWorld;
            List<Vector2> tiles = new List<Vector2>();
            for (int i = -12; i < 12; i++)
            {
                for (int j = -12; j < 12; j++)
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
                for (int k = 0; k < tiles.Count; k++)
                {
                    Vector2 distance = (tiles[k] * 16) - mouse;
                    double truedist = Math.Sqrt((distance.X * distance.X) + (distance.Y * distance.Y));
                    if (truedist < currentdist && (Main.rand.NextBool(5) || currentdist == 9999))
                    {
                        position = tiles[k] * 16;
                        currentdist = truedist;
                    }
                }

                Vector2 direction = mouse - position;
                direction.Normalize();
                direction *= Item.shootSpeed;
                velocity = direction;
                SoundEngine.PlaySound(SoundID.Item103, position);
            }
        }
    }
}