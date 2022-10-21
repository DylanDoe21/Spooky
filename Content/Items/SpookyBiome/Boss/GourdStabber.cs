using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome.Boss
{
	public class GourdStabber : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Gourd Stabber");
			Tooltip.SetDefault("Left click to rapidly stab with the spear"
			+ "\nRight click to throw the spear, dealing far more damage"
			+ "\nYou cannot use this weapon while the thrown spear is active");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 12;
			Item.crit = 2;
			Item.DamageType = DamageClass.Melee;
			Item.noMelee = true;
			Item.autoReuse = true;
			Item.noUseGraphic = true;
			Item.width = 64;
			Item.height = 64;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 2;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(gold: 1);
			Item.UseSound = SoundID.Item1;		
			Item.shoot = ModContent.ProjectileType<GourdStabberProj>();
			Item.shootSpeed = 7.5f;
		}

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}

		public override bool CanUseItem(Player player)
		{
			if (player.ownedProjectileCounts[Item.shoot] > 0)
			{
				return false;
			}

			if (player.altFunctionUse == 2)
			{
				Item.shoot = ModContent.ProjectileType<GourdStabberThrown>();
			}
			else
			{
				Item.shoot = ModContent.ProjectileType<GourdStabberProj>();
			}

			return true;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (player.altFunctionUse != 2)
			{
				//create velocity vectors for the two angled projectiles (outwards at PI/15 radians)
				Vector2 origVect = new Vector2(velocity.X, velocity.Y);
				Vector2 newVect = Vector2.Zero;
				if (Main.rand.Next(2) == 1) 
				{
					newVect = origVect.RotatedBy(System.Math.PI / (Main.rand.Next(82, 1800) / 10));
				}
				else 
				{
					newVect = origVect.RotatedBy(-System.Math.PI / (Main.rand.Next(82, 1800) / 10));
				}

				velocity.X = newVect.X;
				velocity.Y = newVect.Y;
			}

			return true;
		}

		/*
		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<RottenChunk>(), 10)
			.AddIngredient(ModContent.ItemType<SpookyWoodItem>(), 20)
            .AddTile(TileID.Anvils)
            .Register();
        }
		*/
	}
}