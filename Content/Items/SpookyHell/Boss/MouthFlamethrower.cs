using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.SpookyHell;
 
namespace Spooky.Content.Items.SpookyHell.Boss
{
	public class MouthFlamethrower : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 50;       
			Item.mana = 2;    
			Item.DamageType = DamageClass.Magic;  
			Item.autoReuse = true;       
			Item.width = 70;           
			Item.height = 28;         
			Item.useTime = 4;         
			Item.useAnimation = 8;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 2;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 20);
			Item.UseSound = SoundID.Item34;
			Item.shoot = ModContent.ProjectileType<AcidFlame>();
			Item.shootSpeed = 5f;
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-5, 0);
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<ArteryPiece>(), 10)
			.AddIngredient(ModContent.ItemType<CreepyChunk>(), 15)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
	}
}
