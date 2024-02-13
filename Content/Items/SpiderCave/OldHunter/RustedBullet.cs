using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Projectiles.SpiderCave;

namespace Spooky.Content.Items.SpiderCave.OldHunter
{
    public class RustedBullet : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
        {
            Item.damage = 5;
            Item.DamageType = DamageClass.Ranged;
            Item.consumable = true;
            Item.width = 8;
            Item.height = 18;
            Item.knockBack = 1f;
            Item.maxStack = 9999;
            Item.value = Item.buyPrice(silver: 5);
            Item.shoot = ModContent.ProjectileType<RustedBulletProj>();
            Item.ammo = AmmoID.Bullet;
        }
    }
}
