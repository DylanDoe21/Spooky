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
    public class TheHorse : ModNPC
    {
        public int maxFlies = 2;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            NPCID.Sets.ShouldBeCountedAsBoss[NPC.type] = true;

            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Position = new Vector2(28f, 35f),
                PortraitPositionXOverride = -5f,
                PortraitPositionYOverride = 6f
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
            NPC.width = 106;
            NPC.height = 90;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.HitSound = SoundID.NPCHit9;
            NPC.DeathSound = SoundID.NPCDeath22;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
            {
                new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.TheHorse"),
                new MoonLordPortraitBackgroundProviderBestiaryInfoElement()
            });
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];

            player.AddBuff(ModContent.BuffType<HallucinationDebuff>(), 2);
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
                //teleport to the player immediately
                case 0:
                {
                    if (NPC.Distance(player.Center) >= 1000f)
                    {
                        Teleport(player, 0);
                        NPC.localAI[0]++;
                    }

                    if (NPC.localAI[0] > 0)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[0]++;
                        NPC.netUpdate = true;
                    }

                    break;
                }

                //spawn flies and repeat dialogue
                case 1:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] >= 120)
                    {
                        NPC.localAI[1]++;

                        if (NPC.localAI[1] % 120 == 20 && maxFlies <= 1000)
                        {
                            CombatText.NewText(NPC.getRect(), Color.DarkGreen, Language.GetTextValue("Mods.Spooky.Dialogue.TheHorse.Hunger"), true);

                            for (int k = 0; k < Main.projectile.Length; k++)
                            {
                                if (Main.projectile[k].active && Main.projectile[k].type == ModContent.ProjectileType<TheHorseFly>()) 
                                {
                                    Main.projectile[k].Kill();
                                }
                            }

                            for (int numFlies = 0; numFlies <= maxFlies; numFlies++)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center, Vector2.Zero, ModContent.ProjectileType<TheHorseFly>(), 1, 0, NPC.target);
                            }

                            maxFlies *= 2;

                            NPC.netUpdate = true;
                        }
                    }

                    if (NPC.Distance(player.Center) >= 750f)
                    {
                        Teleport(player, 0);
                    }

                    if (maxFlies >= 1000)
                    {
                        if (!Flags.encounteredHorse)
                        {
                            Flags.encounteredHorse = true;

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

            SoundEngine.PlaySound(SoundID.Item8, NPC.position);

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

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            //npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ShadowClump>(), 1));
        }
    }
}