using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell
{
	public class SnotRocket : ModItem
	{
		public override void SetStaticDefaults() 
		{
			AmmoID.Sets.IsSpecialist[Type] = true;
		}

		public override void SetDefaults()
		{
			Item.damage = 90;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 30;
			Item.height = 22;
			Item.maxStack = 9999;
			Item.consumable = true;
			Item.knockBack = 3;
			Item.value = Item.buyPrice(silver: 50);
			Item.rare = ItemRarityID.Blue;
			Item.ammo = AmmoID.Rocket;
			Item.shoot = ModContent.ProjectileType<SnotRocketProj>();
		}

		public override void PickAmmo(Item weapon, Player player, ref int type, ref float speed, ref StatModifier damage, ref float knockback)
		{
			if (weapon.type == ItemID.FireworksLauncher)
			{
				type = ProjectileID.Celeb2Rocket + Main.rand.Next(0, 4);
			}
			else if (weapon.type == ItemID.ElectrosphereLauncher)
			{
				type = ProjectileID.ElectrosphereMissile;
			}
			else if (weapon.type == ItemID.Celeb2)
			{
				type = ProjectileID.Celeb2Rocket;
			}
			else
			{
				type = ModContent.ProjectileType<SnotRocketProj>();
			}
		}

		public override void AddRecipes()
        {
            CreateRecipe(10)
			.AddIngredient(ItemID.RocketI, 10)
			.AddIngredient(ModContent.ItemType<SnotGlob>())
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}
}