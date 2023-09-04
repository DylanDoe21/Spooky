using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Items.Catacomb.Misc;
using Spooky.Content.Projectiles.Catacomb;

namespace Spooky.Content.Items.Catacomb
{
	public class DaffodilRod : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 42;
			Item.mana = 15;       
			Item.DamageType = DamageClass.Magic;
			Item.autoReuse = true;  
			Item.noMelee = true;  
			Item.width = 46;           
			Item.height = 48;         
			Item.useTime = 32;         
			Item.useAnimation = 32;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 3;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 15);
			Item.UseSound = SoundID.Grass; 
			Item.shoot = ModContent.ProjectileType<DaffodilRodFlower>();
			Item.shootSpeed = 10f;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<PlantChunk>(), 18)
			.AddIngredient(ItemID.Daybloom, 3)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
	}
}
