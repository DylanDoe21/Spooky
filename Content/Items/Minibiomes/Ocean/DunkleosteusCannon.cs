using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.Minibiomes.Ocean;
 
namespace Spooky.Content.Items.Minibiomes.Ocean
{
	public class DunkleosteusCannon : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 150;
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.autoReuse = true;
			Item.noUseGraphic = true;
			Item.channel = true;
			Item.width = 92;
			Item.height = 50;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 4;
            Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 10);
			Item.UseSound = SoundID.Item1;
			Item.shoot = ModContent.ProjectileType<DunkleosteusCannonProj>();
			Item.shootSpeed = 0f;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<SharkboneCannon>(), 1)
			.AddIngredient(ModContent.ItemType<DunkleosteusHide>(), 12)
            .AddIngredient(ItemID.SoulofSight, 5)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
	}
}
