using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Dusts;
using Spooky.Content.Items.Pets;
using Spooky.Content.Items.SpookyHell;
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.NPCs.SpookyHell.Projectiles;

namespace Spooky.Content.NPCs.SpookyHell
{
    public class ValleyNautilus : ModNPC
    {
        float SpinMultiplier = 0f;

        bool Charging = false;

        Vector2 SavePosition;
        Vector2 SavePlayerPosition;

        private static Asset<Texture2D> GlowTexture;
        private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.TrailCacheLength[NPC.type] = 8;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Rotation = MathHelper.PiOver2,
                Position = new Vector2(26f, -12f),
                PortraitPositionXOverride = 6f,
                PortraitPositionYOverride = -6f
            };

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //vector2
            writer.WriteVector2(SavePosition);
            writer.WriteVector2(SavePlayerPosition);

            //bools
            writer.Write(Charging);

            //floats
            writer.Write(SpinMultiplier);
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //vector2
			SavePosition = reader.ReadVector2();
            SavePlayerPosition = reader.ReadVector2();

            //bools
            Charging = reader.ReadBoolean();

            //floats
            SpinMultiplier = reader.ReadSingle();
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 10000;
            NPC.damage = 60;
            NPC.defense = 30;
            NPC.width = 144;
            NPC.height = 146;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 3, 0, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit18;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[2] { ModContent.GetInstance<Biomes.SpookyHellBiome>().Type, ModContent.GetInstance<Biomes.SpookyHellLake>().Type };
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            //valley nautilus has high health, so give it special expert mode scaling
            NPC.lifeMax = (int)(NPC.lifeMax * 0.9f * bossAdjustment);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.ValleyNautilus"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellLake>().ModBiomeBestiaryInfoElement)
			});
		}
        
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			if (NPC.ai[0] == 0 || (NPC.ai[0] == 2 && NPC.localAI[0] >= 120))
			{
                NPCTexture ??= ModContent.Request<Texture2D>(Texture);

				Vector2 drawOrigin = new(NPCTexture.Width() * 0.5f, NPC.height * 0.5f);

				for (int oldPos = 0; oldPos < NPC.oldPos.Length; oldPos++)
				{
					var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
					Vector2 drawPos = NPC.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, NPC.gfxOffY + 4);
					Color color = NPC.GetAlpha(Color.Red) * (float)(((float)(NPC.oldPos.Length - oldPos) / (float)NPC.oldPos.Length) / 2);
					spriteBatch.Draw(NPCTexture.Value, drawPos, new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, drawOrigin, NPC.scale, effects, 0f);
				}
			}
            
            return true;
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpookyHell/ValleyNautilusGlow");

            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
        }

        public override void FindFrame(int frameHeight)
        {
            //flying animation
            if (!Charging)
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
            //charging frame
            else
            {
                NPC.frame.Y = 4 * frameHeight;
            }
        }

        public override void AI()
		{
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

            if (NPC.ai[0] != 3 || (NPC.ai[0] == 3 && (NPC.localAI[0] < 100 || NPC.localAI[0] > 350)))
            {
                //set rotation while charging
                if (Charging)
                {
                    NPC.direction = NPC.spriteDirection = NPC.velocity.X > 0f ? -1 : 1;
                
                    NPC.rotation = NPC.velocity.ToRotation();

                    if (NPC.spriteDirection == 1)
                    {
                        NPC.rotation += MathHelper.Pi;
                    }
                }
                //otherwise use EoC style rotation
                else
                {
                    Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
                    float RotateX = player.Center.X - vector.X;
                    float RotateY = player.Center.Y - vector.Y;
                    NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;
                }
            }

            //despawn if all players are dead
            if (player.dead)
            {
                NPC.localAI[2]++;

                NPC.ai[0] = -1;

                Charging = false;

                NPC.velocity.Y = -25;

                if (NPC.localAI[2] >= 75)
                {
                    NPC.active = false;
                }
            }

            switch ((int)NPC.ai[0])
            {
                //emerging from the water
                case 0:
                {
                    NPC.localAI[0]++;

                    //fly up quickly
                    if (NPC.localAI[0] == 2)
                    {
                        NPC.velocity.Y = -35;
                    }

                    //slow down
                    if (NPC.localAI[0] >= 12)
                    {
                        NPC.velocity *= 0.35f;
                    }

                    //go to attacks
                    if (NPC.localAI[0] >= 100)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[0]++;

                        NPC.netUpdate = true;
                    }

                    break;
                }

                //fly towards the player, stop, then rapdily spit a stream of blood clots
                case 1:
                {
                    NPC.localAI[0]++;

                    //save location to go to
                    if (NPC.localAI[0] == 5)
                    {
                        SavePlayerPosition = new Vector2(player.Center.X + Main.rand.Next(-350, 350), player.Center.Y - Main.rand.Next(150, 220));

                        NPC.netUpdate = true;
                    }

                    if (NPC.localAI[0] > 5 && NPC.localAI[0] < 75)
                    {
                        Vector2 GoTo = SavePlayerPosition;

                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 6, Main.rand.Next(8, 15));
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                    }

                    //save npc center
                    if (NPC.localAI[0] == 75)
                    {
                        SoundEngine.PlaySound(SoundID.Item170, NPC.Center);

                        SavePosition = NPC.Center;

                        NPC.netUpdate = true;
                    }

                    //shake before firing projectiles
                    if (NPC.localAI[0] > 75 && NPC.localAI[0] < 100)
                    {
                        NPC.Center = new Vector2(SavePosition.X, SavePosition.Y);
                        NPC.Center += Main.rand.NextVector2Square(-7, 7);
                    }

                    //fire a stream of spit while recoiling
                    if (NPC.localAI[0] > 100 && NPC.localAI[0] < 200) 
                    {
                        Vector2 Recoil = player.Center - NPC.Center;
                        Recoil.Normalize(); 
                        Recoil *= -2;
                        NPC.velocity = Recoil;

                        if (NPC.localAI[0] % 8 == 2)
                        {
                            SoundEngine.PlaySound(SoundID.Item171, NPC.Center);

                            Vector2 ShootSpeed = player.Center - NPC.Center;
                            ShootSpeed.Normalize();
                            ShootSpeed *= 5;

                            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 70f;
                            Vector2 position = new Vector2(NPC.Center.X, NPC.Center.Y);

                            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                            {
                                position += muzzleOffset;
                            }

                            float Spread = Main.rand.Next(-1, 1);

                            int[] Types = new int[] { ModContent.ProjectileType<NautilusSpit1>(), ModContent.ProjectileType<NautilusSpit2>() };

                            NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(position.X, position.Y + 20), new Vector2(ShootSpeed.X + Spread, ShootSpeed.Y + Spread), Main.rand.Next(Types), NPC.damage, 4.5f);
                        }

                        NPC.netUpdate = true;
                    }

                    //stop moving
                    if (NPC.localAI[0] >= 200) 
                    {
                        NPC.velocity *= 0;
                    }

                    if (NPC.localAI[0] >= 250) 
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[0]++;
                        
                        NPC.netUpdate = true;
                    }

                    break;
                }

                //go to a location around the player, shake, then dash quickly multiple times
                case 2:
                {
                    NPC.localAI[0]++;

                    //charge four times
                    if (NPC.localAI[1] < 4)
                    {
                        if (NPC.localAI[0] > 5 && NPC.localAI[0] < 75)
                        {
                            Vector2 GoTo = SavePlayerPosition;

                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 6, Main.rand.Next(8, 15));
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                        }

                        //save npc center
                        if (NPC.localAI[0] == 75)
                        {
                            SoundEngine.PlaySound(SoundID.Item170, NPC.Center);

                            NPC.velocity *= 0;

                            SavePosition = NPC.Center;
                        }

                        //shake before charging
                        if (NPC.localAI[0] > 75 && NPC.localAI[0] < 95)
                        {
                            NPC.Center = new Vector2(SavePosition.X, SavePosition.Y);
                            NPC.Center += Main.rand.NextVector2Square(-7, 7);
                        }

                        //save the players location
                        if (NPC.localAI[0] == 100)
                        {
                            SavePlayerPosition = player.Center;
                        }

                        //charge
                        if (NPC.localAI[0] == 120)
                        {
                            Charging = true;

                            SoundEngine.PlaySound(SoundID.DD2_WyvernDiveDown, NPC.Center);

                            Vector2 ChargeDirection = SavePlayerPosition - NPC.Center;
                            ChargeDirection.Normalize();
                            ChargeDirection *= 55;
                            NPC.velocity = ChargeDirection;

                            for (int numDust = 0; numDust < 45; numDust++)
                            {
                                int newDust = Dust.NewDust(NPC.Center, NPC.width / 2, NPC.height / 2, DustID.Blood, 0f, 0f, 100, default, 1f);
                                Main.dust[newDust].velocity = ChargeDirection * -45;
                                Main.dust[newDust].scale *= Main.rand.NextFloat(2.8f, 3.8f);
                                Main.dust[newDust].noGravity = true;
                            }
                        }

                        //slow down
                        if (NPC.localAI[0] >= 130)
                        {
                            NPC.velocity *= 0.85f;
                        }

                        //repeat charge
                        if (NPC.localAI[0] >= 145)
                        {
                            if (NPC.localAI[1] < 3)
                            {
                                NPC.localAI[0] = 98;
                            }
                            else
                            {
                                NPC.localAI[0] = 0;
                            }

                            NPC.localAI[1]++;

                            NPC.netUpdate = true;
                        }
                    }
                    else
                    {
                        Charging = false;

                        NPC.velocity *= 0.85f;

                        if (NPC.localAI[0] >= 75)
                        {
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0;
                            NPC.ai[0]++;

                            NPC.netUpdate = true;
                        }
                    }

                    break;
                }

                //spin around and fire explosive bubbles all over the place
                case 3:
                {
                    NPC.localAI[0]++;

                    //go above the player
                    if (NPC.localAI[0] > 5 && NPC.localAI[0] < 75)
                    {
                        Vector2 GoTo = new Vector2(player.Center.X, player.Center.Y - 200);

                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 6, Main.rand.Next(8, 15));
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                    }

                    //save npc center
                    if (NPC.localAI[0] == 75)
                    {
                        SoundEngine.PlaySound(SoundID.Item170, NPC.Center);

                        //use localAI[1] for the bubble shoot chance
                        NPC.localAI[1] = 30;

                        NPC.velocity *= 0;

                        SavePosition = NPC.Center;
                    }

                    //shake before spinning
                    if (NPC.localAI[0] > 75 && NPC.localAI[0] < 100)
                    {
                        NPC.Center = new Vector2(SavePosition.X, SavePosition.Y);
                        NPC.Center += Main.rand.NextVector2Square(-7, 7);
                    }
                    
                    //make spin speed accelerate and deaccelerate when needed
                    if (NPC.localAI[0] > 100 && NPC.localAI[0] < 200)
                    {
                        if (NPC.localAI[1] > 2)
                        {
                            NPC.localAI[1] -= 0.2f;
                        }

                        SpinMultiplier += 0.005f;
                    }
                    if (NPC.localAI[0] > 250)
                    {
                        NPC.localAI[1] += 0.2f;

                        SpinMultiplier -= 0.005f;
                    }

                    //spin and shoot out bubbles
                    if (NPC.localAI[0] > 100 && NPC.localAI[0] < 350)
                    {
                        NPC.rotation += (1f * SpinMultiplier) * (float)NPC.direction;

                        //if the player attempts to run too far away while spinning then move towards them, otherwise stay still
                        if (player.Distance(NPC.Center) > 550f)
                        {
                            Vector2 GoTo = player.Center;

                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 8, 12);
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                        }
                        else
                        {
                            NPC.velocity *= 0.2f;
                        }

                        if (Main.rand.NextBool((int)NPC.localAI[1]))
                        {
                            SoundEngine.PlaySound(SoundID.Item95, NPC.Center);

                            int[] Types = new int[] { ModContent.ProjectileType<NautilusBubble1>(), ModContent.ProjectileType<NautilusBubble2>() };

                            NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, new Vector2(Main.rand.Next(-20, 20), Main.rand.Next(-20, 20)), Main.rand.Next(Types), NPC.damage, 0f);
                        }
                    }

                    if (NPC.localAI[0] >= 400)
                    {
                        SpinMultiplier = 0f;
                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0;

                        //switch attacks depending on condition
                        //do not switch to the squid summoning attack if any squid clones exist
                        if (!NPC.AnyNPCs(ModContent.NPCType<ValleySquidClone>()))
                        {
                            NPC.ai[0]++;
                        }
                        else
                        {
                            NPC.ai[0] = 1;
                        }

                        NPC.netUpdate = true;
                    }

                    break;
                }

                //slow down, then summon a large group of valley squids
                case 4:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] > 5 && NPC.localAI[0] < 75)
                    {
                        Vector2 GoTo = new Vector2(player.Center.X, player.Center.Y - 200);

                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 6, Main.rand.Next(8, 15));
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                    }

                    //save npc center
                    if (NPC.localAI[0] == 75)
                    {
                        SoundEngine.PlaySound(SoundID.Item170, NPC.Center);

                        NPC.velocity *= 0;

                        SavePosition = NPC.Center;
                    }

                    //shake before summoning
                    if (NPC.localAI[0] > 75 && NPC.localAI[0] < 100)
                    {
                        NPC.Center = new Vector2(SavePosition.X, SavePosition.Y);
                        NPC.Center += Main.rand.NextVector2Square(-7, 7);
                    }

                    if (NPC.localAI[0] == 100)
                    {
                        SoundEngine.PlaySound(SoundID.Item95, NPC.Center);

                        Vector2 Recoil = player.Center - NPC.Center;
                        Recoil.Normalize(); 
                        Recoil *= -7;
                        NPC.velocity = Recoil;

                        Vector2 ShootSpeed = player.Center - NPC.Center;
                        ShootSpeed.Normalize();
                        ShootSpeed.X *= Main.rand.Next(3, 5);
                        ShootSpeed.Y *= Main.rand.Next(3, 5);

                        Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 70f;
                        Vector2 position = new Vector2(NPC.Center.X, NPC.Center.Y);

                        if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                        {
                            position += muzzleOffset;
                        }

                        NPCGlobalHelper.ShootHostileProjectile(NPC, position, ShootSpeed, ModContent.ProjectileType<NautilusBiomass>(), 0, 0f);
                    }
                    
                    if (NPC.localAI[0] > 100)
                    {
                        NPC.velocity *= 0.8f;
                    }

                    if (NPC.localAI[0] >= 250)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[0] = 1;

                        NPC.netUpdate = true;
                    }

                    break;
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            //sentient heart
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SentientHeart>()));

            //material
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ArteryPiece>(), 1, 7, 15));

            //nautilus pet
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ValleyNautilusShell>(), 5));

            //blood moon monolith
            npcLoot.Add(ItemDropRule.Common(ItemID.BloodMoonMonolith, 2));

            //chum buckets
            npcLoot.Add(ItemDropRule.Common(ItemID.ChumBucket, 1, 5, 10));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 10; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ValleyNautilusGore" + numGores).Type);
                    }
                }
            }
        }
    }
}