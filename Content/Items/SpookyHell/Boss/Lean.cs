using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.Projectiles.SpookyHell;
 
namespace Spooky.Content.Items.SpookyHell.Boss
{
    public class Lean : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cup of Lean");
            Tooltip.SetDefault("'The perfect beverage for halloween parties'");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 65;
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.width = 30;
            Item.height = 36;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.useStyle = 1;
            Item.knockBack = 2f;
            Item.rare = 5;
           	Item.value = Item.buyPrice(gold: 2);
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<LeanProj>();
            Item.shootSpeed = 12f;
        }
    }
}