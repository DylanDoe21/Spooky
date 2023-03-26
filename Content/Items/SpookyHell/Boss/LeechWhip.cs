using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell.Boss
{
	public class LeechWhip : ModItem
	{
		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Leecher");
            Tooltip.SetDefault("Your summons will focus struck enemies"
            + "\nHitting enemies will sometimes rip meat chunks off of them"
			+ "\nPicking up these meat chunks will heal a small amount of your health");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

		public override void SetDefaults() 
        {
			Item.damage = 55;
			Item.DamageType = DamageClass.SummonMeleeSpeed;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.width = 38;
			Item.height = 44;
			Item.useTime = 35;
			Item.useAnimation = 35;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 2;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 2);
			Item.UseSound = SoundID.Item152;
			Item.shoot = ModContent.ProjectileType<LeechWhipProj>();
			Item.shootSpeed = 3.5f;
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