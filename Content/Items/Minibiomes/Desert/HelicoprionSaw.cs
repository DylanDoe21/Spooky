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
    public class HelicoprionSaw : ModItem
    {
        public override void SetStaticDefaults()
		{
			ItemID.Sets.IsChainsaw[Item.type] = true;
		}

        public override void SetDefaults()
		{
			Item.damage = 45;
            Item.axe = 20;
			Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.channel = true;
			Item.noMelee = true;
            Item.noUseGraphic = true;
			Item.width = 70;
			Item.height = 38;
			Item.useTime = 5;
			Item.useAnimation = 25;
			Item.tileBoost = 0;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 3f;
			Item.value = Item.buyPrice(gold: 10);
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item23;
			Item.shoot = ModContent.ProjectileType<HelicoprionSawProj>();
			Item.shootSpeed = 40f;
		}
    }
}