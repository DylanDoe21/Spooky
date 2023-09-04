using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Buffs.Minion;
using Spooky.Content.NPCs.Hallucinations;
using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Items.BossSummon
{
    public class CreepyFace : ModItem
    {
        public static readonly SoundStyle ScreamSound = new("Spooky/Content/Sounds/EggEvent/VentricleScream", SoundType.Sound) { Pitch = 0.5f };

        public override void SetDefaults()
        {
            Item.damage = 18;
			Item.mana = 10;                        
            Item.DamageType = DamageClass.Magic;
			Item.noMelee = true;  
			Item.autoReuse = true;                  
            Item.width = 22;
            Item.height = 34;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 3;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 5);
            Item.UseSound = ScreamSound;
			Item.shoot = ModContent.ProjectileType<CreepyFaceSound>();
            Item.shootSpeed = 8f;
        }

        public override bool CanUseItem(Player player)
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<TheFlesh>()))
            {
                return true;
            }

            return false;
        }

        public override bool? UseItem(Player player)
        {
            if (!Flags.encounteredFlesh)
            {
                int type = ModContent.NPCType<TheFlesh>();

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.SpawnOnPlayer(player.whoAmI, type);
                }
                else 
                {
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
                }
            }

            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
            if (Flags.encounteredFlesh)
            {
                Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 35f;
                if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                {
                    position += muzzleOffset;
                }

                Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
            }

			return false;
		}
    }
}