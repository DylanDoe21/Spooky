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
using Spooky.Content.Events;
using Spooky.Content.Items.SpookyHell.Boss;
using Spooky.Content.NPCs.EggEvent.Projectiles;

namespace Spooky.Content.NPCs.EggEvent
{
    public class Capillary : ModNPC
    {
        int aura;

        public static readonly SoundStyle HitSound = new("Spooky/Content/Sounds/SpookyHell/EnemyHit", SoundType.Sound);
        public static readonly SoundStyle DeathSound = new("Spooky/Content/Sounds/SpookyHell/EnemyDeath", SoundType.Sound);
        public static readonly SoundStyle ScreechSound = new("Spooky/Content/Sounds/SpookyHell/CapillaryScreech", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Capillary");
            Main.npcFrameCount[NPC.type] = 5;
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
            NPC.lifeMax = 700;
            NPC.damage = 50;
            NPC.defense = 35;
            NPC.width = 50;
            NPC.height = 94;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.noTileCollide = false;
            NPC.noGravity = false;
            NPC.HitSound = HitSound;
            NPC.DeathSound = DeathSound;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[2] { ModContent.GetInstance<Biomes.EggEventBiome>().Type, ModContent.GetInstance<Biomes.SpookyHellBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
            {
                new FlavorTextBestiaryInfoElement("Normally hiding in the ground, the eye valley's living capillaries emerge when the egg is threatened, using special auras to support their allies and weaken prey."),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement)
            });
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/EggEvent/CapillaryGlow").Value;

            Main.EntitySpriteDraw(tex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4),
            NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

            if (player.InModBiome(ModContent.GetInstance<Biomes.EggEventBiome>()) && EggEventWorld.EggEventProgress >= 60)
            {
                return 15f;
            }

            return 0f;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1;

            if (NPC.frameCounter > 10)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0.0;
            }
            if (NPC.frame.Y >= frameHeight * 5)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            NPC.localAI[0]++;

            if (NPC.localAI[0] >= 600)
            {
                SoundEngine.PlaySound(ScreechSound, NPC.Center);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    aura = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, 0, 0,
                    ModContent.ProjectileType<CapillaryAura>(), 0, 1, NPC.target, 0, 0);
                }

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

                for (int numGores = 1; numGores <= 6; numGores++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/CapillaryGore" + numGores).Type);
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.ByCondition(new DropConditions.PostOrroboroCondition(), ModContent.ItemType<ArteryPiece>(), 3, 1, 3));
        }
    }
}