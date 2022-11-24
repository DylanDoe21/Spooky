using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Terraria.Audio;

using Spooky.Core;

namespace Spooky.Content.Items.SpookyHell
{   
    public class SentientFleshAxe : SwingWeaponBase
    {
        public override int Length => 65;
		public override int TopSize => 30;
		public override float SwingDownSpeed => 15f;
		public override bool CollideWithTiles => true;
        static bool hasHitSomething = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sentient Flesh Mincer");
            Tooltip.SetDefault("Fires out a short range bloody wave on use\nDeals far more damage to enemies below half health\nCritical hits will bleed enemies");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 100;
			Item.DamageType = DamageClass.Melee;
			Item.autoReuse = true;
            Item.width = 74;
            Item.height = 60;
            Item.useTime = 40;
			Item.useAnimation = 40;
			Item.useStyle = SwingUseStyle;
			Item.knockBack = 12;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 10);
            Item.UseSound = SoundID.DD2_MonkStaffSwing;
            Item.scale = 1.3f;
        }

        public override void UseAnimation(Player player)
        {
            hasHitSomething = false;
        }

        public override void OnHitTiles(Player player)
        {
            if (!hasHitSomething)
            {
                hasHitSomething = true;

                SpookyPlayer.ScreenShakeAmount = 8;

                SoundEngine.PlaySound(SoundID.Dig, player.Center);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            if (target.life <= target.lifeMax * 0.5)
            {
                target.takenDamageMultiplier = 1.65f;
            }

            if (crit)
            {
                target.AddBuff(BuffID.Bleeding, 180);
            }
        }
    }
}