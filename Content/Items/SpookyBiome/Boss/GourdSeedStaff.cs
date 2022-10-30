using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.SpookyBiome;
using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome.Boss
{
	public class GourdSeedStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Gourd Seed Staff");
			Tooltip.SetDefault("Casts spreads of short lived gourd seeds");
			Item.staff[Item.type] = true;
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 10;
			Item.mana = 5;          
			Item.DamageType = DamageClass.Magic;
			Item.noMelee = true;
			Item.autoReuse = true;       
			Item.width = 38;           
			Item.height = 36;         
			Item.useTime = 25;         
			Item.useAnimation = 25;         
			Item.useStyle = ItemUseStyleID.Shoot;          
			Item.knockBack = 1;
			Item.rare = ItemRarityID.Blue;  
			Item.value = Item.buyPrice(silver: 50);
			Item.UseSound = SoundID.Item70;
			Item.shoot = ModContent.ProjectileType<GourdSeed>();
			Item.shootSpeed = 5f;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			for (int numProjectiles = 0; numProjectiles < 2; numProjectiles++)
			{
				Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
				int i = Main.myPlayer;
				float num72 = Item.shootSpeed - Main.rand.Next(1, 3);
				int num73 = damage;
				float num74 = knockback;
				float num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
				float num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
				float f = Main.rand.NextFloat() * 6.28318548f;
				float value12 = 20f;
				float value13 = 60f;
				Vector2 vector13 = vector2 + f.ToRotationVector2() * MathHelper.Lerp(value12, value13, Main.rand.NextFloat());
				
				for (int num202 = 0; num202 < 50; num202++)
				{
					vector13 = vector2 + f.ToRotationVector2() * MathHelper.Lerp(value12, value13, Main.rand.NextFloat());
					if (Collision.CanHit(vector2, 0, 0, vector13 + (vector13 - vector2).SafeNormalize(Vector2.UnitX) * 8f, 0, 0))
					{
						break;
					}

					f = Main.rand.NextFloat() * 6.28318548f;
				}

				Vector2 mouseWorld = Main.MouseWorld;
				Vector2 vector14 = mouseWorld - vector13;
				Vector2 vector15 = new Vector2(num78, num79).SafeNormalize(Vector2.UnitY) * num72;
				vector14 = vector14.SafeNormalize(vector15) * num72;
				vector14 = Vector2.Lerp(vector14, vector15, Main.rand.NextFloat(-0.25f, 0.25f));
				Projectile.NewProjectile(source, player.Center, vector14, type, num73, num74, i, 0f, 0f);
			}
			
			return true;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<RottenChunk>(), 10)
			.AddIngredient(ModContent.ItemType<SpookyWoodItem>(), 10)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}
}
