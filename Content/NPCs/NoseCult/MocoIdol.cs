using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Spooky.Content.Biomes;
using Spooky.Content.Tiles.NoseTemple.Furniture;

namespace Spooky.Content.NPCs.NoseCult
{
    public class MocoIdol1 : ModNPC  
    {
        public override string Texture => "Spooky/Content/NPCs/NoseCult/MocoIdol";

        public bool AnyCultistsExist = false;
        public bool Shake = false;

        private static Asset<Texture2D> NPCTexture;

        public static readonly SoundStyle DeathSound = new("Spooky/Content/Sounds/Moco/MocoIdolShatter", SoundType.Sound);

        public override void SetStaticDefaults()
		{
			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
		}
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 5;
            NPC.width = 68;
			NPC.height = 64;
            NPC.npcSlots = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.immortal = true;
			NPC.dontTakeDamage = true;
            NPC.dontCountMe = true;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.ai[1] > 0)
            {
                NPCTexture ??= ModContent.Request<Texture2D>(Texture);

                float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.4f / 2.4f * 6f)) / 2f + 0.5f;

                Vector2 drawPosition = new Vector2(NPC.Center.X, NPC.Center.Y) - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4);
                Color color = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.Lime);

                for (int repeats = 0; repeats < 4; repeats++)
                {
                    Vector2 afterImagePosition = new Vector2(NPC.Center.X, NPC.Center.Y) + NPC.rotation.ToRotationVector2() - screenPos + new Vector2(0, NPC.gfxOffY + 4) - NPC.velocity * repeats;
                    Main.spriteBatch.Draw(NPCTexture.Value, afterImagePosition, NPC.frame, color * fade, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale * 1.2f, SpriteEffects.None, 0f);
                }

                Main.spriteBatch.Draw(NPCTexture.Value, drawPosition, NPC.frame, color, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale * 1.2f, SpriteEffects.None, 0f);
            }

            //draw sparkle
            if (NPC.ai[2] > 65)
            {
                Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y + 10) - Main.screenPosition;
                float time = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 0.5f / 2.5f * 150f)) / 2f + (NPC.ai[3] / 50);

                DrawPrettyStarSparkle(NPC.Opacity, SpriteEffects.None, vector, Color.White, Color.White, 0.5f, 0f, 0.5f, 0.5f, 1f, 0f, new Vector2(5f * time, 4f * time), new Vector2(2, 2));
                DrawPrettyStarSparkle(NPC.Opacity, SpriteEffects.None, vector, Color.Lime, Color.Lime, 0.5f, 0f, 0.5f, 0.5f, 1f, 90f, new Vector2(5f * time, 4f * time), new Vector2(2, 2));
            }
        }

        private static void DrawPrettyStarSparkle(float opacity, SpriteEffects dir, Vector2 drawpos, Color drawColor, Color shineColor, float flareCounter, float fadeInStart, float fadeInEnd, float fadeOutStart, float fadeOutEnd, float rotation, Vector2 scale, Vector2 fatness) 
        {
			Texture2D Texture = TextureAssets.Extra[98].Value;
			Color color = shineColor * opacity * 0.5f;
			color.A = (byte)0;
			Vector2 origin = Texture.Size() / 2f;
			Color color2 = drawColor * 0.5f;
			float Intensity = Utils.GetLerpValue(fadeInStart, fadeInEnd, flareCounter, clamped: true) * Utils.GetLerpValue(fadeOutEnd, fadeOutStart, flareCounter, clamped: true);
			Vector2 vector = new Vector2(fatness.X * 0.5f, scale.X) * Intensity;
			Vector2 vector2 = new Vector2(fatness.Y * 0.5f, scale.Y) * Intensity;
			color *= Intensity;
			color2 *= Intensity;
			Main.EntitySpriteDraw(Texture, drawpos, null, color, (float)Math.PI / 2f + rotation, origin, vector, dir);
			Main.EntitySpriteDraw(Texture, drawpos, null, color, 0f + rotation, origin, vector2, dir);
			Main.EntitySpriteDraw(Texture, drawpos, null, color2, (float)Math.PI / 2f + rotation, origin, vector * 0.6f, dir);
			Main.EntitySpriteDraw(Texture, drawpos, null, color2, 0f + rotation, origin, vector2 * 0.6f, dir);
		}

		public void ActivateLightTiles()
		{
			int NPCX = (int)NPC.Center.X / 16;
			int NPCY = (int)NPC.Center.Y / 16;

            int Width = NPC.type == ModContent.NPCType<MocoIdol6>() ? 45 : 35;
            int StartHeight = NPC.type == ModContent.NPCType<MocoIdol6>() ? 60 : 20;

			for (int i = NPCX - Width; i <= NPCX + Width; i++)
			{
				for (int j = NPCY - StartHeight; j <= NPCY + 20; j++)
				{
                    Tile tile = Framing.GetTileSafely(i, j);

					if (tile.TileType == ModContent.TileType<CultistCandelabra>() && tile.TileFrameX > 18)
					{
						tile.TileFrameX -= 36;
					}

					if (tile.TileType == ModContent.TileType<CultistCandle>() && tile.TileFrameX > 0)
					{
						tile.TileFrameX -= 18;
					}

					if (tile.TileType == ModContent.TileType<CultistChandelier>() && tile.TileFrameX > 36)
					{
						tile.TileFrameX -= 54;
					}

					if (tile.TileType == ModContent.TileType<CultistLamp>() && tile.TileFrameX > 0)
					{
						tile.TileFrameX -= 18;
					}

					if (tile.TileType == ModContent.TileType<CultistLantern>() && tile.TileFrameX > 0)
					{
						tile.TileFrameX -= 18;
					}
				}
			}
		}

        //check if any player is in the range to activate the range
        public bool AnyPlayersInRange()
        {
            Rectangle CollisionRectangle = new Rectangle((int)NPC.Center.X - 525, (int)NPC.Center.Y - 180, 1050, 300);

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];

                //int playerCount = 0;
                int playerInEventCount = 0;

                if (player.active && !player.dead && player.Hitbox.Intersects(CollisionRectangle))
                {
                    playerInEventCount++;
                }

                if (playerInEventCount >= 1)
                {
                    return true;
                }
            }

            return false;
        }

        public bool AnyPlayersInBiome()
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];

                int playerInBiomeCount = 0;

                if (player.active && !player.dead && player.InModBiome(ModContent.GetInstance<NoseTempleBiome>()))
                {
                    playerInBiomeCount++;
                }

                if (playerInBiomeCount >= 1)
                {
                    return true;
                }
            }

            return false;
        }

        //handle all of the cultist enemy spawn and event varaibles
        public void HandleCultistAmbush()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            //bool to check if any cultist enemies exist
            AnyCultistsExist = NPC.AnyNPCs(ModContent.NPCType<NoseCultistBrute>()) || NPC.AnyNPCs(ModContent.NPCType<NoseCultistGrunt>()) || NPC.AnyNPCs(ModContent.NPCType<NoseCultistGunner>()) || 
            NPC.AnyNPCs(ModContent.NPCType<NoseCultistMage>()) || NPC.AnyNPCs(ModContent.NPCType<NoseCultistWinged>()) || NPC.AnyNPCs(ModContent.NPCType<NoseCultistLeader>());

            if (AnyPlayersInRange() && NPC.ai[1] == 0)
            {
                //activate every single nose cultist attatched to this altar
                NPC.ai[1] = 1;
                
                NoseCultAmbushWorld.AmbushActive = true;

                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }

                NPC.netUpdate = true;
            }

            if (NPC.ai[1] > 0 && NoseCultAmbushWorld.AmbushActive)
			{
                //make sure the event doesnt end unless no players are in the biome
                if (!AnyPlayersInBiome())
                {
					NoseCultAmbushWorld.AmbushActive = false;

					if (Main.netMode == NetmodeID.Server)
					{
						NetMessage.SendData(MessageID.WorldData);
					}

					NPC.active = false;
                }

                if (!AnyCultistsExist)
                {
                    NPC.ai[2]++;

                    if (NPC.ai[2] > 70)
                    {
                        //the minibosses idol is just an invisible spawner due to the statue in the big arena being used not only to rematch him, but spawn waves of cultists for farming
                        //because of this, unlike the other idols, it should just immediately vanish when the miniboss is killed instead of playing an animation
                        if (NPC.type == ModContent.NPCType<MocoIdol6>())
                        {
                            ActivateLightTiles();

                            if (Main.netMode != NetmodeID.SinglePlayer)
                            {
                                ModPacket packet = Mod.GetPacket();
                                packet.Write((byte)SpookyMessageType.MocoIdolDowned6);
                                packet.Send();
                            }
                            else
                            {
                                Flags.downedMocoIdol6 = true;
                            }

                            NoseCultAmbushWorld.AmbushActive = false;

                            if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.SendData(MessageID.WorldData);
                            }

                            NPC.active = false;
                            NPC.netUpdate = true;
                        }

                        NPC.position.Y--;

                        NPC.ai[3] += 0.05f;

                        if (Shake)
                        {
                            NPC.rotation += NPC.ai[3] / 20;
                            if (NPC.rotation > 0.5f)
                            {
                                Shake = false;
                            }
                        }
                        else
                        {
                            NPC.rotation -= NPC.ai[3] / 20;
                            if (NPC.rotation < -0.5f)
                            {
                                Shake = true;
                            }
                        }

                        if (NPC.ai[2] > 245)
                        {
                            SoundEngine.PlaySound(DeathSound, NPC.Center);

                            if (NPC.type == ModContent.NPCType<MocoIdol1>())
                            {
                                if (Main.netMode != NetmodeID.SinglePlayer)
                                {
                                    ModPacket packet = Mod.GetPacket();
                                    packet.Write((byte)SpookyMessageType.MocoIdolDowned1);
                                    packet.Send();
                                }
                                else
                                {
                                    Flags.downedMocoIdol1 = true;
                                }
                            }
                            if (NPC.type == ModContent.NPCType<MocoIdol2>())
                            {
                                if (Main.netMode != NetmodeID.SinglePlayer)
                                {
                                    ModPacket packet = Mod.GetPacket();
                                    packet.Write((byte)SpookyMessageType.MocoIdolDowned2);
                                    packet.Send();
                                }
                                else
                                {
                                    Flags.downedMocoIdol2 = true;
                                }
                            }
                            if (NPC.type == ModContent.NPCType<MocoIdol3>())
                            {
                                if (Main.netMode != NetmodeID.SinglePlayer)
                                {
                                    ModPacket packet = Mod.GetPacket();
                                    packet.Write((byte)SpookyMessageType.MocoIdolDowned3);
                                    packet.Send();
                                }
                                else
                                {
                                    Flags.downedMocoIdol3 = true;
                                }
                            }
                            if (NPC.type == ModContent.NPCType<MocoIdol4>())
                            {
                                if (Main.netMode != NetmodeID.SinglePlayer)
                                {
                                    ModPacket packet = Mod.GetPacket();
                                    packet.Write((byte)SpookyMessageType.MocoIdolDowned4);
                                    packet.Send();
                                }
                                else
                                {
                                    Flags.downedMocoIdol4 = true;
                                }
                            }
                            if (NPC.type == ModContent.NPCType<MocoIdol5>())
                            {
                                if (Main.netMode != NetmodeID.SinglePlayer)
                                {
                                    ModPacket packet = Mod.GetPacket();
                                    packet.Write((byte)SpookyMessageType.MocoIdolDowned5);
                                    packet.Send();
                                }
                                else
                                {
                                    Flags.downedMocoIdol5 = true;
                                }
                            }

                            ActivateLightTiles();

                            //spawn gores
                            for (int numGores = 1; numGores <= 6; numGores++)
                            {
                                if (Main.netMode != NetmodeID.Server) 
                                {
                                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(Main.rand.Next(-10, 10), Main.rand.Next(-5, -2)), ModContent.Find<ModGore>("Spooky/MocoIdolGore" + numGores).Type);
                                }
                            }

                            //spawn dusts
                            for (int numDusts = 0; numDusts < 45; numDusts++)
                            {
                                int dustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GemEmerald, 0f, -2f, 0, default, 3f);
                                Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-16f, 16f);
                                Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-4f, 4f);
                                Main.dust[dustGore].noGravity = true;
                            }

                            NoseCultAmbushWorld.AmbushActive = false;

                            if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.SendData(MessageID.WorldData);
                            }
                            
                            NPC.netUpdate = true;

                            NPC.active = false;
                        }
                    }
                }
            }
        }

        //spawn enemies method for easy spawning and easily setting each cultists parent to this altar
        public void SpawnNPC(int Type, int X, int Y, int Parent)
        {
            int NewNPC = NPC.NewNPC(NPC.GetSource_FromAI(), X, Y, Type, ai0: Parent);

            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.SyncNPC, number: NewNPC);
            }
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void AI()
        {
            //spawn all the cultists
            if (NPC.ai[0] == 0)
            {
                SpawnNPC(ModContent.NPCType<NoseCultistGunnerIdle>(), (int)NPC.Center.X - 250, (int)NPC.Center.Y + 100, NPC.whoAmI);
                SpawnNPC(ModContent.NPCType<NoseCultistGruntIdle>(), (int)NPC.Center.X - 200, (int)NPC.Center.Y + 100, NPC.whoAmI);
                SpawnNPC(ModContent.NPCType<NoseCultistGruntIdle>(), (int)NPC.Center.X - 150, (int)NPC.Center.Y + 100, NPC.whoAmI);
                SpawnNPC(ModContent.NPCType<NoseCultistGruntIdle>(), (int)NPC.Center.X - 100, (int)NPC.Center.Y + 100, NPC.whoAmI);
                SpawnNPC(ModContent.NPCType<NoseCultistGruntIdle>(), (int)NPC.Center.X + 100, (int)NPC.Center.Y + 100, NPC.whoAmI);
                SpawnNPC(ModContent.NPCType<NoseCultistGruntIdle>(), (int)NPC.Center.X + 150, (int)NPC.Center.Y + 100, NPC.whoAmI);
                SpawnNPC(ModContent.NPCType<NoseCultistGruntIdle>(), (int)NPC.Center.X + 200, (int)NPC.Center.Y + 100, NPC.whoAmI);
                SpawnNPC(ModContent.NPCType<NoseCultistGunnerIdle>(), (int)NPC.Center.X + 250, (int)NPC.Center.Y + 100, NPC.whoAmI);

                NPC.ai[0]++;
            }

            if (NPC.ai[1] > 0)
            {
                Lighting.AddLight(NPC.Center, Color.LightGreen.ToVector3() * (NPC.ai[2] / 10));
            }

            HandleCultistAmbush();

            if (NPC.ai[2] < 65)
            {
                //bob up and down 
                if (NPC.localAI[0] == 0)
                {
                    NPC.localAI[1] = Flags.MocoIdolPosition1.Y - 50;
                    NPC.localAI[0]++;
                }

                NPC.localAI[2]++;
                NPC.position.Y = NPC.localAI[1] + (float)Math.Sin(NPC.localAI[2] / 100) * 10;
            }
        }
    }

    public class MocoIdol2 : MocoIdol1  
    {
        public override string Texture => "Spooky/Content/NPCs/NoseCult/MocoIdol";

        public override void AI()
        {
            //spawn all the cultists
            if (NPC.ai[0] == 0)
            {
                SpawnNPC(ModContent.NPCType<NoseCultistWingedIdle>(), (int)NPC.Center.X - 250, (int)NPC.Center.Y + 100, NPC.whoAmI);
                SpawnNPC(ModContent.NPCType<NoseCultistGunnerIdle>(), (int)NPC.Center.X - 200, (int)NPC.Center.Y + 100, NPC.whoAmI);
                SpawnNPC(ModContent.NPCType<NoseCultistGruntIdle>(), (int)NPC.Center.X - 150, (int)NPC.Center.Y + 100, NPC.whoAmI);
                SpawnNPC(ModContent.NPCType<NoseCultistGruntIdle>(), (int)NPC.Center.X - 100, (int)NPC.Center.Y + 100, NPC.whoAmI);
                SpawnNPC(ModContent.NPCType<NoseCultistGruntIdle>(), (int)NPC.Center.X + 100, (int)NPC.Center.Y + 100, NPC.whoAmI);
                SpawnNPC(ModContent.NPCType<NoseCultistGruntIdle>(), (int)NPC.Center.X + 150, (int)NPC.Center.Y + 100, NPC.whoAmI);
                SpawnNPC(ModContent.NPCType<NoseCultistGunnerIdle>(), (int)NPC.Center.X + 200, (int)NPC.Center.Y + 100, NPC.whoAmI);
                SpawnNPC(ModContent.NPCType<NoseCultistWingedIdle>(), (int)NPC.Center.X + 250, (int)NPC.Center.Y + 100, NPC.whoAmI);

                NPC.ai[0]++;
            }

            if (NPC.ai[1] > 0)
            {
                Lighting.AddLight(NPC.Center, Color.LightGreen.ToVector3() * (NPC.ai[2] / 10));
            }

            HandleCultistAmbush();

            if (NPC.ai[2] < 65)
            {
                //bob up and down 
                if (NPC.localAI[0] == 0)
                {
                    NPC.localAI[1] = Flags.MocoIdolPosition1.Y - 50;
                    NPC.localAI[0]++;
                }

                NPC.localAI[2]++;
                NPC.position.Y = NPC.localAI[1] + (float)Math.Sin(NPC.localAI[2] / 100) * 10;
            }
        }
    }

    public class MocoIdol3 : MocoIdol1  
    {
        public override string Texture => "Spooky/Content/NPCs/NoseCult/MocoIdol";

        public override void AI()
        {
            //spawn all the cultists
            if (NPC.ai[0] == 0)
            {
                SpawnNPC(ModContent.NPCType<NoseCultistWingedIdle>(), (int)NPC.Center.X - 250, (int)NPC.Center.Y + 100, NPC.whoAmI);
                SpawnNPC(ModContent.NPCType<NoseCultistGunnerIdle>(), (int)NPC.Center.X - 200, (int)NPC.Center.Y + 100, NPC.whoAmI);
                SpawnNPC(ModContent.NPCType<NoseCultistMageIdle>(), (int)NPC.Center.X - 150, (int)NPC.Center.Y + 100, NPC.whoAmI);
                SpawnNPC(ModContent.NPCType<NoseCultistGruntIdle>(), (int)NPC.Center.X - 100, (int)NPC.Center.Y + 100, NPC.whoAmI);
                SpawnNPC(ModContent.NPCType<NoseCultistGruntIdle>(), (int)NPC.Center.X + 100, (int)NPC.Center.Y + 100, NPC.whoAmI);
                SpawnNPC(ModContent.NPCType<NoseCultistGruntIdle>(), (int)NPC.Center.X + 150, (int)NPC.Center.Y + 100, NPC.whoAmI);
                SpawnNPC(ModContent.NPCType<NoseCultistWingedIdle>(), (int)NPC.Center.X + 200, (int)NPC.Center.Y + 100, NPC.whoAmI);
                SpawnNPC(ModContent.NPCType<NoseCultistWingedIdle>(), (int)NPC.Center.X + 250, (int)NPC.Center.Y + 100, NPC.whoAmI);

                NPC.ai[0]++;
            }

            if (NPC.ai[1] > 0)
            {
                Lighting.AddLight(NPC.Center, Color.LightGreen.ToVector3() * (NPC.ai[2] / 10));
            }

            HandleCultistAmbush();

            if (NPC.ai[2] < 65)
            {
                //bob up and down 
                if (NPC.localAI[0] == 0)
                {
                    NPC.localAI[1] = Flags.MocoIdolPosition1.Y - 50;
                    NPC.localAI[0]++;
                }

                NPC.localAI[2]++;
                NPC.position.Y = NPC.localAI[1] + (float)Math.Sin(NPC.localAI[2] / 100) * 10;
            }
        }
    }

    public class MocoIdol4 : MocoIdol1  
    {
        public override string Texture => "Spooky/Content/NPCs/NoseCult/MocoIdol";
        
        public override void AI()
        {
            //spawn all the cultists
            if (NPC.ai[0] == 0)
            {
                SpawnNPC(ModContent.NPCType<NoseCultistWingedIdle>(), (int)NPC.Center.X - 200, (int)NPC.Center.Y + 100, NPC.whoAmI);
                SpawnNPC(ModContent.NPCType<NoseCultistMageIdle>(), (int)NPC.Center.X - 150, (int)NPC.Center.Y + 100, NPC.whoAmI);
                SpawnNPC(ModContent.NPCType<NoseCultistGruntIdle>(), (int)NPC.Center.X - 100, (int)NPC.Center.Y + 100, NPC.whoAmI);
                SpawnNPC(ModContent.NPCType<NoseCultistBruteIdle>(), (int)NPC.Center.X + 100, (int)NPC.Center.Y + 100, NPC.whoAmI);
                SpawnNPC(ModContent.NPCType<NoseCultistGruntIdle>(), (int)NPC.Center.X + 190, (int)NPC.Center.Y + 100, NPC.whoAmI);
                SpawnNPC(ModContent.NPCType<NoseCultistWingedIdle>(), (int)NPC.Center.X + 250, (int)NPC.Center.Y + 100, NPC.whoAmI);

                NPC.ai[0]++;
            }

            if (NPC.ai[1] > 0)
            {
                Lighting.AddLight(NPC.Center, Color.LightGreen.ToVector3() * (NPC.ai[2] / 10));
            }

            HandleCultistAmbush();

            if (NPC.ai[2] < 65)
            {
                //bob up and down 
                if (NPC.localAI[0] == 0)
                {
                    NPC.localAI[1] = Flags.MocoIdolPosition1.Y - 50;
                    NPC.localAI[0]++;
                }

                NPC.localAI[2]++;
                NPC.position.Y = NPC.localAI[1] + (float)Math.Sin(NPC.localAI[2] / 100) * 10;
            }
        }
    }

    public class MocoIdol5 : MocoIdol1  
    {
        public override string Texture => "Spooky/Content/NPCs/NoseCult/MocoIdol";

        public override void AI()
        {
            //spawn all the cultists
            if (NPC.ai[0] == 0)
            {
                SpawnNPC(ModContent.NPCType<NoseCultistMageIdle>(), (int)NPC.Center.X - 280, (int)NPC.Center.Y + 100, NPC.whoAmI);
                SpawnNPC(ModContent.NPCType<NoseCultistWingedIdle>(), (int)NPC.Center.X - 220, (int)NPC.Center.Y + 100, NPC.whoAmI);
                SpawnNPC(ModContent.NPCType<NoseCultistWingedIdle>(), (int)NPC.Center.X - 150, (int)NPC.Center.Y + 100, NPC.whoAmI);
                SpawnNPC(ModContent.NPCType<NoseCultistBruteIdle>(), (int)NPC.Center.X - 65, (int)NPC.Center.Y + 100, NPC.whoAmI);
                SpawnNPC(ModContent.NPCType<NoseCultistGunnerIdle>(), (int)NPC.Center.X + 90, (int)NPC.Center.Y + 100, NPC.whoAmI);
                SpawnNPC(ModContent.NPCType<NoseCultistWingedIdle>(), (int)NPC.Center.X + 150, (int)NPC.Center.Y + 100, NPC.whoAmI);
                SpawnNPC(ModContent.NPCType<NoseCultistWingedIdle>(), (int)NPC.Center.X + 220, (int)NPC.Center.Y + 100, NPC.whoAmI);
                SpawnNPC(ModContent.NPCType<NoseCultistMageIdle>(), (int)NPC.Center.X + 280, (int)NPC.Center.Y + 100, NPC.whoAmI);

                NPC.ai[0]++;
            }

            if (NPC.ai[1] > 0)
            {
                Lighting.AddLight(NPC.Center, Color.LightGreen.ToVector3() * (NPC.ai[2] / 10));
            }

            HandleCultistAmbush();

            if (NPC.ai[2] < 65)
            {
                //bob up and down 
                if (NPC.localAI[0] == 0)
                {
                    NPC.localAI[1] = Flags.MocoIdolPosition1.Y - 50;
                    NPC.localAI[0]++;
                }

                NPC.localAI[2]++;
                NPC.position.Y = NPC.localAI[1] + (float)Math.Sin(NPC.localAI[2] / 100) * 10;
            }
        }
    }

    public class MocoIdol6 : MocoIdol1  
    {
        public override string Texture => "Spooky/Content/NPCs/NoseCult/MocoIdol";

        public override void AI()
        {
            NPC.alpha = 255;

            //spawn the cultist leader
            if (NPC.ai[0] == 0)
            {
                SpawnNPC(ModContent.NPCType<NoseCultistLeaderIdle>(), (int)NPC.Center.X + ((NPC.Center.X / 16) > (Main.maxTilesX / 2) ? -2 : 2), (int)NPC.Center.Y + 120, NPC.whoAmI);
                NPC.ai[0]++;
            }

            if (NPC.ai[1] > 0)
            {
                Lighting.AddLight(new Vector2(NPC.Center.X, NPC.Center.Y - 50), Color.LightGreen.ToVector3() * (NPC.ai[2] / 3));
            }

            HandleCultistAmbush();
        }
    }
}