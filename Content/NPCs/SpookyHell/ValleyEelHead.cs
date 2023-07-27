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

namespace Spooky.Content.NPCs.SpookyHell
{
    public class ValleyEelHead : ModNPC
    {
        private bool segmentsSpawned;

        Vector2 SavePlayerPosition;

        public static readonly SoundStyle HitSound = new("Spooky/Content/Sounds/EggEvent/EnemyHit", SoundType.Sound);
        public static readonly SoundStyle DeathSound = new("Spooky/Content/Sounds/EggEvent/EnemyDeath", SoundType.Sound);
        public static readonly SoundStyle GrowlSound = new("Spooky/Content/Sounds/ValleyEelGrowl", SoundType.Sound) { PitchVariance = 0.6f };

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                CustomTexturePath = "Spooky/Content/NPCs/SpookyHell/ValleyEelBestiary",
                Position = new Vector2(70f, 0f),
                PortraitPositionXOverride = 55f,
                PortraitPositionYOverride = -3f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //ints
            writer.Write(SavePlayerPosition.X);
            writer.Write(SavePlayerPosition.Y);

            //bools
            writer.Write(segmentsSpawned);

            //floats
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //ints
            SavePlayerPosition.X = reader.ReadInt32();
            SavePlayerPosition.Y = reader.ReadInt32();

            //bools
            segmentsSpawned = reader.ReadBoolean();

            //floats
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 8500;
            NPC.damage = 75;
            NPC.defense = 10;
            NPC.width = 38;
            NPC.height = 46;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 1, 50, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.behindTiles = true;
            NPC.HitSound = HitSound;
            NPC.DeathSound = DeathSound;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[2] { ModContent.GetInstance<Biomes.SpookyHellBiome>().Type, ModContent.GetInstance<Biomes.SpookyHellLake>().Type };
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.9f * bossAdjustment);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.ValleyEel"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellLake>().ModBiomeBestiaryInfoElement)
			});
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glowTex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpookyHell/ValleyEelHeadGlow").Value;

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Rectangle frame = new Rectangle(0, NPC.frame.Y, tex.Width, tex.Height / Main.npcFrameCount[NPC.type]);
            Vector2 origin = new Vector2(tex.Width * 0.5f, tex.Height / Main.npcFrameCount[NPC.type] * 0.5f);
            Main.spriteBatch.Draw(tex, NPC.Center - Main.screenPosition, frame, drawColor, NPC.rotation, origin, NPC.scale, effects, 0);
            Main.spriteBatch.Draw(glowTex, NPC.Center - Main.screenPosition, frame, Color.White, NPC.rotation, origin, NPC.scale, effects, 0);

            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1;

            if (NPC.frameCounter > 6)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0.0;
            }
            if (NPC.frame.Y >= frameHeight * 4)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }
        
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            NPC.spriteDirection = NPC.velocity.X > 0 ? -1 : 1;

            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + 1.57f;

            //despawn if all players are dead or not in the biome
            if (player.dead)
            {
                NPC.localAI[3]++;
                if (NPC.localAI[3] >= 75)
                {
                    NPC.velocity.Y = 35;
                }

                if (NPC.localAI[3] >= 240)
                {
                    NPC.active = false;
                }
            }

            //Create the worm itself
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!segmentsSpawned)
                {
                    NPC.realLife = NPC.whoAmI;
                    int latestNPC = NPC.whoAmI;

                    for (int numSegments = 0; numSegments < 15; numSegments++)
                    {
                        latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), 
                        ModContent.NPCType<ValleyEelBody>(), NPC.whoAmI, 0, latestNPC);                   
                        Main.npc[latestNPC].lifeMax = NPC.lifeMax;
                        Main.npc[latestNPC].realLife = NPC.whoAmI;
                        Main.npc[latestNPC].ai[3] = NPC.whoAmI;
                        NetMessage.SendData(MessageID.SyncNPC, number: latestNPC);
                    }
                    
                    latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), 
                    ModContent.NPCType<ValleyEelTail>(), NPC.whoAmI, 0, latestNPC);                   
                    Main.npc[latestNPC].lifeMax = NPC.lifeMax;
                    Main.npc[latestNPC].realLife = NPC.whoAmI;
                    Main.npc[latestNPC].ai[3] = NPC.whoAmI;
                    NetMessage.SendData(MessageID.SyncNPC, number: latestNPC);

                    segmentsSpawned = true;
                    NPC.netUpdate = true;
                }
            }

            //attacks
            switch ((int)NPC.ai[0])
            {
                //emerging from the water
                case 0:
                {
                    NPC.localAI[0]++;

                    //fly up
                    if (NPC.localAI[0] == 2)
                    {
                        NPC.velocity.Y = -25;
                    }

                    //slow down
                    if (NPC.localAI[0] >= 12)
                    {
                        NPC.velocity *= 0.95f;
                    }

                    //go to attacks
                    if (NPC.localAI[0] >= 35)
                    { 
                        NPC.localAI[0] = 0;
                        NPC.ai[0]++;

                        NPC.netUpdate = true;
                    }

                    break;
                }

                //fly after the player for a bit
                case 1:
                {
                    NPC.localAI[0]++;

                    Movement(player, 12f, 0.2f);

                    if (NPC.localAI[0] >= 300)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[0]++;
                    }

                    break;
                }

                //preform horizontal dashes, then wildly curl towards the player
                case 2:
                {
                    NPC.localAI[0]++;

                    //repeat four times
                    if (NPC.localAI[1] < 4)
                    {
                        //go to the top corner of the player
                        if (NPC.localAI[0] < 50)
                        {
                            Vector2 GoTo = player.Center;
                            GoTo.X += (NPC.Center.X < player.Center.X) ? -400 : 400;

                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 15, 35);
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                        }

                        //charge at the player
                        if (NPC.localAI[0] == 50)
                        {
                            SoundEngine.PlaySound(GrowlSound, NPC.Center);

                            Vector2 ChargeDirection = player.Center - NPC.Center;
                            ChargeDirection.Normalize();
                            ChargeDirection *= 25;
                            NPC.velocity.X = ChargeDirection.X;
                            NPC.velocity.Y = ChargeDirection.Y / 2;
                        }

                        //curl towards the player
                        if (NPC.localAI[0] >= 65 && NPC.localAI[0] <= 130)
                        {
                            NPC.velocity *= 0.995f;

                            double angle = NPC.DirectionTo(player.Center).ToRotation() - NPC.velocity.ToRotation();
                            while (angle > Math.PI)
                            {
                                angle -= 2.0 * Math.PI;
                            }
                            while (angle < -Math.PI)
                            {
                                angle += 2.0 * Math.PI;
                            }

                            if (Math.Abs(angle) > Math.PI / 2)
                            {
                                NPC.localAI[2] = Math.Sign(angle);
                                NPC.velocity = Vector2.Normalize(NPC.velocity) * 25;
                            }

                            NPC.velocity = NPC.velocity.RotatedBy(MathHelper.ToRadians(4f) * NPC.localAI[2]);
                        }

                        //slow down after the charge is done
                        if (NPC.localAI[0] >= 130)
                        {
                            NPC.velocity *= 0.98f;
                        }

                        //loop attack
                        if (NPC.localAI[0] >= 135)
                        {
                            NPC.localAI[0] = 20;
                            NPC.localAI[1]++;
                        }
                    }
                    else
                    {
                        NPC.velocity *= 0.2f;

                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0;
                        NPC.ai[0]++;
                    }

                    break;
                }

                case 3:
                {
                    goto case 1;
                }

                //go to the side of the player, charge towards them, then attempt to circle them, then charge again
                case 4:
                {
                    NPC.localAI[0]++;

                    //go to the top corner of the player
                    if (NPC.localAI[0] < 60)
                    {
                        Vector2 GoTo = player.Center;
                        GoTo.Y -= 750;

                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 15, 35);
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                    }

                    //charge at the player and save their position
                    if (NPC.localAI[0] == 60)
                    {
                        SoundEngine.PlaySound(GrowlSound, NPC.Center);

                        Vector2 ChargeDirection = player.Center - NPC.Center;
                        ChargeDirection.Normalize(); 
                        ChargeDirection *= 35;
                        NPC.velocity.X = ChargeDirection.X / 2;
                        NPC.velocity.Y = ChargeDirection.Y;
                    }

                    //curl towards the saved player location
                    if (NPC.localAI[0] > 60 && NPC.localAI[0] < 280)
                    {
                        double angle = NPC.DirectionTo(player.Center).ToRotation() - NPC.velocity.ToRotation();
                        while (angle > Math.PI)
                        {
                            angle -= 2.0 * Math.PI;
                        }
                        while (angle < -Math.PI)
                        {
                            angle += 2.0 * Math.PI;
                        }

                        if (Math.Abs(angle) > Math.PI / 2)
                        {
                            NPC.localAI[2] = Math.Sign(angle);
                            NPC.velocity = Vector2.Normalize(NPC.velocity) * 55;
                        }

                        NPC.velocity = NPC.velocity.RotatedBy(MathHelper.ToRadians(7f) * NPC.localAI[2]);
                    }

                    //slow down
                    if (NPC.localAI[0] >= 280 && NPC.localAI[0] < 300)
                    {
                        NPC.velocity *= 0.85f;
                    }

                    //save player position
                    if (NPC.localAI[0] == 290)
                    {
                        SavePlayerPosition = player.Center;
                    }

                    //charge again while circling
                    if (NPC.localAI[0] == 300)
                    {
                        SoundEngine.PlaySound(GrowlSound, NPC.Center);

                        Vector2 ChargeDirection = SavePlayerPosition - NPC.Center;
                        ChargeDirection.Normalize(); 
                        ChargeDirection *= 45;
                        NPC.velocity = ChargeDirection;
                    }

                    //slow down again
                    if (NPC.localAI[0] >= 320)
                    {
                        NPC.velocity *= 0.88f;
                    }

                    //loop attack
                    if (NPC.localAI[0] >= 350)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[0] = 1;
                    }

                    break;
                }
            }
        }

        //goofy ahh modified vanilla worm movement code
        private void Movement(Player player, float maxSpeed, float accel)
        {
            bool collision = false;
            bool FastMovement = false;

            if (Vector2.Distance(NPC.Center, player.Center) >= 500)
            {
                FastMovement = true;
            }
            if (Vector2.Distance(NPC.Center, player.Center) <= 500)           
            {
                FastMovement = false;
            }

            float speed = maxSpeed;
            float acceleration = accel;

            if (!collision)
            {
                Rectangle rectangle1 = new((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height);

                int maxDistance = 12;

                bool playerCollision = true;

                for (int index = 0; index < 255; ++index)
                {
                    if (Main.player[index].active)
                    {
                        Rectangle rectangle2 = new((int)Main.player[index].position.X - maxDistance, 
                        (int)Main.player[index].position.Y - maxDistance, maxDistance * 2, maxDistance * 2);

                        if (rectangle1.Intersects(rectangle2))
                        {
                            playerCollision = false;
                        
                            break;
                        }
                    }
                }

                if (playerCollision)
                {
                    collision = true;
                }
            }

            Vector2 NPCCenter = new(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + NPC.height * 0.5f);
            float targetXPos = player.position.X + (player.width / 2);
            float targetYPos = player.position.Y + (player.height / 2);

            float targetRoundedPosX = (int)(targetXPos / 16.0) * 16;
            float targetRoundedPosY = (int)(targetYPos / 16.0) * 16;
            NPCCenter.X = (int)(NPCCenter.X / 16.0) * 16;
            NPCCenter.Y = (int)(NPCCenter.Y / 16.0) * 16;
            float dirX = targetRoundedPosX - NPCCenter.X;
            float dirY = targetRoundedPosY - NPCCenter.Y;
            float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);
            
            if (!collision)
            {
                NPC.TargetClosest(true);

                if (NPC.velocity.Y > speed)
                {
                    NPC.velocity.Y = speed;
                }
                if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 0.4)
                {
                    if (NPC.velocity.X < 0.0)
                    {
                        NPC.velocity.X -= acceleration * 1.1f;
                    }
                    else
                    {
                        NPC.velocity.X += acceleration * 1.1f;
                    }
                }

                else if (NPC.velocity.Y == speed)
                {
                    if (NPC.velocity.X < dirX)
                    {
                        NPC.velocity.X += acceleration;
                    }
                    else if (NPC.velocity.X > dirX)
                    {
                        NPC.velocity.X -= acceleration;
                    }
                }
                else if (NPC.velocity.Y > 4.0)
                {
                    if (NPC.velocity.X < 0.0)
                    {
                        NPC.velocity.X += acceleration * 1f;
                    }
                    else
                    {
                        NPC.velocity.X -= acceleration * 1f; 
                    }
                }
            }

            if (!collision)
            {
                NPC.TargetClosest(true);
                NPC.velocity.Y = NPC.velocity.Y + 0.11f;

                if (NPC.velocity.Y > speed)
                { 
                    NPC.velocity.Y = speed;
                }

                if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 1.0)
                {
                    if (NPC.velocity.X < 0.0)
                    {
                        NPC.velocity.X -= acceleration * 1.1f;
                    }
                    else
                    {
                        NPC.velocity.X += acceleration * 1.4f;
                    }
                }
                
                if (NPC.velocity.Y > 5.0) 
                {
                    if (NPC.velocity.X < 0.0)
                    {
                        NPC.velocity.X += acceleration * 0.9f;
                    }
                    else
                    {
                        NPC.velocity.X -= acceleration * 0.9f;
                    }
                }
            }
            else if (collision || FastMovement)
            {
                float absDirX = Math.Abs(dirX);
                float absDirY = Math.Abs(dirY);
                float newSpeed = speed / length;
                dirX *= newSpeed;
                dirY *= newSpeed;

                if (NPC.velocity.X > 0.0 && dirX > 0.0 || NPC.velocity.X < 0.0 && dirX < 0.0 || (NPC.velocity.Y > 0.0 && dirY > 0.0 || NPC.velocity.Y < 0.0 && dirY < 0.0))
                {
                    if (NPC.velocity.X < dirX)
                    {
                        NPC.velocity.X += acceleration;
                    }
                    else if (NPC.velocity.X > dirX)
                    {
                        NPC.velocity.X -= acceleration;
                    }
                    if (NPC.velocity.Y < dirY)
                    {
                        NPC.velocity.Y += acceleration;
                    }
                    else if (NPC.velocity.Y > dirY)
                    {
                        NPC.velocity.Y -= acceleration;
                    }

                    if (Math.Abs(dirY) < speed * 0.2 && (NPC.velocity.X > 0.0 && dirX < 0.0 || NPC.velocity.X < 0.0 && dirX > 0.0))
                    {
                        if (NPC.velocity.Y > 0.0)
                        {
                            NPC.velocity.Y += acceleration * 2f;
                        }
                        else
                        {
                            NPC.velocity.Y -= acceleration * 2f;
                        }
                    }
                    if (Math.Abs(dirX) < speed * 0.2 && (NPC.velocity.Y > 0.0 && dirY < 0.0 || NPC.velocity.Y < 0.0 && dirY > 0.0))
                    {
                        if (NPC.velocity.X > 0.0)
                        {
                            NPC.velocity.X += acceleration * 2f;
                        }
                        else
                        {
                            NPC.velocity.X -= acceleration * 2f;
                        }
                    }
                }
                else if (absDirX > absDirY)
                {
                    if (NPC.velocity.X < dirX)
                    {
                        NPC.velocity.X += acceleration * 1.1f;
                    }
                    else if (NPC.velocity.X > dirX)
                    {
                        NPC.velocity.X -= acceleration * 1.1f;
                    }
                    if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 0.5)
                    {
                        if (NPC.velocity.Y > 0.0)
                        {
                            NPC.velocity.Y += acceleration;
                        }
                        else
                        {
                            NPC.velocity.Y -= acceleration;
                        }
                    }
                }
                else
                {
                    if (NPC.velocity.Y < dirY)
                    {
                        NPC.velocity.Y += acceleration * 1.1f;
                    }
                    else if (NPC.velocity.Y > dirY)
                    {
                        NPC.velocity.Y -= acceleration * 1.1f;
                    }

                    if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 0.5)
                    {
                        if (NPC.velocity.X > 0.0)
                        {
                            NPC.velocity.X += acceleration;
                        }
                        else
                        {
                            NPC.velocity.X -= acceleration;
                        }
                    }
                }
            }

            //Some netupdate stuff
            if (collision)
            {
                if (NPC.localAI[2] != 1)
                {
                    NPC.netUpdate = true;
                }
                NPC.localAI[2] = 1f;
            }
            else
            {
                if (NPC.localAI[2] != 0.0)
                {
                    NPC.netUpdate = true;
                }
                NPC.localAI[2] = 0.0f;
            }

            if ((NPC.velocity.X > 0.0 && NPC.oldVelocity.X < 0.0 || NPC.velocity.X < 0.0 && NPC.oldVelocity.X > 0.0 || 
            (NPC.velocity.Y > 0.0 && NPC.oldVelocity.Y < 0.0 || NPC.velocity.Y < 0.0 && NPC.oldVelocity.Y > 0.0)) && !NPC.justHit)
            {
                NPC.netUpdate = true;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            //haemorrhaxe
            npcLoot.Add(ItemDropRule.Common(ItemID.BloodHamaxe, 3));

            //drippler crippler
            npcLoot.Add(ItemDropRule.Common(ItemID.DripplerFlail, 3));

            //sanguine staff
            npcLoot.Add(ItemDropRule.Common(ItemID.SanguineStaff, 3));

            //chum buckets
            npcLoot.Add(ItemDropRule.Common(ItemID.ChumBucket, 1, 5, 10));
        }

        public override bool CheckDead()
        {
            if (Main.netMode != NetmodeID.Server) 
            {
                for (int numGores = 1; numGores <= 2; numGores++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity / 2, ModContent.Find<ModGore>("Spooky/ValleyEelHeadGore" + numGores).Type);
                }
            }

            return true;
        }
        
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.2f;
            return null;
        }
    }
}