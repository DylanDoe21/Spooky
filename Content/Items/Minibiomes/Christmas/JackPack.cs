using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.Minibiomes.Christmas;

namespace Spooky.Content.Items.Minibiomes.Christmas
{
    public class JackPack : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 8;
            Item.mana = 5;
            Item.DamageType = DamageClass.Magic;
			Item.noMelee = true;
			Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.width = 26;
            Item.height = 44;
            Item.useTime = 20;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 3;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 1);
            Item.UseSound = SoundID.Item1;
			Item.shoot = ModContent.ProjectileType<ThrowingCard>();
            Item.shootSpeed = 8f;
        }
    }
}