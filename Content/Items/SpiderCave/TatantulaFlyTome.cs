using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.SpiderCave;

namespace Spooky.Content.Items.SpiderCave
{
    public class TatantulaFlyTome : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 40;
            Item.mana = 25;
			Item.DamageType = DamageClass.Magic;
            Item.noMelee = true;
            Item.width = 34;
            Item.height = 38;
            Item.useTime = 50;
			Item.useAnimation = 50;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 3;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 5);
            Item.UseSound = SoundID.Item34;
            Item.shoot = ModContent.ProjectileType<TatantulaTomeFly>();
            Item.shootSpeed = 2f;
        }

        public override Vector2? HoldoutOffset()
		{
			return new Vector2(-2, 0);
		}
    }
}