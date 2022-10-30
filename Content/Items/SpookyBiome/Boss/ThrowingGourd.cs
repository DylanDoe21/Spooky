using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.Projectiles.SpookyBiome;
using Spooky.Content.Tiles.SpookyBiome;
 
namespace Spooky.Content.Items.SpookyBiome.Boss
{
    public class ThrowingGourd : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Throwing Gourd");
            Tooltip.SetDefault("Throws gourds that bounce off of surfaces");
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
            Item.shoot = ModContent.ProjectileType<ThrowingGourdProj>();  
            Item.shootSpeed = 5f;     
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