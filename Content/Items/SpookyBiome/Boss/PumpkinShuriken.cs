using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.Projectiles.SpookyBiome;
using Spooky.Content.Tiles.SpookyBiome;
 
namespace Spooky.Content.Items.SpookyBiome.Boss
{
    public class PumpkinShuriken : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Putrid Shuriken");
            Tooltip.SetDefault("Throws piercing pumpkin shurikens that bounce off of surfaces"
            + "\nThe shurikens have a chance to break on enemy hits, creating damaging shrapnel");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 15;    
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;
            Item.useTurn = true;    
            Item.noUseGraphic = true; 
            Item.autoReuse = true;             
            Item.width = 28;
            Item.height = 26;
            Item.useTime = 45;       
            Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 1);
            Item.UseSound = SoundID.Item1; 
            Item.shoot = ModContent.ProjectileType<PumpkinShurikenProj>();  
            Item.shootSpeed = 5f;     
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<RottenChunk>(), 10)
			.AddIngredient(ModContent.ItemType<SpookyWoodItem>(), 20)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}