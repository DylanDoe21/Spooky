using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.SpookyHell.Boss;
using Spooky.Content.NPCs.EggEvent.Projectiles;

namespace Spooky.Content.NPCs.EggEvent
{
    public class Crux : ModNPC
    {
        public int MoveSpeedX = 0;
		public int MoveSpeedY = 0;

        int aura;

        public static readonly SoundStyle HitSound = new("Spooky/Content/Sounds/SpookyHell/EnemyHit", SoundType.Sound);
        public static readonly SoundStyle DeathSound = new("Spooky/Content/Sounds/SpookyHell/EnemyDeath", SoundType.Sound);
        public static readonly SoundStyle ScreamSound = new("Spooky/Content/Sounds/SpookyHell/CruxScream", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crux");
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 350;
            NPC.damage = 60;
            NPC.defense = 35;
            NPC.width = 56;
            NPC.height = 100;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = HitSound;
			NPC.DeathSound = DeathSound;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[2] { ModContent.GetInstance<Biomes.EggEventBiome>().Type, ModContent.GetInstance<Biomes.SpookyHellBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Crude and misshapen creatures who's heart is outside of their body, allowing them to see their surroundings. They can use auras that can inflict negative effects on prey."),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/EggEvent/CruxGlow").Value;

            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(tex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

            if (player.InModBiome(ModContent.GetInstance<Biomes.EggEventBiome>()))
            {
                return 12f;
            }

            return 0f;
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
            
            NPC.spriteDirection = NPC.direction;

            NPC.localAI[0]++;

            if (NPC.localAI[0] < 300)
            {
                //flies to players X position
                if (NPC.Center.X >= player.Center.X && MoveSpeedX >= -25) 
                {
                    MoveSpeedX--;
                }
                else if (NPC.Center.X <= player.Center.X && MoveSpeedX <= 25)
                {
                    MoveSpeedX++;
                }

                NPC.velocity.X = MoveSpeedX * 0.1f;
                
                //flies to players Y position
                if (NPC.Center.Y >= player.Center.Y - 60f && MoveSpeedY >= -25)
                {
                    MoveSpeedY--;
                }
                else if (NPC.Center.Y <= player.Center.Y - 60f && MoveSpeedY <= 25)
                {
                    MoveSpeedY++;
                }

                NPC.velocity.Y = MoveSpeedY * 0.1f;
            }

            if (NPC.localAI[0] == 300)
            {
                NPC.velocity *= 0;

                SoundEngine.PlaySound(ScreamSound, NPC.Center);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    aura = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, 0, 0, 
                    ModContent.ProjectileType<CruxAura>(), 0, 1, NPC.target, 0, 0);
                }
            }

            if (NPC.localAI[0] == 460)
            {
                NPC.localAI[0] = 0;
            }
        }

        public override void HitEffect(int hitDirection, double damage) 
        {
            //dont run on multiplayer
			if (Main.netMode == NetmodeID.Server) 
            {
				return;
			}

			if (NPC.life <= 0) 
            {
                Main.projectile[aura].Kill();

                for (int numGores = 1; numGores <= 3; numGores++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/CruxGore" + numGores).Type);
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.ByCondition(new DropConditions.PostOrroboroCondition(), ModContent.ItemType<ArteryPiece>(), 3, 1, 3));
        }
    }
}