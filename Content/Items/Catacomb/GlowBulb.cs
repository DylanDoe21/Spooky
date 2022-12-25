using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Core;
using Spooky.Content.Projectiles.Catacomb;

namespace Spooky.Content.Items.Catacomb
{
    public class GlowBulb : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Thorn Covered Bulb");
            Tooltip.SetDefault("Throws a glowing thorn ball that can stick to the ground"
            + "\nThe thorn ball will explode into smaller thorns when it hits an enemy");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 45;
            Item.crit = 10;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.useTurn = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.width = 36;
            Item.height = 38;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 20);
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<GlowBulbProj>();
            Item.shootSpeed = 10f;
        }
    }
}