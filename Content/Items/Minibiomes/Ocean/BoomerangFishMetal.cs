using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Projectiles.Minibiomes.Ocean;
 
namespace Spooky.Content.Items.Minibiomes.Ocean
{
    public class BoomerangFishMetal : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 75;
            Item.DamageType = DamageClass.Melee;
            Item.useTurn = true;    
            Item.noUseGraphic = true; 
            Item.noMelee = true;
            Item.autoReuse = true;             
            Item.width = 30;
            Item.height = 34;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 2;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 10);
            Item.UseSound = SoundID.Item1; 
            Item.shoot = ModContent.ProjectileType<BoomerangFishMetalProj>();  
            Item.shootSpeed = 25f;
        }

        public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[Item.shoot] < 5;
		}

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<BoomerangFish>(), 1)
            .AddIngredient(ModContent.ItemType<DunkleosteusHide>(), 12)
            .AddIngredient(ItemID.SoulofMight, 5)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
}