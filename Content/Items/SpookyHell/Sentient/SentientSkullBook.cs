using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Projectiles;
using Spooky.Content.Projectiles.Sentient;
using Spooky.Content.Tiles.SpookyHell.Furniture;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientSkullBook : ModItem, ICauldronOutput
    {
        public override void SetDefaults()
        {
            Item.damage = 30;
            Item.mana = 10;
			Item.DamageType = DamageClass.Magic;
			Item.autoReuse = true;
            Item.noMelee = true;
            Item.channel = true;
            Item.width = 50;
            Item.height = 56;
            Item.useTime = 25;
			Item.useAnimation = 25;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 0;
            Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 10);
            Item.UseSound = SoundID.NPCHit2;
            Item.shoot = ModContent.ProjectileType<SentientSkull>();
            Item.shootSpeed = 10f;
        }
    }
}