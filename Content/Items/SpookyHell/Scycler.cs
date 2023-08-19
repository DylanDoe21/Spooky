using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.Projectiles.SpookyHell;
 
namespace Spooky.Content.Items.SpookyHell
{
    public class Scycler : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 55;    
            Item.DamageType = DamageClass.Melee;
            Item.useTurn = true;    
            Item.noUseGraphic = true; 
            Item.noMelee = true;
            Item.autoReuse = true;             
            Item.width = 60;
            Item.height = 44;
            Item.useTime = 25;       
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 2;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.buyPrice(gold: 20);   
            Item.UseSound = SoundID.Item1; 
            Item.shoot = ModContent.ProjectileType<ScyclerProj>();  
            Item.shootSpeed = 25f;
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