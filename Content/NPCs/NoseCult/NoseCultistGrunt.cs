using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;

using Spooky.Content.NPCs.NoseCult.Projectiles;

namespace Spooky.Content.NPCs.NoseCult
{
    public class NoseCultistGrunt : ModNPC  
    {
        public static readonly SoundStyle SneezeSound = new("Spooky/Content/Sounds/Moco/MocoSneeze1", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 11;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 150;
            NPC.damage = 35;
            NPC.defense = 5;
            NPC.width = 34;
			NPC.height = 50;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.5f;
            NPC.HitSound = SoundID.NPCHit48 with { Pitch = -0.1f };
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 3;
			AIType = NPCID.GoblinScout;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.NoseTempleBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.NoseCultistGrunt"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.NoseTempleBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (Main.bloodMoon)
            {
                Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpookyBiome/ZomboidRainBlood").Value;

                var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                    
                Main.EntitySpriteDraw(tex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
            }
		}

        public override void FindFrame(int frameHeight)
        {   
            NPC.frameCounter++;
            if (NPC.localAI[1] <= 0)
            {
                //walking animation
                if (NPC.velocity.Y == 0)
                {
                    if (NPC.frameCounter > 5)
                    {
                        NPC.frame.Y = NPC.frame.Y + frameHeight;
                        NPC.frameCounter = 0;
                    }
                    if (NPC.frame.Y >= frameHeight * 5)
                    {
                        NPC.frame.Y = 0 * frameHeight;
                    }
                }
                //falling frame
                else
                {
                    NPC.frame.Y = 6 * frameHeight;
                }
            }
            //sneezing animation
            else
            {
                if (NPC.frame.Y < frameHeight * 8)
                {
                    NPC.frame.Y = 7 * frameHeight;
                }

                if (NPC.frameCounter > 10)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 11)
                {
                    NPC.localAI[0] = 0;
                    NPC.localAI[1] = 0;
                    NPC.localAI[2] = 0;

                    NPC.frame.Y = 0 * frameHeight;
                }
            }
        }
        
        public override void AI()
		{
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

			NPC.spriteDirection = NPC.direction;

            NPC.localAI[0]++;

            if (NPC.localAI[0] == 1)
            {
                NPC.localAI[3] = Main.rand.Next(360, 480);
            }

            if (NPC.localAI[0] >= NPC.localAI[3] && NPC.velocity.Y == 0)
            {
                NPC.localAI[1] = 1;
            }

            if (NPC.localAI[1] > 0)
            {
                NPC.velocity *= 0;

                if (NPC.frame.Y == 8 * NPC.height && NPC.localAI[2] == 0)
                {
                    SoundEngine.PlaySound(SneezeSound with { Pitch = 0.4f, Volume = 0.5f }, NPC.Center);

                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, NPC.direction == -1 ? -10 : 10, 0, ModContent.ProjectileType<NoseCultistGruntSnot>(), NPC.damage / 4, 0, NPC.target);
                    
                    NPC.localAI[2]++;

                    NPC.netUpdate = true;
                }
            }
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 3; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/NoseCultistGruntGore" + numGores).Type);
                    }
                }
            }
        }
    }
}