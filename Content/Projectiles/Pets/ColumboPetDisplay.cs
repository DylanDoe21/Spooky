using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Spooky.Content.Buffs.Minion;

namespace Spooky.Content.Projectiles.Pets
{
    public class ColumboPetDisplay : ModProjectile
    {
        int playerStill = 0;
        bool playerFlying = false;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CharacterPreviewAnimations[Projectile.type] = ProjectileID.Sets.SimpleLoop(0, 1, 1)
            .WithOffset(-15f, 5f).WithSpriteDirection(1).WhenNotSelected(0, 0);
        }

        public override void SetDefaults()
        {
            Projectile.width = 52;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
        }
    }
}