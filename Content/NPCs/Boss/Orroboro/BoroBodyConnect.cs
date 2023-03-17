using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Boss.Orroboro
{
    public class BoroBodyConnect : ModNPC
    {
        public static readonly SoundStyle HitSound = new("Spooky/Content/Sounds/SpookyHell/EnemyHit", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Boro");
            Main.npcFrameCount[NPC.type] = 5;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);

            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] 
                {
                    BuffID.Confused
                }
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = Main.masterMode ? 55000 / 3 : Main.expertMode ? 45000 / 2 : 35000;
            NPC.damage = 55;
            NPC.defense = 35;
            NPC.width = 75;
            NPC.height = 75;
            NPC.knockBackResist = 0f;
            NPC.behindTiles = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.noGravity = true;
            NPC.HitSound = HitSound;
            NPC.aiStyle = -1;
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			float Divide = 1.8f;

			if (projectile.penetrate <= -1)
			{
				damage /= (int)Divide;
			}
			else if (projectile.penetrate >= 3)
			{
				damage /= (int)Divide;
			}
		}

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1;
            if (NPC.frameCounter > 4)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0.0;
            }
            if (NPC.frame.Y >= frameHeight * 5)
            {
                NPC.frame.Y = 0;
            }
        }

        public override bool PreAI()
        {   
            //go invulnerable and shake during phase 2 transition
            if (NPC.AnyNPCs(ModContent.NPCType<OrroHead>()))
            {
                if (Main.npc[(int)NPC.ai[1]].ai[2] > 0)
                {
                    NPC.immortal = true;
                    NPC.dontTakeDamage = true;
                    NPC.netUpdate = true;
                    NPC.velocity *= 0f;

                    NPC.ai[2]++;

                    NPC.Center = new Vector2(NPC.Center.X, NPC.Center.Y);
                    NPC.Center += Main.rand.NextVector2Square(-2, 2);
                }
            }

            //kill segment if the head doesnt exist
			if (!Main.npc[(int)NPC.ai[1]].active)
            {
                NPC.active = false;
            }
			
			if (NPC.ai[1] < (double)Main.npc.Length)
            {
                Vector2 npcCenter = new(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                float dirX = Main.npc[(int)NPC.ai[1]].position.X + (float)(Main.npc[(int)NPC.ai[1]].width / 2) - npcCenter.X;
                float dirY = Main.npc[(int)NPC.ai[1]].position.Y + (float)(Main.npc[(int)NPC.ai[1]].height / 2) - npcCenter.Y;
                NPC.rotation = (float)Math.Atan2(dirY, dirX) + 1.57f;
                float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);
                float dist = (length - (float)NPC.width) / length;
                float posX = dirX * dist;
                float posY = dirY * dist;
 
                NPC.velocity = Vector2.Zero;
                NPC.position.X = NPC.position.X + posX;
                NPC.position.Y = NPC.position.Y + posY;
            }

			return false;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override bool CheckActive()
        {
            return false;
        }
    }
}
