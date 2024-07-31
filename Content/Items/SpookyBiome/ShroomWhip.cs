using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Items.SpookyBiome.Misc;
using Spooky.Content.Projectiles.SpookyBiome;
using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome
{
    public class ShroomWhip : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 13;
			Item.DamageType = DamageClass.SummonMeleeSpeed;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.width = 32;
            Item.height = 42;
			Item.useTime = 35;
			Item.useAnimation = 35;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 2;
			Item.rare = ItemRarityID.White;
            Item.value = Item.buyPrice(gold: 1);
			Item.UseSound = SoundID.Item152;
			Item.shoot = ModContent.ProjectileType<ShroomWhipProj>();
			Item.shootSpeed = 2f;
        }

        public override bool MeleePrefix() 
		{
			return true;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<SpookyGlowshroom>(), 25)
            .AddIngredient(ItemID.Cobweb, 10)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
    }
}