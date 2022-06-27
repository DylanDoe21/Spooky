using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using Spooky.Content.Projectiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome.Boss
{
	public class PumpkinTome : ModItem
	{
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Root Scourge");
			Tooltip.SetDefault("Conjures pumpkin roots out of any surface"
			+ "\nYour cursor must be nearby surfaces to spawn pumpkin roots");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() 
		{
			Item.damage = 32;
			Item.mana = 20;
			Item.DamageType = DamageClass.Magic;
			Item.noMelee = true;
			Item.autoReuse = true;
			Item.width = 32;
            Item.height = 32;
            Item.useTime = 32;
            Item.useAnimation = 32;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 1;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(gold: 1);
			Item.UseSound = SoundID.Item69;
            Item.shoot = ModContent.ProjectileType<Blank>();
            Item.shootSpeed = 4.5f;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Vector2 mouse = new Vector2(Main.mouseX, Main.mouseY) + Main.screenPosition;
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

				Vector2 ProjectileDirection = mouse - position;
				ProjectileDirection.Normalize();
				ProjectileDirection *= Item.shootSpeed;
				velocity.X = ProjectileDirection.X * 0;
				velocity.Y = ProjectileDirection.Y;

				Projectile.NewProjectile(source, position.X, position.Y, velocity.X + Main.rand.Next(-2, 2), velocity.Y, 
				ModContent.ProjectileType<PumpkinRoot>(), damage, knockback, player.whoAmI, 0f, 0f);

				return true;
			}
			
			return false;
		}
    }
}