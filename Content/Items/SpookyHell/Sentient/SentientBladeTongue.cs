using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles;
using Spooky.Content.Projectiles.Sentient;
using Spooky.Content.Tiles.SpookyHell.Furniture;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientBladeTongue : ModItem, ICauldronOutput
    {
        public static readonly SoundStyle SlurpSound = new("Spooky/Content/Sounds/Slurp", SoundType.Sound) { PitchVariance = 0.6f };

        public override void SetDefaults()
        {
            Item.damage = 65;
			Item.DamageType = DamageClass.Melee;
			Item.autoReuse = true;
            Item.noMelee = false;
            Item.noUseGraphic = false;
            Item.width = 92;
            Item.height = 86;
            Item.useTime = 35;
			Item.useAnimation = 35;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 12;
            Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 15);
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<Blank>();
			Item.shootSpeed = 0f;
        }

        public override bool AltFunctionUse(Player player)
		{
			return true;
		}

        public override bool CanUseItem(Player player)
		{
			if (player.ownedProjectileCounts[ModContent.ProjectileType<SentientBladeTongueProj>()] > 0) 
			{
                return false;
            }

			return true;
		}

        public override void UseAnimation(Player player)
		{
			if (player.altFunctionUse == 2)
			{
                Item.noMelee = true;
                Item.noUseGraphic = true;
                Item.shoot = ModContent.ProjectileType<SentientBladeTongueProj>();
                Item.shootSpeed = 0f;
            }
			else
			{
                Item.noMelee = false;
                Item.noUseGraphic = false;
                Item.shoot = ModContent.ProjectileType<Blank>();
				Item.shootSpeed = 0f;
			}
		}

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(3))
            {
                target.AddBuff(BuffID.Confused, 180);
            }

            for (int numDusts = 0; numDusts < 30; numDusts++)
			{
                int dustGore = Dust.NewDust(target.Center, target.width / 2, target.height / 2, 103, 0f, -2f, 0, default, 1.5f);
                Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-5f, 5f);
                Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-10f, 10f);
                Main.dust[dustGore].noGravity = true;
            }
        }
    }
}