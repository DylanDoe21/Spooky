using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Buffs.Minion;
using Spooky.Content.Items.Catacomb.Misc;
using Spooky.Content.Projectiles.Catacomb;

namespace Spooky.Content.Items.Catacomb
{
	public class DaffodilStaff : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 50;
			Item.mana = 15;       
			Item.DamageType = DamageClass.Summon;
			Item.autoReuse = true;
			Item.noMelee = true;
			Item.width = 44;           
			Item.height = 42;         
			Item.useTime = 32;         
			Item.useAnimation = 32;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 3;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 15);
			Item.UseSound = SoundID.Grass;
			Item.buffType = ModContent.BuffType<DaffodilHandBuff>();
			Item.shoot = ModContent.ProjectileType<DaffodilHandMinion>();
		}

		public override bool CanUseItem(Player player)
		{
			if (player.ownedProjectileCounts[Item.shoot] > 0 && player.altFunctionUse != 2) 
			{
				return false;
			}

			return true;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
            player.AddBuff(Item.buffType, 2);

			var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
			projectile.originalDamage = Item.damage;

			return false;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<PlantChunk>(), 18)
			.AddIngredient(ItemID.Stinger, 5)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
	}
}
