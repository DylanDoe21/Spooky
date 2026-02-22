using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Items.Catacomb.Misc;
using Spooky.Content.Projectiles.Slingshots;

namespace Spooky.Content.Items.Slingshots.Ammo
{
	public class SeedAmmo : ModItem
	{
		public override void SetStaticDefaults() 
        {
			ItemGlobal.IsSlingshotAmmo[Item.type] = true;
			Item.ResearchUnlockCount = 99;
		}

		public override void SetDefaults() 
        {
			Item.damage = 20;
			Item.DamageType = DamageClass.Ranged; 
            Item.ammo = Item.type;
            Item.consumable = true;
            Item.width = 22;
			Item.height = 22;
			Item.knockBack = 2f;
            Item.maxStack = 9999;
			Item.value = Item.buyPrice(silver: 1);
			Item.rare = ItemRarityID.Orange;
			Item.shoot = ModContent.ProjectileType<SeedAmmoProj>();
		}

		public override bool? CanBeChosenAsAmmo(Item weapon, Player player)
		{
			return ItemGlobal.IsSlingshot[weapon.type] ? true : null;
		}

		public override void AddRecipes()
        {
            CreateRecipe(15)
            .AddIngredient(ModContent.ItemType<PlantChunk>())
            .Register();
        }
	}
}