using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.NPCs.EggEvent.Projectiles;

namespace Spooky.Content.NPCs.EggEvent
{
    public class Crux : ModNPC
    {
        int aura;

        int repeats = Main.rand.Next(1, 4);

        Vector2 SavePlayerPosition;

        public static readonly SoundStyle HitSound = new("Spooky/Content/Sounds/EggEvent/EnemyHit", SoundType.Sound);
        public static readonly SoundStyle DeathSound = new("Spooky/Content/Sounds/EggEvent/EnemyDeath", SoundType.Sound);
        public static readonly SoundStyle ScreamSound = new("Spooky/Content/Sounds/EggEvent/CruxScream", SoundType.Sound) { PitchVariance = 0.6f };

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Position = new Vector2(12f, 5f),
                PortraitPositionXOverride = 6f,
                PortraitPositionYOverride = 0f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //ints
            writer.Write(aura);
            writer.Write(repeats);

            //floats
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //ints
            aura = reader.ReadInt32();
            repeats = reader.ReadInt32();

            //floats
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 1000;
            NPC.damage = 45;
            NPC.defense = 15;
            NPC.width = 116;
            NPC.height = 130;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = HitSound;
			NPC.DeathSound = DeathSound;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[2] { ModContent.GetInstance<Biomes.SpookyHellBiome>().Type, ModContent.GetInstance<Biomes.SpookyHellEventBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Crux"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellEventBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/EggEvent/CruxGlow").Value;

            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(tex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 6)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 4)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override bool CheckActive()
        {
            return !EggEventWorld.EggEventActive;
        }

        public override void AI()
		{
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);
            
            NPC.spriteDirection = NPC.direction;

            switch ((int)NPC.ai[0])
            {
                //fly to random locations, shoot blood bolts
                case 0:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[1] < repeats)
                    {
                        if (NPC.localAI[0] == 5)
                        {
                            SavePlayerPosition = new Vector2(player.Center.X + Main.rand.Next(-200, 200), player.Center.Y - 250);
                        }

                        //go to a random location
                        if (NPC.localAI[0] > 50 && NPC.localAI[0] < 100)
                        {	
                            Vector2 GoTo = SavePlayerPosition;

                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 5, 12);
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                        }

                        //shoot blood bolts
                        if (NPC.localAI[0] > 100 && NPC.localAI[0] < 135) 
                        {
                            if (Main.rand.NextBool(7))
                            {
                                Vector2 ShootSpeed = player.Center - NPC.Center;
                                ShootSpeed.Normalize();
                                ShootSpeed *= 12f;

                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y + 50, ShootSpeed.X, 
                                ShootSpeed.Y, ModContent.ProjectileType<CruxBloodChunk>(), NPC.damage / 5, 0, NPC.target);
                            }

                            NPC.velocity *= 0.2f;
                        }

                        if (NPC.localAI[0] > 165)
                        {
                            NPC.localAI[0] = 0;
                            NPC.localAI[1]++;

                            NPC.netUpdate = true;
                        }
                    }
                    else
                    {
                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0;
                        NPC.ai[0]++;

                        NPC.netUpdate = true;
                    }

                    break;
                }

                //fly at the player, create debuff aura
                case 1:
                {
                    NPC.localAI[0]++;

                    Vector2 GoTo = player.Center;

                    float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 2, 7);
                    NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);

                    //play sound, spawn aura
                    if (NPC.localAI[0] == 60)
                    {
                        SoundEngine.PlaySound(ScreamSound, NPC.Center);

                        aura = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 0, 0, 
                        ModContent.ProjectileType<CruxAura>(), 0, 1, NPC.target, 0, 0);
                    }

                    //make sure the aura is always on the center of the crux
                    if (NPC.localAI[0] > 60)
                    {
                        NPC.velocity *= 0.99f;

                        if (Main.projectile[aura].type == ModContent.ProjectileType<CruxAura>() && aura != 0)
                        {
                            Main.projectile[aura].position = NPC.Center - new Vector2(Main.projectile[aura].width / 2, Main.projectile[aura].height / 2);
                        }
                    }

                    if (NPC.localAI[0] > 240)
                    {
                        aura = 0;

                        NPC.velocity *= 0;

                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0;
                        NPC.ai[0] = 0;

                        NPC.netUpdate = true;
                    }

                    break;
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.ByCondition(new DropConditions.PostOrroboroCondition(), ModContent.ItemType<ArteryPiece>(), 3, 1, 3));
        }
        
        public override bool CheckDead() 
		{
            if (Main.projectile[aura].type == ModContent.ProjectileType<CruxAura>() && aura != 0)
            {
                Main.projectile[aura].Kill();
            }

            return true;
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 4; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/CruxGore" + numGores).Type);
                    }
                }
            }
        }
    }
}