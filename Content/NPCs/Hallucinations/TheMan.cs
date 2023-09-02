using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.NPCs.Hallucinations
{
    public class TheMan : ModNPC
    {
        public static readonly SoundStyle BreathingSound = new("Spooky/Content/Sounds/EntityBreathing", SoundType.Sound);
        public static readonly SoundStyle ScreamingSound = new("Spooky/Content/Sounds/EntityScream", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 2;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            NPCID.Sets.ShouldBeCountedAsBoss[NPC.type] = true;
            NPCID.Sets.TrailCacheLength[NPC.type] = 7;
            NPCID.Sets.TrailingMode[NPC.type] = 0;

            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Position = new Vector2(15f, 75f),
                PortraitPositionXOverride = 8f,
                PortraitPositionYOverride = 45f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
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
            NPC.width = 90;
            NPC.height = 142;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.HitSound = SoundID.NPCHit9;
            NPC.DeathSound = SoundID.NPCDeath22;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
            {
                new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.TheMan"),
                new MoonLordPortraitBackgroundProviderBestiaryInfoElement()
            });
        }

        public override void FindFrame(int frameHeight)
        {
            Player player = Main.player[NPC.target];

            if (NPC.ai[0] == 2)
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
            Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Hallucinations/TheManGlow").Value;
            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(tex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), NPC.frame,
            Color.White, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            if (NPC.ai[0] == 2)
            {
                Vector2 drawOrigin = new(tex.Width * 0.5f, (NPC.height * 0.5f));

                for (int oldPos = 0; oldPos < NPC.oldPos.Length; oldPos++)
                {
                    Vector2 drawPos = NPC.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, NPC.gfxOffY + 4);
                    Color color = NPC.GetAlpha(Color.White) * (float)(((float)(NPC.oldPos.Length - oldPos) / (float)NPC.oldPos.Length) / 2);
                    spriteBatch.Draw(tex, drawPos, new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, drawOrigin, NPC.scale, effects, 0f);
                }
            }
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];

            player.AddBuff(ModContent.BuffType<HallucinationDebuff1>(), 2);
            player.AddBuff(BuffID.Dazed, 2);

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
                //teleport around the player, repeat 4 times
                case 0:
                {
                    NPC.localAI[0]++;

                    //loop 5 times
                    if (NPC.localAI[1] < 4)
                    {
                        //teleport after a certain time or if the player goes too far
                        if (NPC.localAI[0] >= 450)
                        {
                            SoundEngine.PlaySound(BreathingSound, player.Center);

                            Teleport(player, 0);
                            NPC.localAI[0] = 0;
                            NPC.localAI[1]++;
                        }

                        if (NPC.Distance(player.Center) >= 2200f)
                        {
                            Teleport(player, 0);
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

                //countdown (you better start running)
                case 1:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] == 60)
                    {
                        CombatText.NewText(NPC.getRect(), Color.White, Language.GetTextValue("Mods.Spooky.Dialogue.TheMan.Five"), true);
                    }
                    if (NPC.localAI[0] == 120)
                    {
                        CombatText.NewText(NPC.getRect(), Color.White, Language.GetTextValue("Mods.Spooky.Dialogue.TheMan.Four"), true);
                    }
                    if (NPC.localAI[0] == 180)
                    {
                        CombatText.NewText(NPC.getRect(), Color.White, Language.GetTextValue("Mods.Spooky.Dialogue.TheMan.Three"), true);
                    }
                    if (NPC.localAI[0] == 240)
                    {
                        CombatText.NewText(NPC.getRect(), Color.White, Language.GetTextValue("Mods.Spooky.Dialogue.TheMan.Two"), true);
                    }
                    if (NPC.localAI[0] == 300)
                    {
                        CombatText.NewText(NPC.getRect(), Color.White, Language.GetTextValue("Mods.Spooky.Dialogue.TheMan.One"), true);
                    }
                    if (NPC.localAI[0] == 360)
                    {
                        SoundEngine.PlaySound(ScreamingSound, player.Center);

                        NPC.localAI[0] = 0;
                        NPC.ai[0]++;
                    }

                    break;
                }

                //chase down the player constantly
                case 2:
                {
                    NPC.noGravity = true;
                    NPC.noTileCollide = true;

                    Vector2 ChargeDirection = player.Center - NPC.Center;
                    ChargeDirection.Normalize();

                    ChargeDirection *= 65;
                    NPC.velocity = ChargeDirection;

                    if (NPC.Hitbox.Intersects(player.Hitbox))
                    {
                        if (!Flags.encounteredMan)
                        {
                            Flags.encounteredMan = true;

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

                    break;
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