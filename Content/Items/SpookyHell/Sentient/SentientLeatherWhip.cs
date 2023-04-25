using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.Sentient;
using Spooky.Content.Tiles.SpookyHell.Furniture;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientLeatherWhip : ModItem, ICauldronOutput
    {
        public override void SetDefaults()
        {
            Item.damage = 20;
			Item.DamageType = DamageClass.SummonMeleeSpeed;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.width = 56;
			Item.height = 50;
			Item.useTime = 45;
			Item.useAnimation = 45;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 2;
			Item.rare = ModContent.RarityType<SentientRarity>();
			Item.value = Item.buyPrice(gold: 12);
			Item.UseSound = SoundID.Item152;
			Item.shoot = ModContent.ProjectileType<SentientLeatherWhipProj>();
			Item.shootSpeed = 2f;
        }
    }
}