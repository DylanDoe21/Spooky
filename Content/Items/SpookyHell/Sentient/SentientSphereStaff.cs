using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Projectiles.Sentient;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientSphereStaff : ModItem, ICauldronOutput
    {
        public override void SetDefaults()
        {
            Item.damage = 40;
			Item.DamageType = DamageClass.Ranged;
            Item.autoReuse = true;
			Item.noMelee = true;
			Item.width = 68;
			Item.height = 70;
            //Item.useTime = 12;
			//Item.useAnimation = 12;
			//Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 3;
            Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 30);
            //Item.UseSound = SoundID.Item111;
            //Item.shoot = ModContent.ProjectileType<ToxicBubble>();
            //Item.shootSpeed = 12f;
        }
    }
}