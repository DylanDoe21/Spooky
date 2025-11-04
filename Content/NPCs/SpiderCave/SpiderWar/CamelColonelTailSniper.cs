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
using Spooky.Content.NPCs.SpiderCave.Projectiles;

namespace Spooky.Content.NPCs.SpiderCave.SpiderWar
{
    public class CamelColonelTailSniper : ModNPC
    {
        float SaveRotation;

        private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 7000;
            NPC.damage = 70;
            NPC.defense = 20;
            NPC.width = 30;
            NPC.height = 30;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.noGravity = true;
			NPC.hide = true;
            NPC.HitSound = SoundID.NPCHit29 with { Pitch = 0.4f };
            NPC.aiStyle = -1;
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			NPC Parent = Main.npc[(int)NPC.ai[3]];

			NPCTexture ??= ModContent.Request<Texture2D>(Texture);

            if (Parent.ai[0] == 1 && Parent.localAI[0] < 170)
            {
                Main.EntitySpriteDraw((Texture2D)TextureAssets.MagicPixel, NPC.Center + new Vector2(-4, 30).RotatedBy(NPC.rotation) - Main.screenPosition, null,
			    Color.Lime * (0.25f - ((float)NPC.alpha / 255f)), NPC.rotation, Vector2.Zero, new Vector2(6, 4), SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);

			return false;
        }

		public override void DrawBehind(int index)
		{
			Main.instance.DrawCacheNPCProjectiles.Add(index);
		}

		public override bool PreAI()
        {
            NPC Parent = Main.npc[(int)NPC.ai[3]];
            Player player = Main.player[Parent.target];

            NPC.alpha = Parent.alpha;

            //kill segment if the head doesnt exist
			if (!Parent.active || Parent.type != ModContent.NPCType<CamelColonel>())
            {
                NPC.active = false;
            }

            Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
            float RotateX = player.Center.X - vector.X;
            float RotateY = player.Center.Y - vector.Y;
            NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

			NPC SegmentParent = Main.npc[(int)NPC.ai[1]];

			Vector2 SegmentCenter = SegmentParent.Center + SegmentParent.velocity - NPC.Center;

			//how far each segment should be from each other
			if (SegmentCenter != Vector2.Zero)
			{
				NPC.Center = SegmentParent.Center - SegmentCenter.SafeNormalize(Vector2.Zero) * 2;
			}

            switch ((int)Parent.ai[0])
			{
                //green sniper bolts
                case 1:
				{
                    if (Parent.localAI[0] == 60 || Parent.localAI[0] == 90 || Parent.localAI[0] == 120 || Parent.localAI[0] == 150)
                    {
                        SoundEngine.PlaySound(SoundID.Item167, NPC.Center);
                        SoundEngine.PlaySound(SoundID.Item63 with { Pitch = -0.5f }, NPC.Center);

                        SaveRotation = NPC.rotation;

                        Vector2 ShootSpeed = player.Center - NPC.Center;
						ShootSpeed.Normalize();
						ShootSpeed *= 7f;

                        Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 30f;

                        NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center + muzzleOffset, ShootSpeed, ModContent.ProjectileType<CamelSniperBolt>(), NPC.damage, 1f);
                    }

                    if ((Parent.localAI[0] >= 60 && Parent.localAI[0] <= 80) || (Parent.localAI[0] >= 90 && Parent.localAI[0] <= 110) ||
                    (Parent.localAI[0] >= 120 && Parent.localAI[0] <= 140) || (Parent.localAI[0] >= 150 && Parent.localAI[0] <= 170))
                    {
                        NPC.rotation = SaveRotation;
                    }

                    break;
                }

                //purple venom bolts
                case 2:
				{
                    if (Parent.localAI[0] >= 30 && Parent.localAI[0] <= 150 && Parent.localAI[0] % 10 == 0)
                    {
                        SoundEngine.PlaySound(SoundID.Item98, NPC.Center);

                        SaveRotation = NPC.rotation;

                        Vector2 ShootSpeed = player.Center - NPC.Center;
						ShootSpeed.Normalize();
						ShootSpeed *= 12f;

                        Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 30f;

                        Vector2 newVelocity = ShootSpeed.RotatedByRandom(MathHelper.ToRadians(45));

                        NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center + muzzleOffset, newVelocity, ModContent.ProjectileType<CamelVenomBolt>(), NPC.damage, 1f);
                    }

                    break;
                }

                //mortar gives this spider a missile and then shoots it
                case 3:
				{
                    break;
                }
            }

			return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0) 
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/CamelColonelTailSniperGore").Type);

                NPC Parent = Main.npc[(int)NPC.ai[3]];

                Parent.ai[0] = 4;
                Parent.localAI[0] = 0;
                Parent.localAI[1] = 0;

                foreach (NPC npc in Main.ActiveNPCs)
			    {
                    if (npc.type == ModContent.NPCType<CamelColonelTail>() && npc.ai[3] == NPC.ai[3])
                    {
                        Gore.NewGore(npc.GetSource_Death(), npc.Center, npc.velocity, ModContent.Find<ModGore>("Spooky/CamelColonelTailGore" + (npc.ai[2] + 1)).Type);

                        npc.life = 0;
                        npc.active = false;
                    }
                }
            }
        }

        public override bool CheckActive()
        {
            return false;
        }
    }
}
