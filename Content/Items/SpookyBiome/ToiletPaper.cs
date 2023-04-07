using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Projectiles.SpookyBiome;
 
namespace Spooky.Content.Items.SpookyBiome
{
    public class ToiletPaper : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 22;    
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.useTurn = true;    
            Item.noUseGraphic = true; 
            Item.autoReuse = true;             
            Item.width = 28;
            Item.height = 26;
            Item.useTime = 38;       
            Item.useAnimation = 38;  
            Item.useStyle = ItemUseStyleID.Swing;     
            Item.knockBack = 6;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 1);   
            Item.UseSound = SoundID.Item1; 
            Item.shoot = ModContent.ProjectileType<ToiletPaperProj>();  
            Item.shootSpeed = 5.5f;     
        }
    }
}