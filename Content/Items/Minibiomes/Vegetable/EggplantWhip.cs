using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Projectiles.Minibiomes.Vegetable;

namespace Spooky.Content.Items.Minibiomes.Vegetable
{
    public class EggplantWhip : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 20;
			Item.DamageType = DamageClass.SummonMeleeSpeed;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.width = 32;
            Item.height = 48;
			Item.useTime = 35;
			Item.useAnimation = 35;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 10;
			Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 1);
			Item.UseSound = SoundID.Item152;
			Item.shoot = ModContent.ProjectileType<EggplantWhipProj>();
			Item.shootSpeed = 3f;
        }

        public override bool MeleePrefix() 
		{
			return true;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<PlantMulch>(), 20)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}