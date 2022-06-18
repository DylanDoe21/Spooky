using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.Projectiles.SpookyBiome;
 
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
            Item.damage = 25;    
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;
            Item.useTurn = true;    
            Item.noUseGraphic = true; 
            Item.autoReuse = true;             
            Item.width = 28;
            Item.height = 26;
            Item.useTime = 45;       
            Item.useAnimation = 45;  
            Item.useStyle = 1;      
            Item.knockBack = 6;
            Item.rare = 1;
            Item.value = Item.buyPrice(silver: 25);   
            Item.UseSound = SoundID.Item1; 
            Item.shoot = ModContent.ProjectileType<PumpkinShurikenProj>();  
            Item.shootSpeed = 5f;     
        }
    }
}