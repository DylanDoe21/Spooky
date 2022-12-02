using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Terraria.Audio;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.Items.SpookyHell
{   
    public class SentientFleshAxe : SwingWeaponBase
    {
        public override int Length => 90;
		public override int TopSize => 35;
		public override float SwingDownSpeed => 18f;
		public override bool CollideWithTiles => true;
        static bool hasHitSomething = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sentient Flesh Mincer");
            Tooltip.SetDefault("Deals far more damage to enemies below half health\nCritical hits will bleed enemies, dealing rapid damage over time");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 120;
            Item.crit = 35;
			Item.DamageType = DamageClass.Melee;
			Item.autoReuse = true;
            Item.width = 88;
            Item.height = 82;
            Item.useTime = 45;
			Item.useAnimation = 45;
			Item.useStyle = SwingUseStyle;
			Item.knockBack = 12;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 10);
            Item.UseSound = SoundID.DD2_MonkStaffSwing;
            Item.scale = 1.25f;
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

                SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundMiss, player.Center);
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
                target.AddBuff(ModContent.BuffType<SentientAxeBleed>(), 180);
            }
        }
    }
}