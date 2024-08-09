using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell
{
	public class LeechWhip : ModItem
	{
		public override void SetDefaults() 
        {
			Item.damage = 120;
			Item.DamageType = DamageClass.SummonMeleeSpeed;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.width = 38;
			Item.height = 44;
			Item.useTime = 38;
			Item.useAnimation = 38;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 2;
			Item.rare = ItemRarityID.LightPurple;
			Item.value = Item.buyPrice(gold: 20);
			Item.UseSound = SoundID.Item152;
			Item.shoot = ModContent.ProjectileType<LeechWhipProj>();
			Item.shootSpeed = 3.5f;
		}

		public override bool MeleePrefix() 
		{
			return true;
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