using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Projectiles.Sentient;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientLeatherWhip : ModItem, ICauldronOutput
    {
        public override void SetDefaults()
        {
            Item.damage = 55;
			Item.DamageType = DamageClass.SummonMeleeSpeed;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.channel = true;
			Item.width = 56;
			Item.height = 50;
			Item.useTime = 40;
			Item.useAnimation = 40;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 2;
			Item.rare = ModContent.RarityType<SentientRarity>();
			Item.value = Item.buyPrice(gold: 20);
			Item.UseSound = SoundID.Item1;
			Item.shoot = ModContent.ProjectileType<SentientLeatherWhipProj>();
			Item.shootSpeed = 2.75f;
        }

		public override bool MeleePrefix() 
		{
			return true;
		}
    }
}