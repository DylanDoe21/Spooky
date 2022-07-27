using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Linq;

using Spooky.Content.Buffs.Debuff;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.NPCs
{
    public class TheEntity : ModNPC
    {
        public static readonly SoundStyle CrunchSound = new("Spooky/Content/Sounds/Scream", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("");
            Main.npcFrameCount[NPC.type] = 2;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
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
            NPC.lifeMax = 250;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 130;
            NPC.height = 140;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.boss = true;
            NPC.HitSound = SoundID.NPCHit9;
            NPC.DeathSound = SoundID.NPCDeath22;
            Music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/TheEntity");
        }

        public override void FindFrame(int frameHeight)
        {
            Player player = Main.player[NPC.target];

            //use wretched second frame during the jumpscare, only if in its distance ai state as well
            if (player.Distance(NPC.Center) <= NPC.localAI[0] && NPC.ai[0] >= 1)
            {
                NPC.frame.Y = frameHeight * 1;
            }
            else
            {
                NPC.frame.Y = frameHeight * 0;
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/TheEntityGlow").Value;
            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(tex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), NPC.frame,
            new Color(255, 255, 255), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];

            player.AddBuff(ModContent.BuffType<EntityDebuff>(), 2);

            NPC.localAI[2]++;
            //make npcs displayed name a random jumble of characters constantly
            if (NPC.localAI[2] % 5 == 0)
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()-=_+";
                string nameString = new(Enumerable.Repeat(chars, 12).Select(s => s[Main.rand.Next(s.Length)]).ToArray());
                NPC.GivenName = nameString;
            }

            switch ((int)NPC.ai[0])
            {
                //teleport around the player, repeat 5 times
                case 0:
                {
                    NPC.localAI[0]++;

                    NPC.damage = 0;

                    //loop 5 times
                    if (NPC.localAI[1] < 5)
                    {
                        //teleport after a certain time or if the player goes too far
                        if (NPC.localAI[0] >= 450 || NPC.Distance(player.Center) >= 3000f)
                        {
                            Teleport(player, 0);
                            NPC.localAI[0] = 0;
                            NPC.localAI[1]++;
                        }
                    }
                    else
                    {
                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0;
                        NPC.ai[0]++;
                    }

                    break;
                }

                //create distance ring that increases, do funny jumpscare instakill if player enters the ring
                case 1:
                {
                    //increase and use localAI[0] for the distance
                    NPC.localAI[0] += 0.5f;

                    //lol
                    NPC.damage = 9999999;

                    if (NPC.localAI[0] > 300f)
                    {
                        NPC.localAI[0] += 35f;
                    }

                    for (int i = 0; i < 20; i++)
                    {
                        Vector2 offset = new();
                        double angle = Main.rand.NextDouble() * 2d * Math.PI;
                        offset.X += (float)(Math.Sin(angle) * NPC.localAI[0]);
                        offset.Y += (float)(Math.Cos(angle) * NPC.localAI[0]);
                        Dust dust = Main.dust[Dust.NewDust(NPC.Center + offset - new Vector2(4, 4), 0, 0, DustID.GemDiamond, 0, 0, 100, Color.White, 1f)];
                        dust.velocity *= 0;
                        dust.noGravity = true;
                        dust.scale = 0.5f;
                    }

                    if (player.Distance(NPC.Center) <= NPC.localAI[0] || NPC.Distance(player.Center) >= 3500f)
                    {
                        NPC.noGravity = true;
                        NPC.noTileCollide = true;

                        Vector2 ChargeDirection = Main.player[NPC.target].Center - NPC.Center;
                        ChargeDirection.Normalize();

                        ChargeDirection.X *= 100;
                        ChargeDirection.Y *= 100;
                        NPC.velocity.X = ChargeDirection.X;
                        NPC.velocity.Y = ChargeDirection.Y;
                    }
                    else
                    {
                        NPC.noGravity = false;
                        NPC.noTileCollide = false;
                    }

                    break;
                }
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (NPC.ai[0] >= 1)
            {
                NPC.active = false;
            }
        }

        //bruh moment vanilla caster teleporting ai (skull emoji)
        private void Teleport(Player player, int attemptNum)
        {
            int playerTileX = (int)player.position.X / 16;
            int playerTileY = (int)player.position.Y / 16;
            int npcTileX = (int)NPC.position.X / 16;
            int npcTileY = (int)NPC.position.Y / 16;
            int maxTileDist = 20;
            bool foundNewLoc = false;
            int targetX = Main.rand.Next(playerTileX - maxTileDist, playerTileX + maxTileDist);

            for (int targetY = Main.rand.Next(playerTileY - maxTileDist, playerTileY + maxTileDist); targetY < playerTileY + maxTileDist; ++targetY)
            {
                if ((targetY < playerTileY - 4 || targetY > playerTileY + 4 ||
                (targetX < playerTileX - 4 || targetX > playerTileX + 4)) &&
                (targetY < npcTileY - 1 || targetY > npcTileY + 1 || 
                (targetX < npcTileX - 1 || targetX > npcTileX + 1)) && 
                Main.tile[targetX, targetY].HasUnactuatedTile)
                {
                    bool flag2 = true;
                    if (Main.tile[targetX, targetY - 1].LiquidType == LiquidID.Lava) 
                    {
                        flag2 = false;
                    }

                    if (flag2 && Main.tileSolid[(int)Main.tile[targetX, targetY].TileType] && !Collision.SolidTiles(targetX - 1, targetX + 1, targetY - 4, targetY - 1))
                    {
                        NPC.ai[2] = (float)targetX;
                        NPC.ai[3] = (float)targetY;
                        foundNewLoc = true;
                        break;
                    }
                }
            }

            SoundEngine.PlaySound(SoundID.Item8, NPC.position);
            
            if (NPC.ai[2] != 0 && NPC.ai[3] != 0 && foundNewLoc)
            {
                NPC.position.X = (float)((double)NPC.ai[2] * 16.0 - (double)(NPC.width / 2) + 8.0);
                NPC.position.Y = NPC.ai[3] * 16f - (float)NPC.height;
                NPC.netUpdate = true;

                for (int i = 0; i < 20; i++)
                {
                    Dust dust = Main.dust[Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GemDiamond)];
                    dust.noGravity = true;
                    dust.scale = 1f;
                    dust.velocity *= 0.1f;
                }
            }
            else if (attemptNum < 10) 
            {
                Teleport(player, attemptNum + 1);
            }
        }
    }
}