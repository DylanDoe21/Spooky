using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
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
    public class Biojetter : ModNPC
    {
        bool IsFalling = false;
        bool HasSpawnedEyes = false;

        public static readonly SoundStyle FlySound = new("Spooky/Content/Sounds/EggEvent/BiojetterFly", SoundType.Sound) { Volume = 0.35f };
        public static readonly SoundStyle ScreamSound = new("Spooky/Content/Sounds/EggEvent/BiojetterScream", SoundType.Sound) { PitchVariance = 0.6f };
        public static readonly SoundStyle SquishSound = new("Spooky/Content/Sounds/EggEvent/EnemyDeath", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 8;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
                CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/BiojetterBestiary",
                Position = new Vector2(0f, -20f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = -20f
            };

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 2200;
            NPC.damage = 50;
            NPC.defense = 0;
            NPC.width = 152;
            NPC.height = 142;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 2, 0, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.HitSound = SoundID.NPCHit9;
			NPC.DeathSound = SoundID.NPCDeath12;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[2] { ModContent.GetInstance<Biomes.SpookyHellBiome>().Type, ModContent.GetInstance<Biomes.SpookyHellEventBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type], quickUnlock: true);

			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Biojetter"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellEventBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 7)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 8)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override void AI()
		{
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.rotation = NPC.velocity.X * 0.02f;

            if (!HasSpawnedEyes)
            {
                for (int numEyes = 0; numEyes < 5; numEyes++)
                {
                    Vector2 PositionGoTo = Vector2.Zero;

                    if (numEyes == 0) PositionGoTo = new Vector2(Main.rand.Next(-100, -75), Main.rand.Next(0, 30));
                    if (numEyes == 1) PositionGoTo = new Vector2(Main.rand.Next(-80, -65), Main.rand.Next(-60, -50));
                    if (numEyes == 2) PositionGoTo = new Vector2(Main.rand.Next(-20, 20), Main.rand.Next(-100, -75));
                    if (numEyes == 3) PositionGoTo = new Vector2(Main.rand.Next(65, 80), Main.rand.Next(-60, -50));
                    if (numEyes == 4) PositionGoTo = new Vector2(Main.rand.Next(75, 100), Main.rand.Next(0, 30));

                    int Eye = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (int)PositionGoTo.X, (int)NPC.Center.Y + (int)PositionGoTo.Y, ModContent.NPCType<BiojetterEye>(), ai0: PositionGoTo.X, ai1: PositionGoTo.Y, ai2: NPC.whoAmI);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NetMessage.SendData(MessageID.SyncNPC, number: Eye);
                    }
                }

                HasSpawnedEyes = true;
            }

            if (NPC.ai[1] >= 5)
            {
                NPC.immortal = false;
                NPC.dontTakeDamage = false;

                Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.ProjectileType<BiojetterDeath>(), NPC.damage, 0, NPC.target);

                player.ApplyDamageToNPC(NPC, NPC.lifeMax * 2, 0, 0, false);
            }

            switch ((int)NPC.ai[0])
            {
                //fly above the player
                case 0:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] <= 420 && !IsFalling)
                    {
                        NPC.localAI[1]--;

                        if (NPC.localAI[0] % 10 == 0)
                        {
                            SoundEngine.PlaySound(FlySound, NPC.Center);
                        }

                        for (int i = 0; i < 2; i++)
                        {
                            int direction = i * 2 - 1;
                            Vector2 rotationOrigin = new Vector2(-direction, 0f);
                            float overrideRotation = rotationOrigin.ToRotation();
                            Vector2 dustVelo = new Vector2(0, 0).RotatedBy(overrideRotation);
                            Vector2 fromBody = NPC.Center + new Vector2(direction * (NPC.width / 5) - 5, 57).RotatedBy(NPC.rotation);
                            int index = Dust.NewDust(fromBody + dustVelo * NPC.scale, 0, 0, DustID.KryptonMoss, 0, 0, 0, Color.White, 2.5f);
                            Dust dust = Main.dust[index];
                            dust.noGravity = true;
                            dust.fadeIn = 0.1f;
                            dust.velocity = dustVelo;
                            dust.velocity.Y += 3;
                            dust.scale += 0.3f;
                            dust.scale *= NPC.scale;
                            dust.alpha = NPC.alpha;
                        }

                        Vector2 GoTo = new Vector2(player.Center.X, player.Center.Y - 350);

                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 6, 12);
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                        
                        if (NPC.velocity.Y > 0 && NPC.localAI[1] <= 0)
                        {
                            IsFalling = true;
                            NPC.noGravity = false;
                        }
                    }

                    if (IsFalling)
                    {
                        NPC.velocity.X *= 0.97f;

                        if (NPC.position.Y >= player.Center.Y - 150)
                        {
                            IsFalling = false;
                            NPC.noGravity = true;

                            NPC.localAI[1] = 120;
                        }
                    }

                    if (NPC.localAI[0] > 420)
                    {
                        IsFalling = false;
                        NPC.noGravity = true;

                        NPC.localAI[0] = 0;
                        //NPC.ai[0]++;

                        NPC.netUpdate = true;
                    }

                    break;
                }

                //spit out giant biomass chunk
                case 1:
                {
                    NPC.localAI[0]++;

                    NPC.velocity *= 0.82f;

                    if (NPC.localAI[0] % 10 == 0)
                    {
                        SoundEngine.PlaySound(FlySound, NPC.Center);
                    }

                    for (int i = 0; i < 2; i++)
                    {
                        int direction = i * 2 - 1;
                        Vector2 rotationOrigin = new Vector2(-direction, 0f);
                        float overrideRotation = rotationOrigin.ToRotation();
                        Vector2 dustVelo = new Vector2(0, 0).RotatedBy(overrideRotation);
                        Vector2 fromBody = NPC.Center + new Vector2(direction * (NPC.width / 5) - 5, 57).RotatedBy(NPC.rotation);
                        int index = Dust.NewDust(fromBody + dustVelo * NPC.scale, 0, 0, DustID.KryptonMoss, 0, 0, 0, Color.White, 2.5f);
                        Dust dust = Main.dust[index];
                        dust.noGravity = true;
                        dust.fadeIn = 0.1f;
                        dust.velocity = dustVelo;
                        dust.velocity.Y += 3;
                        dust.scale += 0.3f;
                        dust.scale *= NPC.scale;
                        dust.alpha = NPC.alpha;
                    }

                    if (NPC.localAI[0] <= 60)
                    {
                        Vector2 GoTo = new Vector2(player.Center.X, player.Center.Y - 350);

                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 6, 12);
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                    }
 
                    if (NPC.localAI[0] == 60)
                    {
                        SoundEngine.PlaySound(ScreamSound, NPC.Center);
                    }

                    //do squishing animation before spitting
                    if (NPC.localAI[0] == 60 || NPC.localAI[0] == 90 || NPC.localAI[0] == 120)
                    {
                        SoundEngine.PlaySound(SquishSound, NPC.Center);

                        NPC.velocity.Y = -4;
                    }
                    else
                    {
                        NPC.velocity *= 0.1f;
                    }

                    //spit out goo
                    if (NPC.localAI[0] == 120)
                    {
                    }

                    if (NPC.localAI[0] > 160)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[0] = 0;

                        NPC.netUpdate = true;
                    }

                    break;
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            //npcLoot.Add(ItemDropRule.ByCondition(new DropConditions.PostOrroboroCondition(), ModContent.ItemType<ArteryPiece>(), 3, 1, 3));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                //Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.ProjectileType<BiojetterDeath>(), NPC.damage, 0, NPC.target);
            }
        }
    }
}