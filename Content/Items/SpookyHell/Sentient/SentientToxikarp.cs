using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.Sentient;
using Spooky.Content.Tiles.SpookyHell.Furniture;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientToxikarp : ModItem, ICauldronOutput
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 40;
			Item.DamageType = DamageClass.Ranged;
            Item.autoReuse = true;  
			Item.noMelee = true;
			Item.width = 56;      
			Item.height = 46;
            Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 3;
            Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 25);
            Item.UseSound = SoundID.Item95;
            Item.shoot = ModContent.ProjectileType<ToxicBubble>();
            Item.shootSpeed = 12f;
        }
    }
}