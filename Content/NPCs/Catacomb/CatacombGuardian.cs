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

using Spooky.Content.Items.Pets;
using Spooky.Content.NPCs.Boss.BigBone;

namespace Spooky.Content.NPCs.Catacomb
{
    public class CatacombGuardian : ModNPC
    {
        Vector2 SavePlayerPosition;

        public int attackPattern = 0;
        public int SaveDirection;
        public float SaveRotation;

        private static Asset<Texture2D> EyeGlowTexture;
        private static Asset<Texture2D> EyeTrailTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 2;
            NPCID.Sets.TrailCacheLength[NPC.type] = 8;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Rotation = MathHelper.PiOver2,
                Position = new Vector2(34f, 0f),
                PortraitPositionXOverride = 8f,
                PortraitPositionYOverride = -8f
            };
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
            NPC.lifeMax = 9999;
            NPC.damage = 1000;
            NPC.defense = 9999;
            NPC.width = 152;
			NPC.height = 144;
            NPC.npcSlots = 0f;
			NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.DD2_SkeletonHurt;
			NPC.DeathSound = SoundID.NPCDeath2;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[2] { ModContent.GetInstance<Biomes.CatacombBiome>().Type, ModContent.GetInstance<Biomes.CatacombBiome2>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
            int associatedNPCType = ModContent.NPCType<BigBone>();
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);

            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.CatacombGuardian"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome>().ModBiomeBestiaryInfoElement),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome2>().ModBiomeBestiaryInfoElement)
			});
		}

        //draw eye after images
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            EyeGlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Catacomb/CatacombGuardianGlow");
            EyeTrailTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Catacomb/CatacombGuardianTrail");

            Vector2 drawOrigin = new(EyeTrailTexture.Width() * 0.5f, NPC.height * 0.5f);

            var effects = attackPattern == 2 && NPC.localAI[0] >= 360 ? (SaveDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally) : (NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);

            for (int oldPos = 0; oldPos < NPC.oldPos.Length; oldPos++)
            {
                Vector2 drawPos = NPC.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, NPC.gfxOffY + 4);
                Color color = NPC.GetAlpha(Color.White) * (float)(((float)(NPC.oldPos.Length - oldPos) / (float)NPC.oldPos.Length) / 2);
                spriteBatch.Draw(EyeTrailTexture.Value, drawPos, NPC.frame, color, NPC.rotation, drawOrigin, NPC.scale, effects, 0f);
            }

            Main.spriteBatch.Draw(EyeGlowTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0f, NPC.gfxOffY + 4), NPC.frame, Color.White, NPC.rotation, drawOrigin, NPC.scale, effects, 0);
		}

        public override void FindFrame(int frameHeight)
		{
			//determine frame based on direction
			if (NPC.direction == -1)
			{
				NPC.frame.Y = frameHeight * 0;
			}
            else
            {
                NPC.frame.Y = frameHeight * 1;
            }
        }

        public void RotateToPlayer(Player player)
        {
            Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
            float RotateX = player.Center.X - vector.X;
            float RotateY = player.Center.Y - vector.Y;

            float RotateDirection = (float)Math.Atan2(RotateY, RotateX) + 4.71f;
			float RotateSpeed = 0.04f;

			if (RotateDirection < 0f)
			{
				RotateDirection += (float)Math.PI * 2f;
			}
			if (RotateDirection > (float)Math.PI * 2f)
			{
				RotateDirection -= (float)Math.PI * 2f;
			}

			if (NPC.rotation < RotateDirection)
			{
				if ((double)(RotateDirection - NPC.rotation) > Math.PI)
				{
					NPC.rotation -= RotateSpeed;
				}
				else
				{
					NPC.rotation += RotateSpeed;
				}
			}
			if (NPC.rotation > RotateDirection)
			{
				if ((double)(NPC.rotation - RotateDirection) > Math.PI)
				{
					NPC.rotation += RotateSpeed;
				}
				else
				{
					NPC.rotation -= RotateSpeed;
				}
			}
			if (NPC.rotation > RotateDirection - RotateSpeed && NPC.rotation < RotateDirection + RotateSpeed)
			{
				NPC.rotation = RotateDirection;
			}
			if (NPC.rotation < 0f)
			{
				NPC.rotation += (float)Math.PI * 2f;
			}
			if (NPC.rotation > (float)Math.PI * 2f)
			{
				NPC.rotation -= (float)Math.PI * 2f;
			}
			if (NPC.rotation > RotateDirection - RotateSpeed && NPC.rotation < RotateDirection + RotateSpeed)
			{
				NPC.rotation = RotateDirection;
			}
        }
        
        public override void AI()
		{
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

			NPC.spriteDirection = NPC.direction;

            RotateToPlayer(player);

            Vector2 desiredVelocity = NPC.DirectionTo(player.Center) * 9;
            NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SkullKey>()));
        }

        /*
        public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/BigBoneGore2").Type);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/BigBoneGore3").Type);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/BigBoneGore4").Type);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/BigBoneGore5").Type);
                }
            }
        }
        */
    }
}