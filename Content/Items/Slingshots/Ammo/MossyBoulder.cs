using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Projectiles.Slingshots;
using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Content.Items.Slingshots.Ammo
{
	public class MossyBoulder : ModItem
	{
		public override void SetStaticDefaults() 
        {
			ItemGlobal.IsSlingshotAmmo[Item.type] = true;
			Item.ResearchUnlockCount = 99;
		}

		public override void SetDefaults() 
        {
			Item.damage = 32;
			Item.DamageType = DamageClass.Ranged; 
            Item.ammo = Item.type;
            Item.consumable = true;
            Item.width = 22;
			Item.height = 22;
			Item.knockBack = 4f;
            Item.maxStack = 9999;
			Item.value = Item.buyPrice(silver: 1);
			Item.rare = ItemRarityID.White; 
			Item.shoot = ModContent.ProjectileType<MossyBoulderProj>();
		}

		public override bool? CanBeChosenAsAmmo(Item weapon, Player player)
		{
			return ItemGlobal.IsSlingshot[weapon.type] ? true : null;
		}

		public override void AddRecipes()
        {
            CreateRecipe(5)
            .AddIngredient(ModContent.ItemType<SpookyStoneItem>())
            .Register();
        }
	}
}