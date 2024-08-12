using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell
{
    public class BoogerBook : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<BoogerStaff>();
        }

        public override void SetDefaults()
        {
            Item.damage = 40;
            Item.mana = 5;
			Item.DamageType = DamageClass.Magic;
            Item.noMelee = true;
            Item.channel = true;
            Item.useTurn = true;
            Item.width = 46;
            Item.height = 48;
            Item.useTime = 35;
			Item.useAnimation = 35;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 3;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 15);
            Item.UseSound = SoundID.Item46;
            Item.shoot = ModContent.ProjectileType<ControllableNose>();
            Item.shootSpeed = 0f;
        }

        public override Vector2? HoldoutOffset()
		{
			return new Vector2(-3, 0);
		}
    }
}