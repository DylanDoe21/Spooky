using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.Localization;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.NPCs.Hallucinations
{
    public class TheBaby : ModNPC
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            NPCID.Sets.ShouldBeCountedAsBoss[NPC.type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Rotation = MathHelper.PiOver2,
                Position = new Vector2(34f, 0f),
                PortraitPositionXOverride = 10f,
                PortraitPositionYOverride = 0f
            };
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
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.HitSound = SoundID.NPCHit9;
            NPC.DeathSound = SoundID.NPCDeath22;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
            {
                new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.TheBaby"),
                new MoonLordPortraitBackgroundProviderBestiaryInfoElement()
            });
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];

            player.AddBuff(ModContent.BuffType<HallucinationDebuff2>(), 2);
            player.AddBuff(BuffID.Dazed, 2);

            NPC.spriteDirection = NPC.direction;

            Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
            float RotateX = player.Center.X - vector.X;
            float RotateY = player.Center.Y - vector.Y;
            NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

            NPC.localAI[1]++;
            //make npcs displayed name a random jumble of characters constantly
            if (NPC.localAI[1] % 5 == 0)
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()-=_+";
                string nameString = new(Enumerable.Repeat(chars, 12).Select(s => s[Main.rand.Next(s.Length)]).ToArray());
                NPC.GivenName = nameString;
            }

            if (NPC.Distance(player.Center) >= 750f)
            {
                Teleport(player, 0);
            }

            NPC.localAI[0]++;

            if (NPC.localAI[0] >= 6200)
            {
                Vector2 ChargeDirection = player.Center - NPC.Center;
                ChargeDirection.Normalize();

                ChargeDirection *= 35;
                NPC.velocity = ChargeDirection;

                if (NPC.Hitbox.Intersects(player.Hitbox))
                {
                    if (!Flags.encounteredBaby)
                    {
                        Flags.encounteredBaby = true;

                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(MessageID.WorldData);
                        }
                    }

                    player.ApplyDamageToNPC(NPC, NPC.lifeMax * 2, 0, 0, false);
                    NPC.immortal = false;
                    NPC.dontTakeDamage = false;
                    NPC.netUpdate = true;
                }
            }
        }

        //teleport code from vanilla caster ai, it sucks and i will probably redo it at some point
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

            if (NPC.ai[2] != 0 && NPC.ai[3] != 0 && foundNewLoc)
            {
                NPC.position.X = (float)((double)NPC.ai[2] * 16.0 - (double)(NPC.width / 2) + 8.0);
                NPC.position.Y = NPC.ai[3] * 16f - (float)NPC.height;
                NPC.netUpdate = true;
            }
            else if (attemptNum < 10)
            {
                Teleport(player, attemptNum + 1);
            }
        }
    }
}