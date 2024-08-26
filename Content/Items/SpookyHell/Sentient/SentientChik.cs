using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Projectiles.Sentient;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientChik : ModItem, ICauldronOutput
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.Yoyo[Item.type] = true;
            ItemID.Sets.GamepadSmartQuickReach[Item.type] = true;
            ItemID.Sets.GamepadExtraRange[Item.type] = 15;
        }

        public override void SetDefaults()
        {
			Item.damage = 45;
			Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.width = 42;          
			Item.height = 52;
            Item.useTime = 22;
            Item.useAnimation = 22;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 2;
            Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 30);
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<SentientChikProj>();
            Item.shootSpeed = 8f;
        }
    }
}