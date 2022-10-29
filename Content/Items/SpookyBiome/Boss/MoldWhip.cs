using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.Projectiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome.Boss
{
	public class MoldWhip : ModItem
	{
		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mold Spore Whip");
			Tooltip.SetDefault("Your summons will focus struck enemies"
			+ "\nHitting enemies will inflict them with the moldy debuff"
			+ "\nKilling enemies with this debuff will summon damaging flies");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

		public override void SetDefaults() 
        {
			Item.damage = 15;
			Item.DamageType = DamageClass.SummonMeleeSpeed;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.width = 32;
			Item.height = 38;
			Item.useTime = 42;
			Item.useAnimation = 42;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 2;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(gold: 1);
			Item.UseSound = SoundID.Item152;
			Item.shoot = ModContent.ProjectileType<MoldWhipProj>();
			Item.shootSpeed = 3;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<RottenChunk>(), 15)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}
}