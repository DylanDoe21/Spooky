using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Items.Catacomb.Misc;
using Spooky.Content.Projectiles.Slingshots;

namespace Spooky.Content.Items.Slingshots.Ammo
{
	public class EyeballAmmo : ModItem
	{
		public override void SetStaticDefaults() 
        {
			ItemGlobal.IsSlingshotAmmo[Item.type] = true;
			Item.ResearchUnlockCount = 99;
		}

		public override void SetDefaults() 
        {
			Item.damage = 12;
			Item.DamageType = DamageClass.Ranged; 
            Item.ammo = Item.type;
            Item.consumable = true;
            Item.width = 18;
			Item.height = 20;
			Item.knockBack = 2f;
            Item.maxStack = 9999;
			Item.value = Item.buyPrice(silver: 1);
			Item.rare = ItemRarityID.Green;
			Item.shoot = ModContent.ProjectileType<EyeballAmmoProj>();
		}

		public override bool? CanBeChosenAsAmmo(Item weapon, Player player)
		{
			return ItemGlobal.IsSlingshot[weapon.type] ? true : null;
		}

		/*
		public override void AddRecipes()
        {
            CreateRecipe(15)
            .AddIngredient(ModContent.ItemType<PlantChunk>())
            .Register();
        }
		*/
	}
}