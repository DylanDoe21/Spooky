using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Projectiles.Catacomb;

namespace Spooky.Content.Items.Catacomb
{
    public class GlowBulb : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 45;
            Item.crit = 8;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.useTurn = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.width = 36;
            Item.height = 38;
            Item.useTime = 40;
            Item.useAnimation = 40;
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