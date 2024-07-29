using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.NoseCult.Projectiles
{
    public class OrbitingBooger : ModNPC
    {   
        public override string Texture => "Spooky/Content/NPCs/NoseCult/Projectiles/NoseCultistMageSnot";

        private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 150;
            NPC.damage = 35;
            NPC.defense = 0;
            NPC.width = 30;
            NPC.height = 28;
            NPC.npcSlots = 0f;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.Item177;
			NPC.DeathSound = SoundID.NPCDeath1;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.65f * bossAdjustment);
        }
        
        public override void AI()
        {
            NPC Parent = Main.npc[(int)NPC.ai[0]];

            Player player = Main.player[Parent.target];

            if (!Parent.active)
            {
                NPC.active = false;
            }

            NPC.rotation += 0.25f * (float)Parent.direction;

            NPC.ai[1] += 1f * (float)Parent.direction;
            int distance = 100;
            double rad = NPC.ai[1] * (Math.PI / 180);
            NPC.position.X = Parent.Center.X - (int)(Math.Cos(rad) * distance) - NPC.width / 2;
            NPC.position.Y = Parent.Center.Y - (int)(Math.Sin(rad) * distance) - NPC.height / 2;

            //shoot itself as a projectile
            NPC.ai[2]++;

            if (NPC.ai[2] >= 180)
            {
                SoundEngine.PlaySound(SoundID.Item20, NPC.position);

                Vector2 ShootSpeed = player.Center - NPC.Center;
                ShootSpeed.Normalize();
                ShootSpeed.X *= Main.rand.NextFloat(10f, 15f);
                ShootSpeed.Y *= Main.rand.NextFloat(10f, 15f);

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, ShootSpeed, ModContent.ProjectileType<OrbitingBoogerProj>(), NPC.damage / 4, 0f, Main.myPlayer);
				}

                NPC.active = false;
            }
        }
    }
}