using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.Projectiles.SpookyBiome;
 
namespace Spooky.Content.Items.SpookyBiome
{
    public class ElGourdo : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("El Gourdo");
            Tooltip.SetDefault("Throws a slow moving gourd bomb that will stop midair after a bit"
            + "\nAfter a few seconds, the bomb will explode, unleashing lingering greek fire"
            + "\nThe bomb will automatically detonate when it hits an enemy");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 100;    
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.useTurn = true;    
            Item.noUseGraphic = true; 
            Item.autoReuse = true;             
            Item.width = 42;
            Item.height = 74;
            Item.useTime = 45;       
            Item.useAnimation = 45;  
            Item.useStyle = ItemUseStyleID.Swing;      
            Item.knockBack = 6;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.buyPrice(gold: 20);   
            Item.UseSound = SoundID.Item1; 
            Item.shoot = ModContent.ProjectileType<ElGourdoProj>();  
            Item.shootSpeed = 5f;     
        }
    }
}