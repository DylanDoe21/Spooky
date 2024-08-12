using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.NPCs.PandoraBox.Projectiles;

namespace Spooky.Content.NPCs.PandoraBox
{
    public class StitchWand : ModNPC
    {
        private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };

            NPCID.Sets.ImmuneToAllBuffs[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 5;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 34;
            NPC.height = 32;
            NPC.knockBackResist = 0f;
			NPC.immortal = true;
			NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.aiStyle = -1;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPC Parent = Main.npc[(int)NPC.ai[1]];

            if (Parent.ai[0] == 1)
            {
                NPCTexture ??= ModContent.Request<Texture2D>(Texture);

                float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.4f / 2.4f * 6f)) / 2f + 0.5f;

                var effects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                Vector2 drawPosition = new Vector2(NPC.Center.X, NPC.Center.Y) - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4);
                Color color = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.Brown);

                for (int repeats = 0; repeats < 4; repeats++)
                {
                    Vector2 afterImagePosition = new Vector2(NPC.Center.X, NPC.Center.Y) + NPC.rotation.ToRotationVector2() - screenPos + new Vector2(0, NPC.gfxOffY + 4) - NPC.velocity * repeats;
                    Main.spriteBatch.Draw(NPCTexture.Value, afterImagePosition, NPC.frame, color * fade, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale * 1.2f, effects, 0f);
                }

                Main.spriteBatch.Draw(NPCTexture.Value, drawPosition, NPC.frame, color, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale * 1.2f, effects, 0f);
            }

            return true;
        }

        public override void AI()
        {
            NPC Parent = Main.npc[(int)NPC.ai[1]];

            Player player = Main.player[Parent.target];

            int Damage = Main.masterMode ? 50 / 3 : Main.expertMode ? 40 / 2 : 30;

            //kill the wand if the parent does not exist
            if (!Parent.active || Parent.life <= 0 || Parent.type != ModContent.NPCType<Stitch>())
            {
                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/StitchWandGore").Type);
                }

                NPC.active = false;
            }

            NPC.spriteDirection = Parent.spriteDirection;

            GoToPosition(Parent.spriteDirection == -1 ? -30 : 30, 20);

            if (Parent.ai[0] == 1 && (Parent.localAI[0] == 60 || Parent.localAI[0] == 80 || Parent.localAI[0] == 100))
            {
                SoundEngine.PlaySound(SoundID.Item8, NPC.Center);

                Vector2 ShootSpeed = player.Center - NPC.Center;
                ShootSpeed.Normalize();
                ShootSpeed *= 2f;

                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, ShootSpeed.X, 
                ShootSpeed.Y, ModContent.ProjectileType<StitchBolt>(), Damage, 0, NPC.target);
            }
        }

        public void GoToPosition(float X, float Y)
        {
            NPC Parent = Main.npc[(int)NPC.ai[1]];

            float goToX = (Parent.Center.X + X) - NPC.Center.X;
            float goToY = (Parent.Center.Y + Y) - NPC.Center.Y;

            float speed = 0.45f;
            
            if (NPC.velocity.X > speed)
            {
                NPC.velocity.X *= 0.95f;
            }
            if (NPC.velocity.Y > speed)
            {
                NPC.velocity.Y *= 0.95f;
            }

            if (NPC.velocity.X < goToX)
            {
                NPC.velocity.X = NPC.velocity.X + speed;
                if (NPC.velocity.X < 0f && goToX > 0f)
                {
                    NPC.velocity.X = NPC.velocity.X + speed;
                }
            }
            else if (NPC.velocity.X > goToX)
            {
                NPC.velocity.X = NPC.velocity.X - speed;
                if (NPC.velocity.X > 0f && goToX < 0f)
                {
                    NPC.velocity.X = NPC.velocity.X - speed;
                }
            }
            if (NPC.velocity.Y < goToY)
            {
                NPC.velocity.Y = NPC.velocity.Y + speed;
                if (NPC.velocity.Y < 0f && goToY > 0f)
                {
                    NPC.velocity.Y = NPC.velocity.Y + speed;
                    return;
                }
            }
            else if (NPC.velocity.Y > goToY)
            {
                NPC.velocity.Y = NPC.velocity.Y - speed;
                if (NPC.velocity.Y > 0f && goToY < 0f)
                {
                    NPC.velocity.Y = NPC.velocity.Y - speed;
                    return;
                }
            }
        }
    }
}