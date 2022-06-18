using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome.Boss
{
	public class PumpkinSpear : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Pumpkin Poker");
			Tooltip.SetDefault("Left click to rapidly stab with the spear"
			+ "\nRight click to throw the spear as a javelin that does more damage"
			+ "\nYou cannot use this weapon while the thrown javelin is active");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 20;
			Item.crit = 2;
			Item.DamageType = DamageClass.Melee;
			Item.noMelee = true;
			Item.autoReuse = true;
			Item.noUseGraphic = true;
			Item.width = 58;
			Item.height = 60;
			Item.useTime = 10;
			Item.useAnimation = 10;
			Item.useStyle = 5;
			Item.knockBack = 2;
			Item.rare = 1;
			Item.value = Item.buyPrice(silver: 25);	
			Item.UseSound = SoundID.Item1;		
			Item.shoot = ModContent.ProjectileType<PumpkinSpearProj>();
			Item.shootSpeed = 9.5f;
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
				Item.shoot = ModContent.ProjectileType<PumpkinSpearThrown>();
			}
			else
			{
				Item.shoot = ModContent.ProjectileType<PumpkinSpearProj>();
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
            .AddIngredient(ModContent.ItemType<MagicPumpkin>(), 10)
            .AddIngredient(ModContent.ItemType<SpookyPlasma>(), 5)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
		*/
	}
}