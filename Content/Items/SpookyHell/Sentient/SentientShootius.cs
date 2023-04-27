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
            Item.damage = 30;
            Item.mana = 30;
			Item.DamageType = DamageClass.Summon;
            Item.sentry = true;
			Item.autoReuse = true;
            Item.width = 40;
            Item.height = 50;
            Item.useTime = 45;
			Item.useAnimation = 45;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 2;
            Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 8);
            Item.UseSound = SoundID.DD2_MonkStaffSwing;
            Item.shoot = ModContent.ProjectileType<SentientShootiusSentry>();
            Item.shootSpeed = 0f;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = Main.MouseWorld;
            player.UpdateMaxTurrets();
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