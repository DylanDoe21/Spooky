using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Content.Projectiles.Minibiomes.Desert;

namespace Spooky.Content.Items.Minibiomes.Desert
{
    public class DinoArmHook : ModItem
    {
       public override void SetDefaults() 
       {
			Item.CloneDefaults(ItemID.AmethystHook);
            Item.damage = 65;
			Item.DamageType = DamageClass.Generic;
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.knockBack = 2;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 3);
			Item.shoot = ModContent.ProjectileType<DinoArmHookProj>();
            Item.shootSpeed = 18f;
		}
    }
}