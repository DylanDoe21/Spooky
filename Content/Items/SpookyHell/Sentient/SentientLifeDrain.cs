using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Projectiles;
using Spooky.Content.Projectiles.Sentient;
using Spooky.Content.Tiles.SpookyHell.Furniture;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientLifeDrain : ModItem, ICauldronOutput
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 35;
            Item.mana = 10;
			Item.DamageType = DamageClass.Magic;
			Item.autoReuse = true;
            Item.noMelee = true;
            Item.channel = true;
            Item.width = 46;
            Item.height = 48;
            Item.useTime = 25;
			Item.useAnimation = 25;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 0;
            Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 45);
            Item.shoot = ModContent.ProjectileType<Blank>();
            Item.shootSpeed = 0f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.GetModPlayer<SpookyPlayer>().SoulDrainCharge < 20)
            {
                int[] Types = new int[] { ModContent.ProjectileType<DrainedSoulHealth>(), ModContent.ProjectileType<DrainedSoulMana>() };

                //only up to ten enemies can be drained at once to prevent the aura from being completely broken
                for (int target = 0; target < Main.maxNPCs; target++)
                {
                    NPC npc = Main.npc[target];
                    Vector2 newPosition = new Vector2(Main.npc[target].Center.X, Main.npc[target].Center.Y);

                    if (npc.Distance(Main.MouseWorld) <= 150f && npc.active && !npc.friendly && !npc.dontTakeDamage && !NPCID.Sets.CountsAsCritter[npc.type])
                    {
                        SoundEngine.PlaySound(SoundID.DD2_DarkMageCastHeal, npc.Center);

                        Projectile.NewProjectile(source, newPosition.X, newPosition.Y, 0, 0, Main.rand.Next(Types), damage, knockback, player.whoAmI);
                    }
                }
            }
            else
            {
                Projectile.NewProjectile(source, player.Center.X, player.Center.Y, 0, 0, 
                ModContent.ProjectileType<SoulDrainShockwave>(), damage * 3, knockback, player.whoAmI);

                player.GetModPlayer<SpookyPlayer>().SoulDrainCharge = 0;
            }

            return false;
        }
    }
}