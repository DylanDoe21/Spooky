using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Content.Items.SpookyHell;
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.NPCs.SpookyHell.Projectiles;

namespace Spooky.Content.NPCs.SpookyHell
{
    public class ValleySquidClone : ModNPC
    {
        Vector2 SavePosition;
        Vector2 SavePlayerPosition;

        public static readonly SoundStyle HitSound = new("Spooky/Content/Sounds/EggEvent/EnemyHit", SoundType.Sound);
        public static readonly SoundStyle DeathSound = new("Spooky/Content/Sounds/EggEvent/EnemyDeath", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //floats
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //floats
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 1500;
            NPC.damage = 55;
            NPC.defense = 20;
            NPC.width = 44;
            NPC.height = 70;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = HitSound;
			NPC.DeathSound = DeathSound;
            NPC.aiStyle = -1;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.85f * bossAdjustment);
        }
        
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = new(tex.Width * 0.5f, (NPC.height * 0.5f));

            for (int oldPos = 0; oldPos < NPC.oldPos.Length; oldPos++)
            {
                var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                Vector2 drawPos = NPC.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, NPC.gfxOffY + 4);
                Color color = NPC.GetAlpha(Color.Red) * (float)(((float)(NPC.oldPos.Length - oldPos) / (float)NPC.oldPos.Length) / 2);
                spriteBatch.Draw(tex, drawPos, new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, drawOrigin, NPC.scale, effects, 0f);
            }
            
            return true;
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpookyHell/ValleySquidGlow").Value;

            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(tex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1;

            if (NPC.frameCounter > 6)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0.0;
            }
            if (NPC.frame.Y >= frameHeight * 6)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override void AI()
		{
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            int Damage = Main.masterMode ? 70 / 3 : Main.expertMode ? 55 / 2 : 45;

            NPC.spriteDirection = NPC.direction;

            //EoC rotation
            Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
            float RotateX = player.Center.X - vector.X;
            float RotateY = player.Center.Y - vector.Y;
            NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

            //despawn if all players are dead
            if (player.dead)
            {
                NPC.localAI[1]++;

                NPC.ai[0] = -1;

                NPC.velocity.Y = -25;

                if (NPC.localAI[1] >= 75)
                {
                    NPC.active = false;
                }
            }

            switch ((int)NPC.ai[0])
            {
                //fly towards the player
                case 0:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] == 5)
                    {
                        SavePlayerPosition = new Vector2(player.Center.X + Main.rand.Next(-300, 300), player.Center.Y - Main.rand.Next(250, 300));
                    }

                    if (NPC.localAI[0] > 5 && NPC.localAI[0] < 150)
                    {
                        Vector2 GoTo = SavePlayerPosition;

                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 6, Main.rand.Next(8, 15));
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                    }

                    if (NPC.localAI[0] >= 150)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[0]++;

                        NPC.netUpdate = true;
                    }

                    break;
                }

                //shoot blood clots at the player
                case 1:
                {
                    NPC.localAI[0]++;

                    //save npc center
                    if (NPC.localAI[0] == 5)
                    {
                        NPC.velocity *= 0;

                        SavePosition = NPC.Center;
                    }

                    //shake before shooting
                    if (NPC.localAI[0] > 5 && NPC.localAI[0] < 35)
                    {
                        NPC.Center = new Vector2(SavePosition.X, SavePosition.Y);
                        NPC.Center += Main.rand.NextVector2Square(-7, 7);
                    }

                    if (NPC.localAI[0] >= 40)
                    {
                        NPC.velocity *= 0.8f;
                    }

                    if (NPC.localAI[0] == 40)
                    {
                        SoundEngine.PlaySound(SoundID.Item171, NPC.Center);

                        Vector2 Recoil = player.Center - NPC.Center;
                        Recoil.Normalize(); 
                        Recoil *= -10;
                        NPC.velocity = Recoil;

                        Vector2 ShootSpeed = player.Center - NPC.Center;
                        ShootSpeed.Normalize();
                        ShootSpeed *= 5.5f;

                        float Spread = Main.rand.Next(-2, 3);

                        Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 35f;
                        Vector2 position = new Vector2(NPC.Center.X, NPC.Center.Y);

                        if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                        {
                            position += muzzleOffset;
                        }

                        Projectile.NewProjectile(NPC.GetSource_FromThis(), position.X, position.Y, ShootSpeed.X + Spread, 
                        ShootSpeed.Y + Spread, ModContent.ProjectileType<NautilusSpit1>(), Damage, 0, NPC.target, 0, 0);
                    }

                    if (NPC.localAI[0] >= 80)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[0] = 0;

                        NPC.netUpdate = true;
                    }

                    break;
                }
            }
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            //dont run on multiplayer
			if (Main.netMode == NetmodeID.Server) 
            {
				return;
			}

			if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 5; numGores++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ValleySquidGore" + numGores).Type);
                }
            }
        }
    }
}