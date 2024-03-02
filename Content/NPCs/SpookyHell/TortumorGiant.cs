using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Content.Items.Food;
using Spooky.Content.Items.SpookyHell;
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.NPCs.SpookyHell.Projectiles;

namespace Spooky.Content.NPCs.SpookyHell
{
    public class TortumorGiant : ModNPC
    {
        public int MoveSpeedX = 0;
		public int MoveSpeedY = 0;

        public static readonly SoundStyle ScreechSound = new("Spooky/Content/Sounds/TumorScreech2", SoundType.Sound) { Volume = 0.8f, PitchVariance = 0.6f };

		public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 800;
            NPC.damage = 40;
            NPC.defense = 20;
            NPC.width = 90;
            NPC.height = 92;
            NPC.npcSlots = 5f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 50, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit9;
            NPC.DeathSound = SoundID.NPCDeath22;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyHellBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
                new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.TortumorGiant"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpookyHell/TortumorGiantGlow").Value;

            Main.EntitySpriteDraw(tex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
        }

        public override void FindFrame(int frameHeight)
        {
            //floating animation
            NPC.frameCounter++;
            if (NPC.ai[0] <= 480)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter > 15)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 4)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }

            //screaming animation
            if (NPC.ai[0] > 480)
            {
                if (NPC.frame.Y < 4 * frameHeight)
                {
                    NPC.frame.Y = 4 * frameHeight;
                }

                if (NPC.frameCounter > 10)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 6)
                {
                    NPC.frame.Y = 4 * frameHeight;
                }
            }
        }
        
        public override void AI()
		{
			Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            NPC.rotation = NPC.velocity.X * 0.04f;
			
			if (!NPC.HasBuff(BuffID.Confused))
            {
			    NPC.ai[0]++;  
            }
            else
            {
                NPC.ai[0] = 0;
            } 

            if (NPC.ai[0] <= 480)
            {
                int MaxSpeed = 30;

                if (NPC.HasBuff(BuffID.Confused))
                {
                    MaxSpeed = -30;
                }

                //flies to players X position
                if (NPC.Center.X >= player.Center.X && MoveSpeedX >= -MaxSpeed - 8) 
                {
                    MoveSpeedX--;
                }
                else if (NPC.Center.X <= player.Center.X && MoveSpeedX <= MaxSpeed + 8)
                {
                    MoveSpeedX++;
                }

                NPC.velocity.X = MoveSpeedX * 0.1f;
                
                //flies to players Y position
                if (NPC.Center.Y >= player.Center.Y - 60f && MoveSpeedY >= -MaxSpeed)
                {
                    MoveSpeedY--;
                }
                else if (NPC.Center.Y <= player.Center.Y - 60f && MoveSpeedY <= MaxSpeed)
                {
                    MoveSpeedY++;
                }

                NPC.velocity.Y = MoveSpeedY * 0.1f;
            }

            if (NPC.ai[0] >= 480)
            {
                if (NPC.ai[0] == 480)
                {
                    SoundEngine.PlaySound(ScreechSound, NPC.Center);
                }

                NPC.velocity *= 0.95f;

                //only summon tortumors if no other tortumors exist
                if (!NPC.AnyNPCs(ModContent.NPCType<Tortumor>()))
                {
                    if (NPC.ai[0] == 555)
                    {
                        for (int numSpawns = 0; numSpawns < 2; numSpawns++)
                        {
                            int TumorSummon = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X + Main.rand.Next(-150, 150), 
                            (int)NPC.Center.Y + Main.rand.Next(-150, 150), ModContent.NPCType<Tortumor>());

                            //set tortumor ai to -2 so the dust effect happens
                            Main.npc[TumorSummon].ai[0] = -2;

                            if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.SendData(MessageID.SyncNPC, number: TumorSummon);
                            }
                        }

                        //use npc.ai[1] to prevent projectile shooting attack from running after summoning
                        NPC.ai[1] = 1;
                    }
                }
                else
                {
                    if (NPC.ai[0] >= 550 && NPC.ai[1] == 0)
                    {
                        int[] Projectiles = new int[] { ModContent.ProjectileType<TumorOrb1>(), 
                        ModContent.ProjectileType<TumorOrb2>(), ModContent.ProjectileType<TumorOrb3>() };

                        if (Main.rand.NextBool(3))
                        {
                            SoundEngine.PlaySound(SoundID.Item87, NPC.Center);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {   
                                int TumorOrb = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X + Main.rand.Next(-50, 50), NPC.Center.Y + Main.rand.Next(-50, 50), 
                                0, 0, Main.rand.Next(Projectiles), NPC.damage / 4, 0f, Main.myPlayer, 0f, (float)NPC.whoAmI);
                                Main.projectile[TumorOrb].ai[0] = 179;
                            }
                        }
                    }
                }

                if (NPC.ai[0] >= 580)
                {
                    NPC.ai[0] = 0;
                    NPC.ai[1] = 0;
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TortumorStaff>(), 4));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MonsterBloodVial>(), 100));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EyeChocolate>(), 100));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 4; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/TortumorGiantGore" + numGores).Type);
                    }
                }

                for (int numDusts = 0; numDusts < 45; numDusts++)
                {
                    int newDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, 0f, 0f, 100, default, 2f);
                    Main.dust[newDust].velocity.X *= Main.rand.Next(-12, 12);
                    Main.dust[newDust].velocity.Y *= Main.rand.Next(-12, 12);
                    Main.dust[newDust].noGravity = true;

                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[newDust].scale = 0.5f;
                        Main.dust[newDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
            }
        }
    }
}